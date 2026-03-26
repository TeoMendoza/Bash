use crate::*;

pub static CROUCH_LEG_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.255, y: 0.69, z: 0.015 },
    DbVector3 { x: -0.13, y: 0.67, z: -0.06 },
    DbVector3 { x: 0.015, y: 0.675, z: -0.06 },
    DbVector3 { x: 0.145, y: 0.71, z: 0.0 },
    DbVector3 { x: -0.36, y: 0.66, z: 0.09 },
    DbVector3 { x: -0.255, y: 0.61, z: 0.19 },
    DbVector3 { x: -0.13, y: 0.575, z: 0.26 },
    DbVector3 { x: -0.015, y: 0.56, z: 0.28 },
    DbVector3 { x: 0.065, y: 0.61, z: 0.185 },
    DbVector3 { x: 0.17, y: 0.565, z: 0.255 },
    DbVector3 { x: 0.27, y: 0.5, z: 0.275 },
    DbVector3 { x: 0.34, y: 0.43, z: 0.245 },
    DbVector3 { x: -0.41, y: 0.52, z: 0.03 },
    DbVector3 { x: -0.345, y: 0.41, z: 0.015 },
    DbVector3 { x: -0.285, y: 0.255, z: 0.01 },
    DbVector3 { x: -0.25, y: 0.085, z: 0.01 },
    DbVector3 { x: -0.275, y: 0.01, z: 0.145 },
    DbVector3 { x: -0.15, y: 0.0, z: 0.19 },
    DbVector3 { x: -0.025, y: 0.01, z: 0.115 },
    DbVector3 { x: 0.115, y: 0.395, z: 0.055 },
    DbVector3 { x: 0.145, y: 0.28, z: 0.02 },
    DbVector3 { x: 0.125, y: 0.145, z: 0.01 },
    DbVector3 { x: 0.105, y: 0.01, z: 0.0 },
    DbVector3 { x: 0.06, y: 0.01, z: 0.1 },
    DbVector3 { x: 0.165, y: 0.01, z: 0.045 },
    DbVector3 { x: -0.085, y: 0.455, z: 0.305 },
    DbVector3 { x: 0.245, y: 0.455, z: 0.29 },
    DbVector3 { x: -0.335, y: 0.3, z: 0.14 },
    DbVector3 { x: 0.31, y: 0.315, z: 0.165 },
];

pub static CROUCH_LEG_TRIANGLE_INDICES_LOCAL: &[i32] = &[22, 16, 15, 16, 12, 15, 16, 22, 17, 25, 16, 17, 12, 16, 27, 16, 25, 27, 22, 15, 1, 2, 22, 1, 17, 22, 24, 28, 11, 24, 15, 12, 13, 12, 1, 13, 1, 15, 13, 12, 27, 4, 25, 17, 26, 17, 11, 26, 11, 28, 3, 28, 24, 3, 24, 22, 3, 22, 2, 3, 2, 1, 3, 11, 17, 23, 17, 24, 23, 24, 11, 23, 26, 11, 10, 11, 3, 10, 3, 9, 10, 1, 12, 0, 12, 4, 0, 4, 3, 0, 3, 1, 0, 6, 25, 7, 25, 26, 7, 26, 10, 7, 10, 9, 7, 3, 6, 7, 27, 25, 5, 25, 6, 5, 6, 3, 5, 3, 4, 5, 4, 27, 5, 9, 3, 8, 3, 7, 8, 7, 9, 8];

pub static CROUCH_BODY_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.23, y: 0.62, z: -0.035 },
    DbVector3 { x: -0.11, y: 0.6, z: -0.15 },
    DbVector3 { x: 0.02, y: 0.61, z: -0.15 },
    DbVector3 { x: 0.14, y: 0.645, z: -0.055 },
    DbVector3 { x: -0.255, y: 0.76, z: 0.045 },
    DbVector3 { x: -0.125, y: 0.735, z: -0.17 },
    DbVector3 { x: 0.035, y: 0.745, z: -0.17 },
    DbVector3 { x: 0.165, y: 0.79, z: 0.025 },
    DbVector3 { x: -0.27, y: 0.92, z: 0.12 },
    DbVector3 { x: -0.145, y: 0.895, z: -0.135 },
    DbVector3 { x: 0.03, y: 0.905, z: -0.125 },
    DbVector3 { x: 0.165, y: 0.945, z: 0.09 },
    DbVector3 { x: -0.235, y: 0.995, z: 0.14 },
    DbVector3 { x: -0.115, y: 0.985, z: -0.04 },
    DbVector3 { x: 0.015, y: 0.99, z: -0.025 },
    DbVector3 { x: 0.125, y: 1.015, z: 0.115 },
    DbVector3 { x: -0.185, y: 1.06, z: 0.15 },
    DbVector3 { x: -0.075, y: 1.055, z: 0.03 },
    DbVector3 { x: 0.025, y: 1.06, z: 0.035 },
    DbVector3 { x: 0.105, y: 1.075, z: 0.14 },
    DbVector3 { x: -0.18, y: 1.095, z: 0.11 },
    DbVector3 { x: 0.13, y: 1.1, z: 0.11 },
    DbVector3 { x: -0.105, y: 1.12, z: 0.17 },
];

pub static CROUCH_BODY_TRIANGLE_INDICES_LOCAL: &[i32] = &[0, 1, 2, 7, 8, 4, 8, 9, 4, 9, 0, 4, 0, 7, 4, 9, 8, 20, 7, 0, 3, 0, 2, 3, 1, 0, 5, 0, 9, 5, 7, 10, 11, 10, 21, 11, 21, 19, 11, 19, 8, 11, 8, 7, 11, 19, 21, 22, 21, 20, 22, 20, 16, 22, 8, 19, 22, 10, 9, 17, 10, 7, 6, 7, 3, 6, 3, 2, 6, 2, 1, 6, 1, 5, 6, 5, 9, 6, 9, 10, 6, 20, 8, 12, 8, 22, 12, 22, 16, 12, 16, 20, 12, 9, 20, 13, 20, 17, 13, 17, 9, 13, 20, 21, 18, 21, 10, 18, 10, 17, 18, 17, 20, 18];

pub static CROUCH_HEAD_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.145, y: 1.105, z: 0.1 },
    DbVector3 { x: -0.055, y: 1.095, z: 0.06 },
    DbVector3 { x: 0.035, y: 1.105, z: 0.095 },
    DbVector3 { x: -0.16, y: 1.17, z: 0.175 },
    DbVector3 { x: -0.06, y: 1.165, z: 0.03 },
    DbVector3 { x: 0.04, y: 1.17, z: 0.145 },
    DbVector3 { x: -0.155, y: 1.255, z: 0.22 },
    DbVector3 { x: -0.05, y: 1.245, z: 0.05 },
    DbVector3 { x: 0.035, y: 1.25, z: 0.17 },
    DbVector3 { x: -0.135, y: 1.345, z: 0.21 },
    DbVector3 { x: -0.04, y: 1.355, z: 0.085 },
    DbVector3 { x: 0.02, y: 1.335, z: 0.15 },
    DbVector3 { x: -0.11, y: 1.395, z: 0.17 },
    DbVector3 { x: -0.03, y: 1.405, z: 0.115 },
    DbVector3 { x: -0.17, y: 1.215, z: 0.25 },
    DbVector3 { x: -0.12, y: 1.135, z: 0.27 },
    DbVector3 { x: -0.08, y: 1.085, z: 0.24 },
];

pub static CROUCH_HEAD_TRIANGLE_INDICES_LOCAL: &[i32] = &[5, 8, 16, 4, 12, 7, 6, 14, 9, 8, 5, 11, 13, 12, 11, 12, 9, 11, 12, 4, 0, 5, 16, 2, 4, 7, 2, 11, 5, 2, 16, 8, 15, 8, 11, 15, 11, 9, 15, 9, 14, 15, 14, 0, 15, 0, 16, 15, 12, 13, 10, 13, 11, 10, 11, 2, 10, 2, 7, 10, 7, 12, 10, 14, 6, 3, 6, 9, 3, 9, 12, 3, 12, 0, 3, 0, 14, 3, 16, 0, 1, 0, 4, 1, 4, 2, 1, 2, 16, 1];

pub fn MagicianCrouchCollider() -> ComplexCollider {
    let crouch_leg_hull: ConvexHullCollider = ConvexHullCollider {
        vertices_local: CROUCH_LEG_VERTICES.to_vec(),
        triangle_indices_local: CROUCH_LEG_TRIANGLE_INDICES_LOCAL.to_vec(),
        margin: 0.0,
        collider_type: ConvexHullColliderType::Leg
    };
    let crouch_body_hull: ConvexHullCollider = ConvexHullCollider {
        vertices_local: CROUCH_BODY_VERTICES.to_vec(),
        triangle_indices_local: CROUCH_BODY_TRIANGLE_INDICES_LOCAL.to_vec(),
        margin: 0.0,
        collider_type: ConvexHullColliderType::Body
    };
    let crouch_head_hull: ConvexHullCollider = ConvexHullCollider {
        vertices_local: CROUCH_HEAD_VERTICES.to_vec(),
        triangle_indices_local: CROUCH_HEAD_TRIANGLE_INDICES_LOCAL.to_vec(),
        margin: 0.0,
        collider_type: ConvexHullColliderType::Head
    };
    let crouch_convex_hulls: Vec<ConvexHullCollider> = vec![
        crouch_leg_hull,
        crouch_body_hull,
        crouch_head_hull
    ];
    ComplexCollider { convex_hulls: crouch_convex_hulls, center_point: DbVector3 { x: -0.04, y: 0.84, z: 0.02 } }
}
