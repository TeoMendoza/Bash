use std::sync::OnceLock;
use crate::*;

pub static IDLE_LEG_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.08, y: 0.01, z: 0.18 },
    DbVector3 { x: 0.08, y: 0.01, z: 0.18 },
    DbVector3 { x: -0.15, y: 0.01, z: 0.11 },
    DbVector3 { x: 0.15, y: 0.01, z: 0.11 },
    DbVector3 { x: -0.15, y: 0.0, z: 0.0 },
    DbVector3 { x: 0.15, y: 0.01, z: 0.0 },
    DbVector3 { x: -0.08, y: 0.01, z: -0.07 },
    DbVector3 { x: 0.08, y: 0.01, z: -0.07 },
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
    DbVector3 { x: -0.11, y: 0.72, z: 0.13 },
    DbVector3 { x: 0.11, y: 0.72, z: 0.13 },
    DbVector3 { x: -0.19, y: 0.72, z: 0.07 },
    DbVector3 { x: 0.19, y: 0.72, z: 0.07 },
    DbVector3 { x: -0.18, y: 0.72, z: -0.03 },
    DbVector3 { x: 0.18, y: 0.72, z: -0.03 },
    DbVector3 { x: -0.1, y: 0.72, z: -0.08 },
    DbVector3 { x: 0.1, y: 0.72, z: -0.08 },
    DbVector3 { x: -0.12, y: 0.96, z: 0.12 },
    DbVector3 { x: 0.12, y: 0.96, z: 0.12 },
    DbVector3 { x: -0.2, y: 0.96, z: 0.06 },
    DbVector3 { x: 0.2, y: 0.96, z: 0.06 },
    DbVector3 { x: -0.19, y: 0.96, z: -0.04 },
    DbVector3 { x: 0.19, y: 0.96, z: -0.04 },
    DbVector3 { x: -0.1, y: 0.96, z: -0.09 },
    DbVector3 { x: 0.1, y: 0.96, z: -0.09 },
];

pub static IDLE_LEG_TRIANGLE_INDICES_LOCAL: &[i32] = &[34, 35, 38, 35, 34, 32, 34, 38, 36, 4, 0, 2, 0, 10, 2, 10, 4, 2, 35, 32, 33, 34, 10, 8, 10, 0, 8, 33, 32, 8, 32, 34, 8, 8, 0, 1, 0, 4, 1, 5, 4, 7, 34, 36, 20, 7, 4, 6, 38, 35, 39, 4, 10, 12, 10, 20, 12, 6, 4, 12, 10, 34, 18, 34, 20, 18, 20, 10, 18, 35, 11, 19, 33, 8, 9, 8, 1, 9, 1, 11, 9, 11, 35, 9, 35, 33, 9, 4, 5, 3, 5, 11, 3, 11, 1, 3, 1, 4, 3, 7, 6, 14, 6, 12, 14, 12, 20, 14, 20, 36, 14, 36, 38, 14, 38, 39, 14, 39, 35, 37, 37, 35, 21, 35, 19, 21, 19, 11, 21, 7, 14, 15, 14, 39, 15, 39, 37, 15, 37, 21, 15, 11, 5, 13, 5, 7, 13, 7, 15, 13, 15, 21, 13, 21, 11, 13];

pub static IDLE_BODY_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.13, y: 0.92, z: 0.14 },
    DbVector3 { x: 0.13, y: 0.92, z: 0.14 },
    DbVector3 { x: -0.22, y: 0.92, z: 0.07 },
    DbVector3 { x: 0.22, y: 0.92, z: 0.07 },
    DbVector3 { x: -0.21, y: 0.92, z: -0.04 },
    DbVector3 { x: 0.21, y: 0.92, z: -0.04 },
    DbVector3 { x: -0.11, y: 0.92, z: -0.1 },
    DbVector3 { x: 0.11, y: 0.92, z: -0.1 },
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

pub static IDLE_BODY_TRIANGLE_INDICES_LOCAL: &[i32] = &[6, 0, 2, 0, 10, 2, 0, 6, 3, 16, 22, 20, 6, 12, 14, 12, 20, 14, 20, 22, 14, 22, 15, 14, 15, 6, 14, 22, 16, 19, 10, 0, 8, 16, 10, 8, 10, 12, 4, 12, 6, 4, 6, 2, 4, 2, 10, 4, 0, 3, 1, 3, 11, 1, 8, 0, 1, 6, 15, 7, 3, 6, 7, 12, 10, 18, 10, 16, 18, 16, 20, 18, 20, 12, 18, 11, 19, 17, 19, 16, 17, 16, 8, 17, 22, 19, 21, 11, 3, 5, 3, 7, 5, 8, 1, 9, 1, 11, 9, 11, 17, 9, 17, 8, 9, 15, 22, 23, 22, 21, 23, 21, 15, 23, 19, 11, 13, 11, 5, 13, 5, 7, 13, 7, 15, 13, 15, 21, 13, 21, 19, 13];

pub static IDLE_HEAD_VERTICES: &[DbVector3] = &[
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

pub static IDLE_HEAD_TRIANGLE_INDICES_LOCAL: &[i32] = &[0, 4, 1, 10, 8, 18, 14, 20, 18, 4, 10, 12, 10, 18, 12, 18, 20, 12, 4, 0, 2, 0, 8, 2, 8, 10, 2, 10, 4, 2, 20, 14, 17, 11, 9, 3, 9, 1, 3, 1, 4, 3, 1, 9, 7, 0, 1, 7, 8, 14, 16, 14, 18, 16, 18, 8, 16, 4, 12, 13, 12, 20, 13, 9, 11, 19, 11, 13, 19, 20, 17, 19, 17, 9, 19, 9, 17, 15, 17, 14, 15, 14, 7, 15, 7, 9, 15, 8, 0, 6, 0, 7, 6, 7, 14, 6, 14, 8, 6, 11, 3, 5, 3, 4, 5, 4, 13, 5, 13, 11, 5, 13, 20, 21, 20, 19, 21, 19, 13, 21];

pub fn MagicianIdleCollider() -> ComplexCollider {
    static COLLIDER: OnceLock<ComplexCollider> = OnceLock::new();

    COLLIDER.get_or_init(|| {
        let idle_leg_hull: ConvexHullCollider = create_convex_hull_collider(IDLE_LEG_VERTICES, IDLE_LEG_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::Leg);
        let idle_body_hull: ConvexHullCollider = create_convex_hull_collider(IDLE_BODY_VERTICES, IDLE_BODY_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::Body);
        let idle_head_hull: ConvexHullCollider = create_convex_hull_collider(IDLE_HEAD_VERTICES, IDLE_HEAD_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::Head);
        let idle_convex_hulls: Vec<ConvexHullCollider> = vec![
            idle_leg_hull,
            idle_body_hull,
            idle_head_hull
        ];

        create_complex_collider(idle_convex_hulls, DbVector3 { x: 0.0, y: 0.90, z: 0.03 })
    }).clone()
}
