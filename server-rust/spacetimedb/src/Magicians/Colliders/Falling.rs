use crate::*;

pub static FALLING_LEG_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.25, y: 0.99, z: 0.01 },
    DbVector3 { x: -0.13, y: 0.97, z: -0.08 },
    DbVector3 { x: 0.01, y: 0.98, z: -0.08 },
    DbVector3 { x: 0.135, y: 1.0, z: -0.03 },
    DbVector3 { x: -0.32, y: 0.94, z: 0.12 },
    DbVector3 { x: -0.205, y: 0.875, z: 0.215 },
    DbVector3 { x: -0.085, y: 0.81, z: 0.285 },
    DbVector3 { x: 0.075, y: 0.9, z: 0.145 },
    DbVector3 { x: 0.165, y: 0.855, z: 0.235 },
    DbVector3 { x: 0.245, y: 0.77, z: 0.28 },
    DbVector3 { x: -0.3, y: 0.665, z: 0.095 },
    DbVector3 { x: -0.21, y: 0.555, z: 0.135 },
    DbVector3 { x: -0.15, y: 0.39, z: 0.12 },
    DbVector3 { x: -0.175, y: 0.22, z: 0.07 },
    DbVector3 { x: 0.105, y: 0.68, z: 0.05 },
    DbVector3 { x: 0.145, y: 0.53, z: 0.04 },
    DbVector3 { x: 0.12, y: 0.39, z: 0.005 },
    DbVector3 { x: 0.095, y: 0.19, z: -0.02 },
    DbVector3 { x: -0.095, y: 0.69, z: 0.29 },
    DbVector3 { x: 0.215, y: 0.71, z: 0.29 },
    DbVector3 { x: -0.355, y: 0.86, z: 0.02 },
    DbVector3 { x: -0.29, y: 0.72, z: 0.01 },
    DbVector3 { x: -0.255, y: 0.5, z: 0.025 },
    DbVector3 { x: -0.25, y: 0.3, z: 0.015 },
    DbVector3 { x: -0.255, y: 0.21, z: 0.17 },
    DbVector3 { x: -0.12, y: 0.19, z: 0.195 },
    DbVector3 { x: 0.035, y: 0.15, z: 0.09 },
    DbVector3 { x: 0.14, y: 0.16, z: 0.025 },
];

pub static FALLING_LEG_TRIANGLE_INDICES_LOCAL: &[i32] = &[9, 27, 3, 20, 24, 4, 24, 20, 23, 20, 1, 23, 9, 3, 8, 3, 4, 8, 4, 3, 0, 3, 2, 0, 2, 1, 0, 1, 20, 0, 20, 4, 0, 24, 25, 18, 24, 23, 13, 23, 1, 17, 1, 2, 17, 2, 3, 17, 3, 27, 17, 13, 23, 17, 27, 9, 19, 18, 25, 19, 25, 24, 26, 24, 13, 26, 13, 17, 26, 17, 27, 26, 27, 19, 26, 19, 25, 26, 9, 8, 6, 18, 19, 6, 19, 9, 6, 4, 24, 5, 24, 18, 5, 18, 6, 5, 6, 8, 5, 8, 4, 5];

pub static FALLING_BODY_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.22, y: 0.9, z: -0.03 },
    DbVector3 { x: -0.08, y: 0.86, z: -0.19 },
    DbVector3 { x: 0.06, y: 0.9, z: -0.17 },
    DbVector3 { x: 0.17, y: 0.95, z: -0.03 },
    DbVector3 { x: -0.24, y: 1.02, z: 0.02 },
    DbVector3 { x: -0.11, y: 0.99, z: -0.22 },
    DbVector3 { x: 0.08, y: 1.02, z: -0.19 },
    DbVector3 { x: 0.185, y: 1.07, z: -0.01 },
    DbVector3 { x: -0.235, y: 1.13, z: 0.06 },
    DbVector3 { x: -0.12, y: 1.11, z: -0.16 },
    DbVector3 { x: 0.07, y: 1.13, z: -0.12 },
    DbVector3 { x: 0.17, y: 1.17, z: 0.04 },
    DbVector3 { x: -0.255, y: 1.23, z: 0.115 },
    DbVector3 { x: -0.125, y: 1.215, z: -0.055 },
    DbVector3 { x: 0.07, y: 1.23, z: -0.02 },
    DbVector3 { x: 0.175, y: 1.25, z: 0.105 },
    DbVector3 { x: -0.235, y: 1.3, z: 0.175 },
    DbVector3 { x: -0.07, y: 1.305, z: 0.215 },
    DbVector3 { x: 0.09, y: 1.295, z: 0.185 },
    DbVector3 { x: -0.29, y: 1.245, z: 0.08 },
    DbVector3 { x: 0.215, y: 1.255, z: 0.075 },
    DbVector3 { x: -0.185, y: 1.335, z: 0.14 },
];

pub static FALLING_BODY_TRIANGLE_INDICES_LOCAL: &[i32] = &[0, 1, 3, 21, 20, 14, 21, 19, 16, 19, 9, 5, 9, 6, 5, 6, 1, 5, 1, 0, 5, 0, 19, 5, 1, 6, 2, 6, 3, 2, 3, 1, 2, 6, 20, 7, 20, 3, 7, 3, 6, 7, 9, 19, 13, 19, 21, 13, 21, 14, 13, 14, 9, 13, 20, 6, 10, 6, 9, 10, 9, 14, 10, 14, 20, 10, 19, 0, 4, 0, 16, 4, 3, 20, 15, 16, 19, 12, 19, 4, 12, 4, 16, 12, 3, 15, 18, 15, 20, 18, 20, 21, 18, 0, 3, 17, 3, 18, 17, 18, 21, 17, 21, 16, 17, 16, 0, 17];

pub static FALLING_HEAD_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.18, y: 1.3, z: 0.14 },
    DbVector3 { x: -0.06, y: 1.28, z: 0.12 },
    DbVector3 { x: 0.06, y: 1.3, z: 0.15 },
    DbVector3 { x: -0.2, y: 1.43, z: 0.18 },
    DbVector3 { x: -0.07, y: 1.41, z: 0.105 },
    DbVector3 { x: 0.08, y: 1.43, z: 0.185 },
    DbVector3 { x: -0.17, y: 1.56, z: 0.25 },
    DbVector3 { x: -0.04, y: 1.54, z: 0.15 },
    DbVector3 { x: 0.07, y: 1.56, z: 0.255 },
    DbVector3 { x: -0.12, y: 1.66, z: 0.26 },
    DbVector3 { x: -0.02, y: 1.675, z: 0.2 },
    DbVector3 { x: 0.04, y: 1.645, z: 0.285 },
    DbVector3 { x: -0.12, y: 1.5, z: 0.36 },
    DbVector3 { x: -0.01, y: 1.52, z: 0.375 },
];

pub static FALLING_HEAD_TRIANGLE_INDICES_LOCAL: &[i32] = &[3, 10, 4, 10, 3, 9, 10, 9, 11, 9, 13, 11, 10, 5, 7, 5, 4, 7, 4, 10, 7, 13, 9, 12, 5, 13, 2, 13, 12, 2, 1, 4, 2, 4, 5, 2, 5, 10, 8, 10, 11, 8, 11, 13, 8, 13, 5, 8, 9, 3, 6, 3, 12, 6, 12, 9, 6, 3, 4, 0, 4, 1, 0, 1, 2, 0, 2, 12, 0, 12, 3, 0];

pub fn MagicianFallingCollider() -> ComplexCollider {
    let falling_leg_hull: ConvexHullCollider = ConvexHullCollider {
        vertices_local: FALLING_LEG_VERTICES.to_vec(),
        triangle_indices_local: FALLING_LEG_TRIANGLE_INDICES_LOCAL.to_vec(),
        margin: 0.0,
        collider_type: ConvexHullColliderType::Leg
    };
    let falling_body_hull: ConvexHullCollider = ConvexHullCollider {
        vertices_local: FALLING_BODY_VERTICES.to_vec(),
        triangle_indices_local: FALLING_BODY_TRIANGLE_INDICES_LOCAL.to_vec(),
        margin: 0.0,
        collider_type: ConvexHullColliderType::Body
    };
    let falling_head_hull: ConvexHullCollider = ConvexHullCollider {
        vertices_local: FALLING_HEAD_VERTICES.to_vec(),
        triangle_indices_local: FALLING_HEAD_TRIANGLE_INDICES_LOCAL.to_vec(),
        margin: 0.0,
        collider_type: ConvexHullColliderType::Head
    };
    let falling_convex_hulls: Vec<ConvexHullCollider> = vec![
        falling_leg_hull,
        falling_body_hull,
        falling_head_hull
    ];
    ComplexCollider { convex_hulls: falling_convex_hulls, center_point: DbVector3 { x: -0.05, y: 1.00, z: -0.03 } }
}
