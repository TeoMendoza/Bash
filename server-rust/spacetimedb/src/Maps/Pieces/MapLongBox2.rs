use crate::*;

pub static MAP_LONG_BOX_2_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: 13.0, y: 0.0, z: 2.0 },
    DbVector3 { x: 21.0, y: 0.0, z: 2.0 },
    DbVector3 { x: 13.0, y: 4.0, z: 2.0 },
    DbVector3 { x: 21.0, y: 4.0, z: 2.0 },
    DbVector3 { x: 21.0, y: 0.0, z: -2.0 },
    DbVector3 { x: 21.0, y: 4.0, z: -2.0 },
    DbVector3 { x: 13.0, y: 0.0, z: -2.0 },
    DbVector3 { x: 13.0, y: 4.0, z: -2.0 },
];

pub static MAP_LONG_BOX_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
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

pub fn map_long_box_2_collider() -> ComplexCollider {
    let map_long_box_2_convex_hull_0: ConvexHullCollider = create_convex_hull_collider(MAP_LONG_BOX_2_CONVEX_HULL0_VERTICES, MAP_LONG_BOX_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::None);
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![map_long_box_2_convex_hull_0];
    create_complex_collider(plane_convex_hulls, DbVector3 { x: 17.0, y: 2.0, z: 0.0 })
}

