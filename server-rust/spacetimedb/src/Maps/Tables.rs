use spacetimedb::{table};
use crate::*;

#[table(accessor = map, public)]
pub struct Map {
    #[primary_key] #[auto_inc] pub id: u64,
    #[unique] pub name: String, // Used to link to prefab on client side
    pub collider: ComplexCollider,
}


#[table(accessor = map_respawn_points)]
pub struct MapRespawnPoint {
    #[primary_key] #[auto_inc] pub id: u64,
    #[unique] pub name: String,
    #[unique] pub position: DbVector3,
    #[unique] pub rotation: DbRotation2
}