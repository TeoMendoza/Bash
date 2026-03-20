use crate::*;

pub static FLOOR_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: 37.5, y: -5.0, z: 37.5 },
    DbVector3 { x: -37.5, y: -5.0, z: 37.5 },
    DbVector3 { x: -37.5, y: -5.0, z: -37.5 },
    DbVector3 { x: 37.5, y: -5.0, z: -37.5 },
    DbVector3 { x: 37.5, y: 0.0, z: 37.5 },
    DbVector3 { x: -37.5, y: 0.0, z: 37.5 },
    DbVector3 { x: -37.5, y: 0.0, z: -37.5 },
    DbVector3 { x: 37.5, y: 0.0, z: -37.5 },
];

pub static FLOOR_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    1,
    6,
    2,
    1,
    2,
    0,
    1,
    0,
    4,
    0,
    2,
    3,
    2,
    6,
    3,
    4,
    0,
    3,
    6,
    1,
    5,
    1,
    4,
    5,
    4,
    6,
    5,
    6,
    4,
    7,
    4,
    3,
    7,
    3,
    6,
    7,
];

pub fn floor_collider() -> ComplexCollider {
    let floor_convex_hull_0: ConvexHullCollider = ConvexHullCollider { vertices_local: FLOOR_CONVEX_HULL0_VERTICES.to_vec(), triangle_indices_local: FLOOR_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL.to_vec(), margin: 0.0, collider_type: ConvexHullColliderType::None };
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![floor_convex_hull_0];
    ComplexCollider { convex_hulls: plane_convex_hulls, center_point: DbVector3 { x: 0.0, y: -0.5, z: 0.0 } }
}

