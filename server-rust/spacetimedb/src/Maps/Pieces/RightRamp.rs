use crate::*;

pub static RIGHT_RAMP_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: 12.4, y: 0.0, z: 21.875 },
    DbVector3 { x: 12.4, y: 0.0, z: 18.125 },
    DbVector3 { x: 4.0, y: 0.0, z: 18.125 },
    DbVector3 { x: 4.0, y: 0.0, z: 21.875 },
    DbVector3 { x: 4.0, y: 4.0, z: 21.875 },
    DbVector3 { x: 4.0, y: 4.0, z: 18.125 },
];

pub static RIGHT_RAMP_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    2,
    1,
    0,
    0,
    1,
    4,
    2,
    0,
    3,
    0,
    4,
    3,
    4,
    2,
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

pub fn right_ramp_collider() -> ComplexCollider {
    let right_ramp_convex_hull_0: ConvexHullCollider = create_convex_hull_collider(RIGHT_RAMP_CONVEX_HULL0_VERTICES, RIGHT_RAMP_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::None);
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![right_ramp_convex_hull_0];
    create_complex_collider(plane_convex_hulls, DbVector3 { x: 8.2, y: 1.0, z: 20.0 })
}

