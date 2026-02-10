use spacetimedb::{ReducerContext, Table, TimeDuration, ScheduleAt};
use spacetimedb::rand::Rng;
use std::time::Duration;
use crate::*;

pub fn handle_magician_death(ctx: &ReducerContext, magician: &mut Magician) { // Removes killed magician and adds respawn timer
    let player_option = ctx.db.logged_in_players().identity().find(magician.identity); // Adds respawn timer if player is still connected - Handles rage disconnect case
    if let Some(_player) = player_option {
        let respawn_time = ctx.timestamp.checked_add(TimeDuration::from_micros(5_000_000)).expect("Respawn Timestamp Overflow"); // 5 seconds
        let respawn_timer = RespawnTimersTimer { scheduled_id: 0, scheduled_at: ScheduleAt::Time(respawn_time), game_id: magician.game_id, identity: magician.identity};
        ctx.db.respawn_timers().insert(respawn_timer);
    }

    cleanup_on_disconnect_or_death(ctx, magician);
    ctx.db.magician().id().delete(magician.id);
}

pub fn cleanup_on_disconnect_or_death(ctx: &ReducerContext, magician: &mut Magician) { // Cleans up disconnected or dead magician related data - Data: collision entries and effects
    let collision_entry = CollisionEntry { entry_type: CollisionEntryType::Magician, id: magician.id };
    for mut other in ctx.db.magician().game_id().filter(magician.game_id) {
        if let Some(index) = other.collision_entries.iter().position(|entry| *entry == collision_entry) {
            other.collision_entries.swap_remove(index);
            ctx.db.magician().id().update(other);
        }
    }

    for player_effect in ctx.db.player_effects().target_id().filter(magician.id) {
        match player_effect.effect_type {
            EffectType::Hypnosis => undo_hypnosis_effect_magician(ctx, magician, &player_effect.effect_info.hypnosis_information), // Ensures stun effect is cleared if present - Stun effect is indefinite so it must be manually cleaned up
            _ => { }
        }
        ctx.db.player_effects().id().delete(player_effect.id);
    }
}

pub fn cleanup_on_game_end(ctx: &ReducerContext, game_id: u32) { // Cleans up data related to game - Data: magicians, effects, respawns, and scheduled reducers
    for magician in ctx.db.magician().game_id().filter(game_id) { ctx.db.magician().id().delete(magician.id); }
    for effect in ctx.db.player_effects().game_id().filter(game_id) { ctx.db.player_effects().id().delete(effect.id); }
    for respawn in ctx.db.respawn_timers().game_id().filter(game_id) { ctx.db.respawn_timers().scheduled_id().delete(respawn.scheduled_id); }

    ctx.db.move_all_magicians().game_id().delete(game_id);
    ctx.db.gravity_magician().game_id().delete(game_id);

    ctx.db.handle_magician_timers_timer().game_id().delete(game_id);
    ctx.db.handle_magician_stateless_timers_timer().game_id().delete(game_id);
    ctx.db.player_effects_table_timer().game_id().delete(game_id);
}

pub fn remove_player_info_from_game(ctx: &ReducerContext, game_id: u32) { // Decrements games current players and force ends game if 0 (force ends WIP)

    let game_option = ctx.db.game().id().find(game_id);
    if let Some(mut game) = game_option {
        if game.current_players > 0 {
            game.current_players -= 1;
        }

        let scoreboard_players = &mut game.scoreboard.players;
        for index in 0..scoreboard_players.len() {
            if scoreboard_players[index].identity == ctx.sender {
                scoreboard_players.swap_remove(index);
            }
        }

        ctx.db.game().id().update(game);
    }
}

pub fn create_game(ctx: &ReducerContext) -> Game { // Creates and inserts new game with scheduled reducers configured - Test player parameter adds fake player (parameter WIP)
    let created_game = ctx.db.game().insert(Game { id: 0, max_players: 12, current_players: 0, in_progress: false, scoreboard: Scoreboard { players: Vec::new() } });
    let tick_millis: u64 = 1000 / 60;
    let tick_rate: f32 = 1.0 / 60.0;

    ctx.db.move_all_magicians().insert(MoveAllMagiciansTimer { scheduled_id: 0, scheduled_at: ScheduleAt::Interval(Duration::from_millis(tick_millis).into()), tick_rate, game_id: created_game.id });
    ctx.db.handle_magician_timers_timer().insert(HandleMagicianTimersTimer { scheduled_id: 0, scheduled_at: ScheduleAt::Interval(Duration::from_millis(tick_millis).into()), tick_rate, game_id: created_game.id });
    ctx.db.handle_magician_stateless_timers_timer().insert(HandleMagicianStatelessTimersTimer { scheduled_id: 0, scheduled_at: ScheduleAt::Interval(Duration::from_millis(tick_millis).into()), tick_rate, game_id: created_game.id });
    ctx.db.gravity_magician().insert(GravityTimerMagician { scheduled_id: 0, scheduled_at: ScheduleAt::Interval(Duration::from_millis(tick_millis).into()), tick_rate, gravity: 20.0, game_id: created_game.id });
    ctx.db.player_effects_table_timer().insert(PlayerEffectsTableTimer {scheduled_id: 0, scheduled_at: ScheduleAt::Interval(Duration::from_millis(tick_millis).into()), tick_rate, game_id: created_game.id });
    
    //create_test_player(ctx, created_game.id);
    created_game
}

pub fn is_permission_unblocked(entries: &[PermissionEntry], key: &str) -> bool { // Returns whether permission is free
    let entry: &PermissionEntry = get_permission_entry(entries, key).expect("Permission entry not found");
    return entry.subscribers.is_empty()
}

pub fn get_permission_entry<'a>(entries: &'a [PermissionEntry], key: &str) -> Option<&'a PermissionEntry> { // Gets requested permission entry
    for entry in entries.iter() {
        if entry.key == key {
            return Some(entry);
        }
    }
    
    None
}

pub fn add_subscriber_to_permission(entries: &mut [PermissionEntry], key: &str, subscriber: &str) { // Adds subscriber to permission entry
    for entry in entries.iter_mut() {
        if entry.key == key {
            add_subscriber(&mut entry.subscribers, subscriber);
            return;
        }
    }

    panic!("Permission entry not found: {}", key);
}

pub fn remove_subscriber_from_permission(entries: &mut [PermissionEntry], key: &str, subscriber: &str) { // Removes subscriber from permission entry
    for entry in entries.iter_mut() {
        if entry.key == key {
            remove_subscriber(&mut entry.subscribers, subscriber);
            return;
        }
    }

    panic!("Permission entry not found: {}", key);
}

pub fn add_subscriber(subscribers: &mut Vec<String>, reason: &str) {
    subscribers.push(reason.to_string());
}

pub fn remove_subscriber(subscribers: &mut Vec<String>, reason: &str)  {
    if let Some(index) = subscribers.iter().position(|existing| existing == reason) {
        subscribers.swap_remove(index); // O(1) instead of O(n) remove method
    }
}

pub fn generate_random_username(ctx: &ReducerContext) -> String {
    const ADJECTIVES: &[&str] = &[
        "Fun", "Cool", "Wild", "Zen", "Brave", "Swift", "Sly", "Happy", "Merry", "Chill",
        "Spicy", "Sunny", "Lucky", "Zappy", "Fuzzy", "Snug", "Peppy", "Witty",
    ];

    const NOUNS: &[&str] = &[
        "Monk", "Fox", "Wolf", "Bear", "Lion", "Hawk", "Crow", "Panda", "Otter", "Moth",
        "Toad", "Crab", "Koala", "Gecko", "Tiger", "Bunny", "Raven", "Squid",
    ];

    let mut Rng = ctx.rng();

    for _ in 0..32 {
        let Adjective = ADJECTIVES[Rng.gen_range(0..ADJECTIVES.len())];
        let Noun = NOUNS[Rng.gen_range(0..NOUNS.len())];

        let DigitCount = if Rng.gen_bool(0.65) { 2 } else { 1 };
        let MaxDigits = if DigitCount == 2 { 100 } else { 10 };
        let Number = Rng.gen_range(0..MaxDigits).to_string();

        let CandidateWithNumber = format!("{Adjective}{Noun}{Number}");
        if CandidateWithNumber.len() <= 10 {
            return CandidateWithNumber;
        }

        let CandidateNoNumber = format!("{Adjective}{Noun}");
        if CandidateNoNumber.len() <= 10 {
            return CandidateNoNumber;
        }
    }

    let FallbackNumber = Rng.gen_range(0..1000).to_string();
    let Fallback = format!("User{FallbackNumber}");
    Fallback.chars().take(10).collect()
}







