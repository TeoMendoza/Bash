use crate::*;

pub static BOX_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    1, 6, 2,
    1, 2, 0,
    1, 0, 4,
    0, 2, 3,
    2, 6, 3,
    4, 0, 3,
    6, 1, 5,
    1, 4, 5,
    4, 6, 5,
    6, 4, 7,
    4, 3, 7,
    3, 6, 7,
];

pub static NORTH_WALL_VERTICES: &[DbVector3] = &[
    DbVector3 { x: 39.0, y: -5.0, z: 37.5 },
    DbVector3 { x: -39.0, y: -5.0, z: 37.5 },
    DbVector3 { x: -39.0, y: -5.0, z: 40.5 },
    DbVector3 { x: 39.0, y: -5.0, z: 40.5 },
    DbVector3 { x: 39.0, y: 30.0, z: 37.5 },
    DbVector3 { x: -39.0, y: 30.0, z: 37.5 },
    DbVector3 { x: -39.0, y: 30.0, z: 40.5 },
    DbVector3 { x: 39.0, y: 30.0, z: 40.5 },
];

pub static SOUTH_WALL_VERTICES: &[DbVector3] = &[
    DbVector3 { x: 39.0, y: -5.0, z: -40.5 },
    DbVector3 { x: -39.0, y: -5.0, z: -40.5 },
    DbVector3 { x: -39.0, y: -5.0, z: -37.5 },
    DbVector3 { x: 39.0, y: -5.0, z: -37.5 },
    DbVector3 { x: 39.0, y: 30.0, z: -40.5 },
    DbVector3 { x: -39.0, y: 30.0, z: -40.5 },
    DbVector3 { x: -39.0, y: 30.0, z: -37.5 },
    DbVector3 { x: 39.0, y: 30.0, z: -37.5 },
];

pub static EAST_WALL_VERTICES: &[DbVector3] = &[
    DbVector3 { x: 37.5, y: -5.0, z: 39.0 },
    DbVector3 { x: 37.5, y: -5.0, z: -39.0 },
    DbVector3 { x: 40.5, y: -5.0, z: -39.0 },
    DbVector3 { x: 40.5, y: -5.0, z: 39.0 },
    DbVector3 { x: 37.5, y: 30.0, z: 39.0 },
    DbVector3 { x: 37.5, y: 30.0, z: -39.0 },
    DbVector3 { x: 40.5, y: 30.0, z: -39.0 },
    DbVector3 { x: 40.5, y: 30.0, z: 39.0 },
];

pub static WEST_WALL_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -40.5, y: -5.0, z: 39.0 },
    DbVector3 { x: -40.5, y: -5.0, z: -39.0 },
    DbVector3 { x: -37.5, y: -5.0, z: -39.0 },
    DbVector3 { x: -37.5, y: -5.0, z: 39.0 },
    DbVector3 { x: -40.5, y: 30.0, z: 39.0 },
    DbVector3 { x: -40.5, y: 30.0, z: -39.0 },
    DbVector3 { x: -37.5, y: 30.0, z: -39.0 },
    DbVector3 { x: -37.5, y: 30.0, z: 39.0 },
];

pub fn north_wall_collider() -> ComplexCollider {
    let wall_convex_hull: ConvexHullCollider = create_convex_hull_collider(NORTH_WALL_VERTICES, BOX_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::None);
    create_complex_collider(vec![wall_convex_hull], DbVector3 { x: 0.0, y: 12.5, z: 39.0 })
}

pub fn south_wall_collider() -> ComplexCollider {
    let wall_convex_hull: ConvexHullCollider = create_convex_hull_collider(SOUTH_WALL_VERTICES, BOX_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::None);
    create_complex_collider(vec![wall_convex_hull], DbVector3 { x: 0.0, y: 12.5, z: -39.0 })
}

pub fn east_wall_collider() -> ComplexCollider {
    let wall_convex_hull: ConvexHullCollider = create_convex_hull_collider(EAST_WALL_VERTICES, BOX_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::None);
    create_complex_collider(vec![wall_convex_hull], DbVector3 { x: 39.0, y: 12.5, z: 0.0 })
}

pub fn west_wall_collider() -> ComplexCollider {
    let wall_convex_hull: ConvexHullCollider = create_convex_hull_collider(WEST_WALL_VERTICES, BOX_TRIANGLE_INDICES_LOCAL, 0.0, ConvexHullColliderType::None);
    create_complex_collider(vec![wall_convex_hull], DbVector3 { x: -39.0, y: 12.5, z: 0.0 })
}
