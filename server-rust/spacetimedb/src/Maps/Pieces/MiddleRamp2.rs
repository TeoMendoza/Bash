use crate::*;

pub static MIDDLE_RAMP_2_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -4.0, y: 0.0, z: -30.2750015 },
    DbVector3 { x: 4.0, y: 0.0, z: -30.2750015 },
    DbVector3 { x: 4.0, y: 0.0, z: -21.875 },
    DbVector3 { x: -4.0, y: 0.0, z: -21.875 },
    DbVector3 { x: -4.0, y: 4.0, z: -21.875 },
    DbVector3 { x: 4.0, y: 4.0, z: -21.875 },
];

pub static MIDDLE_RAMP_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    0,
    4,
    1,
    0,
    1,
    2,
    0,
    2,
    3,
    2,
    4,
    3,
    4,
    0,
    3,
    1,
    4,
    5,
    4,
    2,
    5,
    2,
    1,
    5,
];

pub fn middle_ramp_2_collider() -> ComplexCollider {
    let middle_ramp_2_convex_hull_0: ConvexHullCollider = ConvexHullCollider { vertices_local: MIDDLE_RAMP_2_CONVEX_HULL0_VERTICES.to_vec(), triangle_indices_local: MIDDLE_RAMP_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL.to_vec(), margin: 0.0 };
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![middle_ramp_2_convex_hull_0];
    ComplexCollider { convex_hulls: plane_convex_hulls, center_point: DbVector3 { x: 0.0, y: 1.0, z: -24.0 } }
}

