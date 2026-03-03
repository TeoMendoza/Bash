use crate::*;

pub static RIGHT_RAMP_2_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: 12.4, y: 0.0, z: -18.125 },
    DbVector3 { x: 12.4, y: 0.0, z: -21.875 },
    DbVector3 { x: 4.0, y: 0.0, z: -18.125 },
    DbVector3 { x: 4.0, y: 0.0, z: -21.875 },
    DbVector3 { x: 4.0, y: 4.0, z: -18.125 },
    DbVector3 { x: 4.0, y: 4.0, z: -21.875 },
];

pub static RIGHT_RAMP_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    2,
    1,
    0,
    0,
    1,
    5,
    2,
    0,
    4,
    0,
    5,
    4,
    5,
    2,
    4,
    2,
    5,
    3,
    5,
    1,
    3,
    1,
    2,
    3,
];

pub fn right_ramp_2_collider() -> ComplexCollider {
    let right_ramp_2_convex_hull_0: ConvexHullCollider = ConvexHullCollider { vertices_local: RIGHT_RAMP_2_CONVEX_HULL0_VERTICES.to_vec(), triangle_indices_local: RIGHT_RAMP_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL.to_vec(), margin: 0.0 };
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![right_ramp_2_convex_hull_0];
    ComplexCollider { convex_hulls: plane_convex_hulls, center_point: DbVector3 { x: 8.2, y: 2.0, z: -20.0 } }
}

