use crate::*;

pub static IDLE_LEG_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.315, y: 0.0, z: 0.285 },
    DbVector3 { x: 0.26, y: 0.0, z: 0.19 },
    DbVector3 { x: -0.235, y: 0.0, z: -0.015 },
    DbVector3 { x: 0.165, y: 0.0, z: -0.095 },
    DbVector3 { x: -0.225, y: 0.34, z: 0.22 },
    DbVector3 { x: 0.19, y: 0.34, z: 0.18 },
    DbVector3 { x: -0.21, y: 0.34, z: 0.015 },
    DbVector3 { x: 0.18, y: 0.34, z: -0.02 },
];

pub static IDLE_LEG_TRIANGLE_INDICES_LOCAL: &[i32] = &[0, 3, 1, 6, 0, 4, 0, 6, 2, 6, 3, 2, 3, 0, 2, 1, 3, 7, 3, 6, 7, 6, 4, 7, 0, 1, 5, 1, 7, 5, 7, 4, 5, 4, 0, 5];

pub static IDLE_BODY_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.21, y: 0.34, z: 0.16 },
    DbVector3 { x: 0.17, y: 0.34, z: 0.14 },
    DbVector3 { x: -0.185, y: 0.34, z: -0.06 },
    DbVector3 { x: 0.145, y: 0.34, z: -0.1 },
    DbVector3 { x: -0.35, y: 1.12, z: 0.03 },
    DbVector3 { x: 0.35, y: 1.12, z: 0.02 },
    DbVector3 { x: -0.285, y: 1.12, z: -0.09 },
    DbVector3 { x: 0.285, y: 1.12, z: -0.085 },
    DbVector3 { x: 0.022, y: 1.42, z: 0.09 },
    DbVector3 { x: 0.022, y: 1.42, z: -0.165 },
];

pub static IDLE_BODY_TRIANGLE_INDICES_LOCAL: &[i32] = &[4, 0, 8, 3, 0, 2, 0, 4, 2, 3, 2, 9, 4, 8, 9, 8, 5, 9, 0, 3, 1, 3, 5, 1, 5, 8, 1, 8, 0, 1, 5, 3, 7, 3, 9, 7, 9, 5, 7, 2, 4, 6, 4, 9, 6, 9, 2, 6];

pub static IDLE_HEAD_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.09, y: 1.62, z: 0.02 },
    DbVector3 { x: 0.12, y: 1.62, z: 0.02 },
    DbVector3 { x: -0.055, y: 1.62, z: -0.14 },
    DbVector3 { x: 0.085, y: 1.62, z: -0.135 },
    DbVector3 { x: -0.04, y: 1.84, z: 0.09 },
    DbVector3 { x: 0.08, y: 1.84, z: 0.09 },
    DbVector3 { x: -0.015, y: 1.84, z: -0.105 },
    DbVector3 { x: 0.065, y: 1.84, z: -0.105 },
];

pub static IDLE_HEAD_TRIANGLE_INDICES_LOCAL: &[i32] = &[0, 4, 6, 0, 1, 4, 0, 6, 2, 1, 0, 2, 6, 4, 5, 4, 1, 5, 1, 2, 3, 2, 6, 7, 6, 5, 7, 5, 1, 7, 1, 3, 7, 3, 2, 7];

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
    ComplexCollider { convex_hulls: idle_convex_hulls, center_point: DbVector3 { x: 0.0, y: 0.93, z: 0.03 } }
}
