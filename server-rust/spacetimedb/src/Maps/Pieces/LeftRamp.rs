use crate::*;

pub static LEFT_RAMP_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -12.4, y: 0.0, z: 18.125 },
    DbVector3 { x: -12.4, y: 0.0, z: 21.875 },
    DbVector3 { x: -4.0, y: 0.0, z: 21.875 },
    DbVector3 { x: -4.0, y: 0.0, z: 18.125 },
    DbVector3 { x: -4.0, y: 4.0, z: 18.125 },
    DbVector3 { x: -4.0, y: 4.0, z: 21.875 },
];

pub static LEFT_RAMP_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    0,
    1,
    4,
    0,
    2,
    1,
    0,
    4,
    3,
    4,
    2,
    3,
    2,
    0,
    3,
    2,
    4,
    5,
    4,
    1,
    5,
    1,
    2,
    5,
];

pub fn left_ramp_collider() -> ComplexCollider {
    let left_ramp_convex_hull_0: ConvexHullCollider = ConvexHullCollider { vertices_local: LEFT_RAMP_CONVEX_HULL0_VERTICES.to_vec(), triangle_indices_local: LEFT_RAMP_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL.to_vec(), margin: 0.0 };
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![left_ramp_convex_hull_0];
    ComplexCollider { convex_hulls: plane_convex_hulls, center_point: DbVector3 { x: -8.2, y: 1.0, z: 20.0 } }
}

