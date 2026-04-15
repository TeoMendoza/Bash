use crate::*;

pub static PLATFORM_2_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -4.0, y: 0.0, z: -18.125 },
    DbVector3 { x: 4.0, y: 0.0, z: -18.125 },
    DbVector3 { x: 4.0, y: 0.0, z: -21.875 },
    DbVector3 { x: -4.0, y: 0.0, z: -21.875 },
    DbVector3 { x: -4.0, y: 4.0, z: -18.125 },
    DbVector3 { x: 4.0, y: 4.0, z: -18.125 },
    DbVector3 { x: 4.0, y: 4.0, z: -21.875 },
    DbVector3 { x: -4.0, y: 4.0, z: -21.875 },
];

pub static PLATFORM_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    0,
    2,
    1,
    1,
    2,
    6,
    0,
    1,
    4,
    6,
    2,
    3,
    2,
    0,
    3,
    0,
    4,
    3,
    1,
    6,
    5,
    6,
    4,
    5,
    4,
    1,
    5,
    4,
    6,
    7,
    6,
    3,
    7,
    3,
    4,
    7,
];

pub fn platform_2_collider() -> ComplexCollider {
    let platform_2_convex_hull_0: ConvexHullCollider = create_convex_hull_collider(PLATFORM_2_CONVEX_HULL0_VERTICES, PLATFORM_2_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::None);
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![platform_2_convex_hull_0];
    create_complex_collider(plane_convex_hulls, DbVector3 { x: 0.0, y: 2.0, z: -20.0 })
}

