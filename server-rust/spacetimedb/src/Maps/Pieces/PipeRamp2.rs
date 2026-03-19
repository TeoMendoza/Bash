use crate::*;

pub static PIPE_RAMP_2_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -2.5, y: 4.0, z: 18.125 },
    DbVector3 { x: 2.5, y: 4.0, z: 18.125 },
    DbVector3 { x: -2.5, y: 4.0, z: 13.125 },
    DbVector3 { x: 2.5, y: 4.0, z: 13.125 },
    DbVector3 { x: -2.5, y: 7.0, z: 13.125 },
    DbVector3 { x: 2.5, y: 7.0, z: 13.125 },
];

pub static PIPE_RAMP_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    0,
    1,
    4,
    0,
    4,
    2,
    0,
    2,
    1,
    1,
    2,
    3,
    2,
    4,
    3,
    4,
    1,
    5,
    1,
    3,
    5,
    3,
    4,
    5,
];

pub fn pipe_ramp_2_collider() -> ComplexCollider {
    let pipe_ramp_2_convex_hull_0: ConvexHullCollider = ConvexHullCollider { vertices_local: PIPE_RAMP_2_CONVEX_HULL0_VERTICES.to_vec(), triangle_indices_local: PIPE_RAMP_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL.to_vec(), margin: 0.0, collider_type: ConvexHullColliderType::None };
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![pipe_ramp_2_convex_hull_0];
    ComplexCollider { convex_hulls: plane_convex_hulls, center_point: DbVector3 { x: 0.0, y: 5.5, z: 15.625 } }
}



