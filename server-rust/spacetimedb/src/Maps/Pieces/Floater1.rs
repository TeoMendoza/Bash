use crate::*;

pub static FLOATER_1_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: 4.5, y: 6.5, z: 2.5 },
    DbVector3 { x: 9.5, y: 6.5, z: 2.5 },
    DbVector3 { x: 4.5, y: 7.0, z: 2.5 },
    DbVector3 { x: 9.5, y: 7.0, z: 2.5 },
    DbVector3 { x: 9.5, y: 6.5, z: -2.5 },
    DbVector3 { x: 9.5, y: 7.0, z: -2.5 },
    DbVector3 { x: 4.5, y: 6.5, z: -2.5 },
    DbVector3 { x: 4.5, y: 7.0, z: -2.5 },
];

pub static FLOATER_1_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    0,
    1,
    2,
    5,
    1,
    4,
    1,
    0,
    4,
    0,
    2,
    6,
    5,
    4,
    6,
    4,
    0,
    6,
    1,
    5,
    3,
    5,
    2,
    3,
    2,
    1,
    3,
    2,
    5,
    7,
    5,
    6,
    7,
    6,
    2,
    7,
];

pub fn floater_1_collider() -> ComplexCollider {
    let floater_1_convex_hull_0: ConvexHullCollider = ConvexHullCollider { vertices_local: FLOATER_1_CONVEX_HULL0_VERTICES.to_vec(), triangle_indices_local: FLOATER_1_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL.to_vec(), margin: 0.0, collider_type: ConvexHullColliderType::None };
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![floater_1_convex_hull_0];
    ComplexCollider { convex_hulls: plane_convex_hulls, center_point: DbVector3 { x: 7.0, y: 6.75, z: 0.0 } }
}

