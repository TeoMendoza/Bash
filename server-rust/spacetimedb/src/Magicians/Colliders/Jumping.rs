use crate::*;

pub static IDLE_LEG_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.255, y: 0.0, z: -0.08 },
    DbVector3 { x: -0.225, y: 0.0349999964, z: 0.02 },
    DbVector3 { x: -0.165, y: 0.06999996, z: 0.16 },
    DbVector3 { x: -0.075, y: 0.16, z: 0.235 },
    DbVector3 { x: 0.03, y: 0.0499999821, z: -0.3 },
    DbVector3 { x: 0.135, y: 0.104999989, z: -0.255 },
    DbVector3 { x: 0.225, y: 0.234999985, z: -0.225 },
    DbVector3 { x: 0.24, y: 0.405, z: -0.245 },
    DbVector3 { x: -0.265, y: 0.304999977, z: 0.125 },
    DbVector3 { x: -0.24, y: 0.444999963, z: 0.205 },
    DbVector3 { x: -0.175, y: 0.544999957, z: -0.145 },
    DbVector3 { x: -0.09, y: 0.549999952, z: 0.19 },
    DbVector3 { x: -0.05, y: 0.655, z: -0.095 },
    DbVector3 { x: 0.075, y: 0.7299999, z: -0.225 },
    DbVector3 { x: 0.165, y: 0.7049999, z: -0.205 },
    DbVector3 { x: -0.14, y: 0.75, z: 0.06 },
    DbVector3 { x: 0.055, y: 0.789999962, z: 0.065 },
];

pub static IDLE_LEG_TRIANGLE_INDICES_LOCAL: &[i32] = &[10, 8, 9, 7, 4, 13, 4, 10, 13, 3, 16, 11, 9, 3, 11, 10, 9, 15, 9, 11, 15, 11, 16, 15, 16, 13, 15, 13, 10, 15, 7, 16, 6, 16, 3, 6, 8, 10, 0, 10, 4, 0, 16, 7, 14, 7, 13, 14, 13, 16, 14, 4, 7, 5, 7, 6, 5, 6, 3, 5, 3, 9, 2, 9, 8, 2, 0, 4, 2, 4, 5, 2, 5, 3, 2, 8, 0, 1, 0, 2, 1, 2, 8, 1];

pub static IDLE_BODY_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.185, y: 0.645, z: -0.215 },
    DbVector3 { x: 0.085, y: 0.645, z: -0.215 },
    DbVector3 { x: -0.215, y: 0.674999952, z: 0.06 },
    DbVector3 { x: 0.105, y: 0.674999952, z: 0.06 },
    DbVector3 { x: -0.235, y: 0.789999962, z: -0.205 },
    DbVector3 { x: 0.115, y: 0.789999962, z: -0.205 },
    DbVector3 { x: -0.255, y: 0.8299999, z: 0.085 },
    DbVector3 { x: 0.125, y: 0.8299999, z: 0.085 },
    DbVector3 { x: -0.25, y: 1.0, z: -0.175 },
    DbVector3 { x: 0.12, y: 1.0, z: -0.175 },
    DbVector3 { x: -0.235, y: 1.05, z: 0.11 },
    DbVector3 { x: 0.115, y: 1.05, z: 0.11 },
    DbVector3 { x: -0.215, y: 1.17, z: -0.12 },
    DbVector3 { x: 0.105, y: 1.17, z: -0.12 },
    DbVector3 { x: -0.19, y: 1.2299999, z: 0.115 },
    DbVector3 { x: 0.09, y: 1.2299999, z: 0.115 },
];

pub static IDLE_BODY_TRIANGLE_INDICES_LOCAL: &[i32] = &[6, 7, 10, 6, 4, 2, 7, 6, 2, 10, 14, 12, 14, 13, 12, 14, 10, 15, 13, 14, 15, 7, 2, 3, 2, 1, 3, 2, 4, 0, 4, 1, 0, 1, 2, 0, 12, 13, 9, 4, 6, 8, 6, 10, 8, 10, 12, 8, 12, 9, 8, 9, 4, 8, 10, 7, 11, 7, 9, 11, 9, 13, 11, 13, 15, 11, 15, 10, 11, 7, 3, 5, 3, 1, 5, 1, 4, 5, 4, 9, 5, 9, 7, 5];

pub static IDLE_HEAD_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -0.195, y: 1.155, z: 0.095 },
    DbVector3 { x: 0.075, y: 1.155, z: -0.005 },
    DbVector3 { x: -0.165, y: 1.25, z: 0.04 },
    DbVector3 { x: 0.095, y: 1.25, z: -0.05 },
    DbVector3 { x: -0.2, y: 1.36, z: -0.05 },
    DbVector3 { x: 0.085, y: 1.395, z: -0.05 },
    DbVector3 { x: -0.18, y: 1.44499993, z: 0.015 },
    DbVector3 { x: 0.105, y: 1.46499991, z: -0.05 },
    DbVector3 { x: -0.17, y: 1.49, z: 0.155 },
    DbVector3 { x: -0.11, y: 1.52, z: 0.195 },
    DbVector3 { x: -0.025, y: 1.515, z: 0.195 },
    DbVector3 { x: 0.03, y: 1.485, z: 0.15 },
    DbVector3 { x: -0.11, y: 1.54, z: 0.03 },
    DbVector3 { x: 0.005, y: 1.545, z: 0.05 },
    DbVector3 { x: -0.095, y: 1.55299985, z: 0.11 },
    DbVector3 { x: -0.005, y: 1.547, z: 0.125 },
    DbVector3 { x: -0.055, y: 1.55000007, z: 0.155 },
    DbVector3 { x: -0.015, y: 1.545, z: 0.15 },
    DbVector3 { x: -0.035, y: 1.56299984, z: 0.045 },
];

pub static IDLE_HEAD_TRIANGLE_INDICES_LOCAL: &[i32] = &[4, 7, 3, 7, 1, 3, 1, 4, 3, 4, 1, 0, 7, 4, 12, 1, 7, 11, 4, 0, 8, 0, 9, 8, 7, 12, 18, 9, 0, 10, 0, 1, 10, 1, 11, 10, 12, 4, 6, 4, 8, 6, 8, 12, 6, 12, 8, 14, 8, 9, 14, 18, 12, 14, 7, 18, 13, 9, 10, 16, 18, 14, 16, 14, 9, 16, 11, 7, 15, 7, 13, 15, 13, 18, 15, 18, 16, 17, 16, 10, 17, 10, 11, 17, 11, 15, 17, 15, 18, 17];

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
    ComplexCollider { convex_hulls: idle_convex_hulls, center_point: DbVector3 { x: -0.195, y: 1.155, z: 0.095 } }
}
