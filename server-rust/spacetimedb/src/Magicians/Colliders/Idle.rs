use crate::*;

pub static IDLE_LEG_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.08, y: 0.0, z: 0.18 },
    DbVector3 { x: 0.08, y: 0.0, z: 0.18 },
    DbVector3 { x: -0.15, y: 0.0, z: 0.11 },
    DbVector3 { x: 0.15, y: 0.0, z: 0.11 },
    DbVector3 { x: -0.15, y: 0.0, z: 0.0 },
    DbVector3 { x: 0.15, y: 0.0, z: 0.0 },
    DbVector3 { x: -0.08, y: 0.0, z: -0.07 },
    DbVector3 { x: 0.08, y: 0.0, z: -0.07 },
    DbVector3 { x: -0.1, y: 0.18, z: 0.18 },
    DbVector3 { x: 0.1, y: 0.18, z: 0.18 },
    DbVector3 { x: -0.17, y: 0.18, z: 0.11 },
    DbVector3 { x: 0.17, y: 0.18, z: 0.11 },
    DbVector3 { x: -0.17, y: 0.18, z: -0.01 },
    DbVector3 { x: 0.17, y: 0.18, z: -0.01 },
    DbVector3 { x: -0.09, y: 0.18, z: -0.08 },
    DbVector3 { x: 0.09, y: 0.18, z: -0.08 },
    DbVector3 { x: -0.11, y: 0.38, z: 0.15 },
    DbVector3 { x: 0.11, y: 0.38, z: 0.15 },
    DbVector3 { x: -0.18, y: 0.38, z: 0.09 },
    DbVector3 { x: 0.18, y: 0.38, z: 0.09 },
    DbVector3 { x: -0.18, y: 0.38, z: -0.02 },
    DbVector3 { x: 0.18, y: 0.38, z: -0.02 },
    DbVector3 { x: -0.1, y: 0.38, z: -0.07 },
    DbVector3 { x: 0.1, y: 0.38, z: -0.07 },
];

pub static IDLE_LEG_TRIANGLE_INDICES_LOCAL: &[i32] = &[18, 19, 22, 6, 0, 2, 19, 18, 16, 0, 6, 3, 22, 19, 23, 18, 22, 20, 2, 0, 10, 18, 20, 10, 19, 16, 17, 22, 23, 14, 20, 22, 14, 6, 2, 4, 2, 10, 4, 10, 0, 8, 17, 16, 8, 16, 18, 8, 18, 10, 8, 0, 3, 1, 8, 0, 1, 6, 14, 7, 3, 6, 7, 23, 19, 21, 19, 13, 21, 20, 14, 12, 14, 6, 12, 6, 4, 12, 4, 10, 12, 10, 20, 12, 3, 13, 11, 13, 19, 11, 1, 3, 11, 14, 23, 15, 23, 21, 15, 21, 13, 15, 13, 7, 15, 7, 14, 15, 13, 3, 5, 3, 7, 5, 7, 13, 5, 17, 8, 9, 8, 1, 9, 1, 11, 9, 11, 19, 9, 19, 17, 9];

pub static IDLE_BODY_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.11, y: 0.26, z: 0.13 },
    DbVector3 { x: 0.11, y: 0.26, z: 0.13 },
    DbVector3 { x: -0.19, y: 0.26, z: 0.07 },
    DbVector3 { x: 0.19, y: 0.26, z: 0.07 },
    DbVector3 { x: -0.18, y: 0.26, z: -0.03 },
    DbVector3 { x: 0.18, y: 0.26, z: -0.03 },
    DbVector3 { x: -0.1, y: 0.26, z: -0.08 },
    DbVector3 { x: 0.1, y: 0.26, z: -0.08 },
    DbVector3 { x: -0.13, y: 0.82, z: 0.14 },
    DbVector3 { x: 0.13, y: 0.82, z: 0.14 },
    DbVector3 { x: -0.22, y: 0.82, z: 0.07 },
    DbVector3 { x: 0.22, y: 0.82, z: 0.07 },
    DbVector3 { x: -0.21, y: 0.82, z: -0.04 },
    DbVector3 { x: 0.21, y: 0.82, z: -0.04 },
    DbVector3 { x: -0.11, y: 0.82, z: -0.1 },
    DbVector3 { x: 0.11, y: 0.82, z: -0.1 },
    DbVector3 { x: -0.15, y: 1.18, z: 0.14 },
    DbVector3 { x: 0.15, y: 1.18, z: 0.14 },
    DbVector3 { x: -0.24, y: 1.18, z: 0.07 },
    DbVector3 { x: 0.24, y: 1.18, z: 0.07 },
    DbVector3 { x: -0.22, y: 1.18, z: -0.05 },
    DbVector3 { x: 0.22, y: 1.18, z: -0.05 },
    DbVector3 { x: -0.12, y: 1.18, z: -0.1 },
    DbVector3 { x: 0.12, y: 1.18, z: -0.1 },
    DbVector3 { x: -0.11, y: 1.4, z: 0.1 },
    DbVector3 { x: 0.11, y: 1.4, z: 0.1 },
    DbVector3 { x: -0.18, y: 1.4, z: 0.04 },
    DbVector3 { x: 0.18, y: 1.4, z: 0.04 },
    DbVector3 { x: -0.16, y: 1.4, z: -0.06 },
    DbVector3 { x: 0.16, y: 1.4, z: -0.06 },
    DbVector3 { x: -0.08, y: 1.4, z: -0.09 },
    DbVector3 { x: 0.08, y: 1.4, z: -0.09 },
];

pub static IDLE_BODY_TRIANGLE_INDICES_LOCAL: &[i32] = &[6, 0, 2, 18, 2, 16, 0, 6, 3, 3, 19, 17, 20, 18, 26, 30, 26, 29, 19, 3, 13, 3, 6, 7, 6, 15, 7, 15, 13, 7, 6, 2, 4, 2, 0, 8, 17, 16, 8, 16, 2, 8, 0, 3, 1, 8, 0, 1, 20, 26, 28, 26, 30, 28, 30, 22, 28, 22, 20, 28, 15, 22, 23, 22, 30, 23, 16, 17, 25, 17, 19, 25, 29, 26, 25, 19, 13, 21, 13, 15, 21, 15, 23, 21, 23, 29, 21, 13, 3, 5, 3, 7, 5, 7, 13, 5, 15, 6, 14, 20, 22, 14, 22, 15, 14, 2, 18, 12, 18, 20, 12, 20, 14, 12, 14, 6, 12, 6, 4, 12, 4, 2, 12, 3, 17, 9, 17, 8, 9, 8, 1, 9, 1, 3, 9, 30, 29, 31, 29, 23, 31, 23, 30, 31, 29, 25, 27, 25, 19, 27, 19, 21, 27, 21, 29, 27, 18, 16, 24, 16, 25, 24, 25, 26, 24, 26, 18, 24];

pub static IDLE_HEAD_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.06, y: 1.36, z: 0.08 },
    DbVector3 { x: 0.06, y: 1.36, z: 0.08 },
    DbVector3 { x: -0.1, y: 1.36, z: 0.02 },
    DbVector3 { x: 0.1, y: 1.36, z: 0.02 },
    DbVector3 { x: -0.08, y: 1.36, z: -0.09 },
    DbVector3 { x: 0.08, y: 1.36, z: -0.09 },
    DbVector3 { x: -0.03, y: 1.36, z: -0.13 },
    DbVector3 { x: 0.03, y: 1.36, z: -0.13 },
    DbVector3 { x: -0.07, y: 1.6, z: 0.1 },
    DbVector3 { x: 0.07, y: 1.6, z: 0.1 },
    DbVector3 { x: -0.12, y: 1.6, z: 0.03 },
    DbVector3 { x: 0.12, y: 1.6, z: 0.03 },
    DbVector3 { x: -0.09, y: 1.6, z: -0.1 },
    DbVector3 { x: 0.09, y: 1.6, z: -0.1 },
    DbVector3 { x: -0.03, y: 1.6, z: -0.14 },
    DbVector3 { x: 0.03, y: 1.6, z: -0.14 },
    DbVector3 { x: -0.05, y: 1.84, z: 0.09 },
    DbVector3 { x: 0.05, y: 1.84, z: 0.09 },
    DbVector3 { x: -0.09, y: 1.84, z: 0.03 },
    DbVector3 { x: 0.09, y: 1.84, z: 0.03 },
    DbVector3 { x: -0.07, y: 1.84, z: -0.08 },
    DbVector3 { x: 0.07, y: 1.84, z: -0.08 },
    DbVector3 { x: -0.02, y: 1.84, z: -0.11 },
    DbVector3 { x: 0.02, y: 1.84, z: -0.11 },
];

pub static IDLE_HEAD_TRIANGLE_INDICES_LOCAL: &[i32] = &[6, 0, 2, 0, 10, 2, 0, 6, 3, 12, 10, 20, 16, 22, 20, 6, 12, 14, 12, 20, 14, 20, 22, 14, 22, 16, 19, 10, 0, 8, 16, 10, 8, 10, 12, 4, 12, 6, 4, 6, 2, 4, 2, 10, 4, 0, 3, 1, 3, 11, 1, 8, 0, 1, 13, 11, 5, 11, 3, 5, 3, 6, 5, 10, 16, 18, 16, 20, 18, 20, 10, 18, 6, 14, 15, 14, 22, 15, 11, 13, 21, 13, 15, 21, 22, 19, 21, 19, 11, 21, 11, 19, 17, 19, 16, 17, 16, 8, 17, 13, 5, 7, 5, 6, 7, 6, 15, 7, 15, 13, 7, 15, 22, 23, 22, 21, 23, 21, 15, 23, 8, 1, 9, 1, 11, 9, 11, 17, 9, 17, 8, 9];

pub fn MagicianIdleCollider() -> ComplexCollider {
    let idle_leg_hull: ConvexHullCollider = ConvexHullCollider {
        vertices_local: IDLE_LEG_VERTICES.to_vec(),
        triangle_indices_local: IDLE_LEG_TRIANGLE_INDICES_LOCAL.to_vec(),
        margin: 0.0,
        collider_type: ConvexHullColliderType::Leg
    };
    let idle_body_hull: ConvexHullCollider = ConvexHullCollider {
        vertices_local: IDLE_BODY_VERTICES.to_vec(),
        triangle_indices_local: IDLE_BODY_TRIANGLE_INDICES_LOCAL.to_vec(),
        margin: 0.0,
        collider_type: ConvexHullColliderType::Body
    };
    let idle_head_hull: ConvexHullCollider = ConvexHullCollider {
        vertices_local: IDLE_HEAD_VERTICES.to_vec(),
        triangle_indices_local: IDLE_HEAD_TRIANGLE_INDICES_LOCAL.to_vec(),
        margin: 0.0,
        collider_type: ConvexHullColliderType::Head
    };
    let idle_convex_hulls: Vec<ConvexHullCollider> = vec![
        idle_leg_hull,
        idle_body_hull,
        idle_head_hull
    ];
    ComplexCollider { convex_hulls: idle_convex_hulls, center_point: DbVector3 { x: 0.0, y: 0.90, z: 0.03 } }
}
