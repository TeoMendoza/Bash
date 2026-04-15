use std::sync::OnceLock;
use crate::*;

pub static JUMP_LEG_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.255, y: 0.42, z: -0.08 },
    DbVector3 { x: -0.225, y: 0.454999983, z: 0.02 },
    DbVector3 { x: -0.165, y: 0.48999995, z: 0.16 },
    DbVector3 { x: -0.075, y: 0.58, z: 0.235 },
    DbVector3 { x: 0.03, y: 0.469999969, z: -0.3 },
    DbVector3 { x: 0.135, y: 0.525, z: -0.255 },
    DbVector3 { x: 0.225, y: 0.655, z: -0.225 },
    DbVector3 { x: 0.24, y: 0.825, z: -0.245 },
    DbVector3 { x: -0.265, y: 0.724999964, z: 0.125 },
    DbVector3 { x: -0.24, y: 0.86499995, z: 0.205 },
    DbVector3 { x: -0.175, y: 0.965, z: -0.145 },
    DbVector3 { x: -0.09, y: 0.969999969, z: 0.19 },
    DbVector3 { x: -0.05, y: 1.075, z: -0.095 },
    DbVector3 { x: 0.075, y: 1.14999986, z: -0.225 },
    DbVector3 { x: 0.165, y: 1.12499988, z: -0.205 },
    DbVector3 { x: -0.14, y: 1.17, z: 0.06 },
    DbVector3 { x: 0.055, y: 1.20999992, z: 0.065 },
];

pub static JUMP_LEG_TRIANGLE_INDICES_LOCAL: &[i32] = &[10, 8, 9, 7, 4, 13, 4, 10, 13, 3, 16, 11, 9, 3, 11, 10, 9, 15, 9, 11, 15, 11, 16, 15, 16, 13, 15, 13, 10, 15, 7, 16, 6, 16, 3, 6, 8, 10, 0, 10, 4, 0, 16, 7, 14, 7, 13, 14, 13, 16, 14, 4, 7, 5, 7, 6, 5, 6, 3, 5, 3, 9, 2, 9, 8, 2, 0, 4, 2, 4, 5, 2, 5, 3, 2, 8, 0, 1, 0, 2, 1, 2, 8, 1];

pub static JUMP_BODY_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.185, y: 1.065, z: -0.215 },
    DbVector3 { x: 0.085, y: 1.065, z: -0.215 },
    DbVector3 { x: -0.215, y: 1.09499991, z: 0.06 },
    DbVector3 { x: 0.105, y: 1.09499991, z: 0.06 },
    DbVector3 { x: -0.235, y: 1.20999992, z: -0.205 },
    DbVector3 { x: 0.115, y: 1.20999992, z: -0.205 },
    DbVector3 { x: -0.255, y: 1.24999988, z: 0.085 },
    DbVector3 { x: 0.125, y: 1.24999988, z: 0.085 },
    DbVector3 { x: -0.25, y: 1.42, z: -0.175 },
    DbVector3 { x: 0.12, y: 1.42, z: -0.175 },
    DbVector3 { x: -0.235, y: 1.47, z: 0.11 },
    DbVector3 { x: 0.115, y: 1.47, z: 0.11 },
    DbVector3 { x: -0.215, y: 1.59, z: -0.12 },
    DbVector3 { x: 0.105, y: 1.59, z: -0.12 },
    DbVector3 { x: -0.19, y: 1.64999986, z: 0.115 },
    DbVector3 { x: 0.09, y: 1.64999986, z: 0.115 },
];

pub static JUMP_BODY_TRIANGLE_INDICES_LOCAL: &[i32] = &[6, 7, 10, 6, 4, 2, 7, 6, 2, 10, 14, 12, 14, 13, 12, 14, 10, 15, 13, 14, 15, 7, 2, 3, 2, 1, 3, 2, 4, 0, 4, 1, 0, 1, 2, 0, 12, 13, 9, 4, 6, 8, 6, 10, 8, 10, 12, 8, 12, 9, 8, 9, 4, 8, 10, 7, 11, 7, 9, 11, 9, 13, 11, 13, 15, 11, 15, 10, 11, 7, 3, 5, 3, 1, 5, 1, 4, 5, 4, 9, 5, 9, 7, 5];

pub static JUMP_HEAD_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.195, y: 1.625, z: 0.05 },
    DbVector3 { x: 0.075, y: 1.625, z: 0.145 },
    DbVector3 { x: -0.165, y: 1.72, z: 0.085 },
    DbVector3 { x: 0.095, y: 1.72, z: 0.18 },
    DbVector3 { x: -0.2, y: 1.83, z: 0.19 },
    DbVector3 { x: 0.085, y: 1.865, z: 0.195 },
    DbVector3 { x: -0.18, y: 1.915, z: 0.125 },
    DbVector3 { x: 0.105, y: 1.935, z: 0.195 },
    DbVector3 { x: -0.17, y: 1.96, z: -0.01 },
    DbVector3 { x: -0.11, y: 1.99, z: -0.055 },
    DbVector3 { x: -0.025, y: 1.985, z: -0.055 },
    DbVector3 { x: 0.03, y: 1.955, z: -0.005 },
    DbVector3 { x: -0.11, y: 2.01, z: 0.11 },
    DbVector3 { x: 0.005, y: 2.015, z: 0.095 },
    DbVector3 { x: -0.095, y: 2.02299976, z: 0.04 },
    DbVector3 { x: -0.005, y: 2.017, z: 0.02 },
    DbVector3 { x: -0.055, y: 2.02, z: -0.015 },
    DbVector3 { x: -0.015, y: 2.015, z: -0.01 },
    DbVector3 { x: -0.035, y: 2.03299975, z: 0.1 },
];

pub static JUMP_HEAD_TRIANGLE_INDICES_LOCAL: &[i32] = &[4, 1, 3, 1, 4, 0, 4, 7, 12, 7, 3, 11, 3, 1, 11, 7, 4, 5, 4, 3, 5, 3, 7, 5, 9, 0, 8, 0, 4, 8, 1, 0, 10, 0, 9, 10, 11, 1, 10, 4, 12, 6, 12, 8, 6, 8, 4, 6, 10, 9, 16, 9, 8, 14, 8, 12, 14, 16, 9, 14, 7, 11, 15, 11, 10, 17, 10, 16, 17, 15, 11, 17, 16, 14, 18, 14, 12, 18, 12, 7, 18, 15, 17, 18, 17, 16, 18, 7, 15, 13, 15, 18, 13, 18, 7, 13];

pub fn MagicianJumpingCollider() -> ComplexCollider {
    static COLLIDER: OnceLock<ComplexCollider> = OnceLock::new();

    COLLIDER.get_or_init(|| {
        let jump_leg_hull: ConvexHullCollider = create_convex_hull_collider(JUMP_LEG_VERTICES, JUMP_LEG_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::Leg);
        let jump_body_hull: ConvexHullCollider = create_convex_hull_collider(JUMP_BODY_VERTICES, JUMP_BODY_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::Body);
        let jump_head_hull: ConvexHullCollider = create_convex_hull_collider(JUMP_HEAD_VERTICES, JUMP_HEAD_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::Head);
        let jump_convex_hulls: Vec<ConvexHullCollider> = vec![
            jump_leg_hull,
            jump_body_hull,
            jump_head_hull
        ];

        create_complex_collider(jump_convex_hulls, DbVector3 { x: -0.06, y: 1.30, z: -0.03 })
    }).clone()
}
