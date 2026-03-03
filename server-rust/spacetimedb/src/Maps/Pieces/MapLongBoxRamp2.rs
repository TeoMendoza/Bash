use crate::*;

pub static MAP_LONG_BOX_RAMP_2_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: 29.0, y: 0.0, z: -2.00000072 },
    DbVector3 { x: 21.0, y: 0.0, z: -2.00000072 },
    DbVector3 { x: 21.0, y: 4.0, z: -2.00000072 },
    DbVector3 { x: 29.0, y: 0.0, z: 2.00000072 },
    DbVector3 { x: 21.0, y: 0.0, z: 2.00000072 },
    DbVector3 { x: 21.0, y: 4.0, z: 2.00000072 },
];

pub static MAP_LONG_BOX_RAMP_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    1,
    5,
    2,
    1,
    2,
    0,
    0,
    2,
    5,
    1,
    0,
    3,
    0,
    5,
    3,
    5,
    1,
    4,
    1,
    3,
    4,
    3,
    5,
    4,
];

pub fn map_long_box_ramp_2_collider() -> ComplexCollider {
    let map_long_box_ramp_2_convex_hull_0: ConvexHullCollider = ConvexHullCollider { vertices_local: MAP_LONG_BOX_RAMP_2_CONVEX_HULL0_VERTICES.to_vec(), triangle_indices_local: MAP_LONG_BOX_RAMP_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL.to_vec(), margin: 0.0 };
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![map_long_box_ramp_2_convex_hull_0];
    ComplexCollider { convex_hulls: plane_convex_hulls, center_point: DbVector3 { x: 25.0, y: 2.0, z: 0.0 } }
}

