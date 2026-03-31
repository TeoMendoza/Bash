use spacetimedb::{SpacetimeType};
use crate::*;

#[derive(SpacetimeType, Copy, Clone, Debug, PartialEq, Eq)]
pub enum RaycastHitType {
    None,
    Magician,
    MapPiece,
}

#[derive(SpacetimeType, Clone, Debug)]
pub struct Raycast {
    pub hit: bool,
    pub hit_distance: f32,
    pub hit_point: DbVector3,
    pub hit_type: RaycastHitType,
    pub hit_entity_id: u64,
    pub collider_type: ConvexHullColliderType,
    pub hit_name: String
}
