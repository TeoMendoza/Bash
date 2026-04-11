use crate::*;

pub static MAP_LONG_BOX_RAMP_1_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -29.0, y: 0.0, z: -2.00000072 },
    DbVector3 { x: -21.0, y: 0.0, z: -2.00000072 },
    DbVector3 { x: -21.0, y: 4.0, z: -2.00000072 },
    DbVector3 { x: -29.0, y: 0.0, z: 2.00000072 },
    DbVector3 { x: -21.0, y: 0.0, z: 2.00000072 },
    DbVector3 { x: -21.0, y: 4.0, z: 2.00000072 },
];

pub static MAP_LONG_BOX_RAMP_1_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    0,
    5,
    2,
    0,
    2,
    1,
    1,
    2,
    5,
    0,
    1,
    3,
    5,
    0,
    3,
    1,
    5,
    4,
    5,
    3,
    4,
    3,
    1,
    4,
];

pub fn map_long_box_ramp_1_collider() -> ComplexCollider {
    let map_long_box_ramp_1_convex_hull_0: ConvexHullCollider = create_convex_hull_collider(MAP_LONG_BOX_RAMP_1_CONVEX_HULL0_VERTICES, MAP_LONG_BOX_RAMP_1_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::None);
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![map_long_box_ramp_1_convex_hull_0];
    create_complex_collider(plane_convex_hulls, DbVector3 { x: -25.0, y: 2.0, z: 0.0 })
}

