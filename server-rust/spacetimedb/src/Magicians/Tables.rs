use spacetimedb::{table, Identity, ScheduleAt};
use crate::*;

#[table(accessor = magician, public)]
pub struct Magician {
    #[primary_key] pub identity: Identity,
    #[unique] #[auto_inc] pub id: u64,
    #[index(btree)] pub game_id: u32,
    pub name: String,
    pub position: DbVector3,
    pub rotation: DbRotation2,
    pub requested_velocity: DbVector3,
    pub corrected_velocity: DbVector3,
    pub collider: ComplexCollider,
    pub collision_entries: Vec<CollisionEntry>,
    pub is_colliding: bool,
    pub state: MagicianState,
    pub kinematic_information: KinematicInformation,
    pub combat_information: CombatInformation,
    pub permissions: Vec<PermissionEntry>,
    pub stateless_timers: Vec<StatelessTimer>,
    pub timers: Vec<Timer>,
    pub bullets: Vec<ThrowingCard>,
    pub bullet_capacity: u8,
}

#[table(accessor = move_all_magicians, scheduled(move_magicians))]
pub struct MoveAllMagiciansTimer {
    #[primary_key] #[auto_inc] pub scheduled_id: u64,
    pub scheduled_at: ScheduleAt,
    pub tick_rate: f32,
    #[unique] pub game_id: u32, // One timer per game
}

#[table(accessor = gravity_magician, scheduled(apply_gravity_magician))]
pub struct GravityTimerMagician {
    #[primary_key] #[auto_inc] pub scheduled_id: u64,
    pub scheduled_at: ScheduleAt,
    pub tick_rate: f32,
    pub gravity: f32,
    #[unique] pub game_id: u32, // One timer per game
}

#[table(accessor = handle_magician_timers_timer, scheduled(handle_magician_timers))]
pub struct HandleMagicianTimersTimer {
    #[primary_key] #[auto_inc] pub scheduled_id: u64,
    pub scheduled_at: ScheduleAt,
    pub tick_rate: f32,
    #[unique] pub game_id: u32, // One timer per game
}

#[table(accessor = handle_magician_stateless_timers_timer, scheduled(handle_magician_stateless_timers))]
pub struct HandleMagicianStatelessTimersTimer {
    #[primary_key] #[auto_inc] pub scheduled_id: u64,
    pub scheduled_at: ScheduleAt,
    pub tick_rate: f32,
    #[unique] pub game_id: u32, // One timer per game
}

#[table(accessor = handle_magician_colliders_timer, scheduled(handle_magician_colliders))]
pub struct HandleMagicianCollidersTimer {
    #[primary_key] #[auto_inc] pub scheduled_id: u64,
    pub scheduled_at: ScheduleAt,
    pub tick_rate: f32,
    #[unique] pub game_id: u32, // One timer per game
}

#[table(accessor = unavailable_request_event, public, event)]
pub struct UnavailableRequestEvent {
    pub identity: Identity,
}

#[table(accessor = unavailable_request_interrupt_event, public, event)]
pub struct UnavailableRequestInterruptEvent {
    pub identity: Identity,
}

