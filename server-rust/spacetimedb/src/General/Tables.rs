use spacetimedb::{Identity, ScheduleAt, table};
use crate::*;

#[table(accessor = logged_in_players, public)] // Public for lobby purposes
#[table(accessor = logged_out_players)]
pub struct Player {
    #[primary_key] pub identity: Identity,
    #[unique] #[auto_inc] pub id: u64,
    pub name: String // In game name
}

#[table(accessor = game, public)]
pub struct Game {
    #[unique] #[primary_key] #[auto_inc] pub id: u32,
    #[index(btree)] pub in_progress: bool,
    pub max_players: u32,
    pub current_players: u32,
    pub scoreboard: Scoreboard
}

#[table(accessor = game_timers, public, scheduled(handle_game_end))]
pub struct GameTimersTimer {
    #[primary_key] #[auto_inc] pub scheduled_id: u64,
    #[unique] pub game_id: u32,
    pub scheduled_at: ScheduleAt,
}

#[table(accessor = respawn_timers, public, scheduled(handle_respawn))]
pub struct RespawnTimersTimer {
    #[primary_key] #[auto_inc] pub scheduled_id: u64,
    #[index(btree)] pub game_id: u32,
    #[unique] pub identity: Identity, // Used to find target in logged in players (confirms existence)
    pub scheduled_at: ScheduleAt,
}

#[table(accessor = debug_table, public)]
pub struct ModuleDebug {
    #[primary_key] #[auto_inc] pub id: u64,
    pub debug_on: bool
}

#[table(accessor = kill_logs, public, event)]
pub struct KillLog {
    pub game_id: u32,
    pub killer_name: String,
    pub killed_name: String
}