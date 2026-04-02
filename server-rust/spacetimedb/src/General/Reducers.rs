use spacetimedb::{ReducerContext, ScheduleAt, Table, TimeDuration, rand::Rng, reducer};
use crate::*;

#[reducer(init)] // Init gets called at DB publish
pub fn init(ctx: &ReducerContext) // Adds map pieces to database with colliders (map pieces are static)
{
    log::info!("Initializing...");

    ctx.db.map().insert(Map {id: 0, name: "Pipe".to_string(), collider: pipe_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Pipe Platform".to_string(), collider: pipe_platform_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Floor".to_string(), collider: floor_collider() });

    ctx.db.map().insert(Map {id: 0, name: "Pipe Ramp".to_string(), collider: pipe_ramp_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Pipe Ramp 2".to_string(), collider: pipe_ramp_2_collider() });
    
    ctx.db.map().insert(Map {id: 0, name: "Floater 1".to_string(), collider: floater_1_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Floater 2".to_string(), collider: floater_2_collider() });
    
    ctx.db.map().insert(Map {id: 0, name: "Left Ramp".to_string(), collider: left_ramp_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Left Ramp 2".to_string(), collider: left_ramp_2_collider() });

    ctx.db.map().insert(Map {id: 0, name: "Map Box 1".to_string(), collider: map_box_1_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box 2".to_string(), collider: map_box_2_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box 3".to_string(), collider: map_box_3_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box 4".to_string(), collider: map_box_4_collider() });

    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 1".to_string(), collider: map_box_edge_1_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 1 - Jump 1".to_string(), collider: map_box_edge_1_jump_1_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 1 - Jump 2".to_string(), collider: map_box_edge_1_jump_2_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 1 - Jump 3".to_string(), collider: map_box_edge_1_jump_3_collider() });

    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 2".to_string(), collider: map_box_edge_2_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 2 - Jump 1".to_string(), collider: map_box_edge_2_jump_1_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 2 - Jump 2".to_string(), collider: map_box_edge_2_jump_2_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 2 - Jump 3".to_string(), collider: map_box_edge_2_jump_3_collider() });

    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 3".to_string(), collider: map_box_edge_3_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 3 - Jump 1".to_string(), collider: map_box_edge_3_jump_1_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 3 - Jump 2".to_string(), collider: map_box_edge_3_jump_2_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 3 - Jump 3".to_string(), collider: map_box_edge_3_jump_3_collider() });

    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 4".to_string(), collider: map_box_edge_4_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 4 - Jump 1".to_string(), collider: map_box_edge_4_jump_1_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 4 - Jump 2".to_string(), collider: map_box_edge_4_jump_2_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Box Edge 4 - Jump 3".to_string(), collider: map_box_edge_4_jump_3_collider() });

    ctx.db.map().insert(Map {id: 0, name: "Map Long Box 1".to_string(), collider: map_long_box_1_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Long Box 2".to_string(), collider: map_long_box_2_collider() });

    ctx.db.map().insert(Map {id: 0, name: "Map Long Box Ramp 1".to_string(), collider: map_long_box_ramp_1_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Map Long Box Ramp 2".to_string(), collider: map_long_box_ramp_2_collider() });

    ctx.db.map().insert(Map {id: 0, name: "Middle Ramp".to_string(), collider: middle_ramp_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Middle Ramp 2".to_string(), collider: middle_ramp_2_collider() });

    ctx.db.map().insert(Map {id: 0, name: "Platform".to_string(), collider: platform_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Platform 2".to_string(), collider: platform_2_collider() });

    ctx.db.map().insert(Map {id: 0, name: "Right Ramp".to_string(), collider: right_ramp_collider() });
    ctx.db.map().insert(Map {id: 0, name: "Right Ramp 2".to_string(), collider: right_ramp_2_collider() });


    ctx.db.map_respawn_points().insert(MapRespawnPoint { id: 0, name: "Center".to_string(), position: DbVector3 { x: 0.0, y: 7.0, z: 0.0 }});
    ctx.db.map_respawn_points().insert(MapRespawnPoint { id: 0, name: "Top Right".to_string(), position: DbVector3 { x: 20.0, y: 0.0, z: 20.0 }});
    ctx.db.map_respawn_points().insert(MapRespawnPoint { id: 0, name: "Top Left".to_string(), position: DbVector3 { x: -20.0, y: 0.0, z: 20.0 }});
    ctx.db.map_respawn_points().insert(MapRespawnPoint { id: 0, name: "Bottom Left".to_string(), position: DbVector3 { x: -20.0, y: 0.0, z: -20.0 }});
    ctx.db.map_respawn_points().insert(MapRespawnPoint { id: 0, name: "Bottom Right".to_string(), position: DbVector3 { x: 20.0, y: 0.0, z: -20.0 }});

    ctx.db.debug_table().insert(ModuleDebug { id: 0, debug_on: false } );
}

#[reducer(client_connected)]
pub fn connect(ctx: &ReducerContext) // Adds player to logged_in_players and out of logged_out_players - Creates player if they don't exist in logged_out_players
{
    if IsUnitTestModeEnabled(ctx) { return; } // Cuts reducer short if we are unit testing to ensure persistent data while tests run

    let logged_out_player_option = ctx.db.logged_out_players().identity().find(ctx.sender());

    if let Some(logged_out_player) = logged_out_player_option {
        ctx.db.logged_in_players().insert(logged_out_player);
        ctx.db.logged_out_players().identity().delete(ctx.sender());
    } 

    else {
        let name = generate_random_username(&ctx);
        ctx.db.logged_in_players().insert(Player {id: 0, identity: ctx.sender(), name: name });
    }

    log::info!("{} just connected.", ctx.sender());
}

#[reducer(client_disconnected)]
pub fn disconnect(ctx: &ReducerContext) // Cleans up data related to player - Cases: player is in match, dead & respawning, or in lobby
{
    if IsUnitTestModeEnabled(ctx) { return; } // Cuts reducer short if we are unit testing to ensure persistent data while tests run

    let player = ctx.db.logged_in_players().identity().find(ctx.sender()).expect("Player not found");

    let magician_option = ctx.db.magician().identity().find(ctx.sender());
    let respawn_timer_option = ctx.db.respawn_timers().identity().find(ctx.sender()); 

    if let Some(mut magician) = magician_option { // Case: in match
        cleanup_on_disconnect_or_death(ctx, &mut magician);
        remove_player_info_from_game(ctx, magician.game_id);
        ctx.db.magician().identity().delete(player.identity);
    }

    else if let Some(respawn_timer) = respawn_timer_option { // Case: dead & respawning
        remove_player_info_from_game(ctx, respawn_timer.game_id);
        ctx.db.respawn_timers().scheduled_id().delete(respawn_timer.scheduled_id); 
    }
    
    ctx.db.logged_in_players().identity().delete(player.identity); // Case: In lobby but still executed for all cases
    ctx.db.logged_out_players().insert(player);
    
    log::info!("{} just disconnected.", ctx.sender());
}

#[reducer]
pub fn try_join_game(ctx: &ReducerContext) // Adds player to first unstarted game - Creates new game if all games are in progress
{
    let player_option = ctx.db.logged_in_players().identity().find(ctx.sender());
    if let Some(player) = player_option {
        let scoreboard_player = ScoreboardPlayer { identity: ctx.sender(), name: player.name.clone(), score: 0 };
        let mut game: Game = match ctx.db.game().in_progress().filter(false).next() { // Finds unstarted game or creates new if none
            Some(mut existing_game) => { 
                existing_game.current_players += 1;
                existing_game 
            },
            None =>  { 
                let mut created_game = create_game(ctx);
                created_game.current_players += 1;
                created_game
            }
        };

        game.scoreboard.players.push(scoreboard_player);
        if game.current_players == 2 && game.in_progress != true { // Starts game if full - No new players can join, players can leave and rejoin (rejoin WIP)
            game.in_progress = true;
            for scoreboard_player in game.scoreboard.players.iter_mut() {
                scoreboard_player.score = 0;
            }

            let end_time = ctx.timestamp.checked_add(TimeDuration::from_micros(900_000_000)).expect("Match End Time Timestamp Overflow"); // 15 Minutes
            let game_end_timer = GameTimersTimer {scheduled_id: 0, scheduled_at: ScheduleAt::Time(end_time), game_id: game.id};
            ctx.db.game_timers().insert(game_end_timer);
        }

        let effects = vec![create_invincible_effect(5.0)];
        let initial_spawn_point = ctx.db.map_respawn_points().name().find("Center".to_string()).expect("Center Spawn Point Must Exist!").position;
        let magician_config = MagicianConfig {player, game_id: game.id, position: initial_spawn_point };
        let magician = create_magician(magician_config);

        let inserted_magician = ctx.db.magician().insert(magician);
        add_effects_to_table(ctx, effects, inserted_magician.id, inserted_magician.id, game.id);   

        ctx.db.game().id().update(game);
    } 
}

#[reducer]
pub fn try_leave_game(ctx: &ReducerContext) // Cleans up data related to player but only match related data - Similar functionality to disconnect
{
    let player_option = ctx.db.logged_in_players().identity().find(ctx.sender());
    let magician_option = ctx.db.magician().identity().find(ctx.sender());
    let respawn_timer_option = ctx.db.respawn_timers().identity().find(ctx.sender()); 

    if let Some(player) = player_option {
        if let Some(mut magician) = magician_option {
            cleanup_on_disconnect_or_death(ctx, &mut magician);
            remove_player_info_from_game(ctx, magician.game_id);
            ctx.db.magician().identity().delete(player.identity);
        }

        else if let Some(respawn_timer) = respawn_timer_option {
            remove_player_info_from_game(ctx, respawn_timer.game_id);
            ctx.db.respawn_timers().scheduled_id().delete(respawn_timer.scheduled_id); 
        }
    }
}

#[reducer]
pub fn handle_respawn(ctx: &ReducerContext, timer: RespawnTimersTimer) // Respawns player into game
{ 
    let player_option = ctx.db.logged_in_players().identity().find(timer.identity); // Player not gauranteed to exist - Not sure if this is true, but case handled regardless
    if let Some(player) = player_option {
        let game_option = ctx.db.game().id().find(timer.game_id);
        if game_option.is_some() {
            let spawn_points: Vec<_> = ctx.db.map_respawn_points().iter().collect();
            if spawn_points.is_empty() {
                ctx.db.respawn_timers().scheduled_id().delete(timer.scheduled_id);
                return;
            }

            let effects = vec![create_invincible_effect(5.0)];

            let random_index = ctx.rng().gen_range(0..spawn_points.len());
            let spawn_point = spawn_points[random_index].position;

            let magician_config = MagicianConfig { player, game_id: timer.game_id, position: spawn_point };
            let magician = create_magician(magician_config);

            let inserted_magician = ctx.db.magician().insert(magician);
            add_effects_to_table(ctx, effects, inserted_magician.id, inserted_magician.id, timer.game_id);       
        }
    }
        
    ctx.db.respawn_timers().scheduled_id().delete(timer.scheduled_id); 
}

#[reducer]
pub fn handle_game_end(ctx: &ReducerContext, timer: GameTimersTimer) // Cleans up data related to game - Data: magicians, effects, respawns, and scheduled reducers
{
    cleanup_on_game_end(ctx, timer.game_id);
    ctx.db.game_timers().scheduled_id().delete(timer.scheduled_id);
    ctx.db.game().id().delete(timer.game_id);
}

#[reducer]
pub fn debug_mode(ctx: &ReducerContext) {
    let mut debug_row = ctx.db.debug_table().id().find(1).expect("Debug Row Should Exist!");

    log::info!("Before toggle: {}", debug_row.debug_on);

    debug_row.debug_on = !debug_row.debug_on;
    ctx.db.debug_table().id().update(debug_row);

    let updated_row = ctx.db.debug_table().id().find(1).expect("Debug Row Should Exist After Update!");
    log::info!("After toggle: {}", updated_row.debug_on);
}