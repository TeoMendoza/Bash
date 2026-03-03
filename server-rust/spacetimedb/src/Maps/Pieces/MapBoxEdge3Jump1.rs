use crate::*;

pub static MAP_BOX_EDGE_3_JUMP_1_CONVEX_HULL0_VERTICES: &[DbVector3] = &[
    DbVector3 { x: -19.5, y: 0.0, z: -12.0 },
    DbVector3 { x: -18.5, y: 0.0, z: -12.0 },
    DbVector3 { x: -19.5, y: 1.0, z: -12.0 },
    DbVector3 { x: -18.5, y: 1.0, z: -12.0 },
    DbVector3 { x: -18.5, y: 0.0, z: -13.0 },
    DbVector3 { x: -18.5, y: 1.0, z: -13.0 },
    DbVector3 { x: -19.5, y: 0.0, z: -13.0 },
    DbVector3 { x: -19.5, y: 1.0, z: -13.0 },
];

pub static MAP_BOX_EDGE_3_JUMP_1_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL: &[i32] = &[
    0,
    1,
    2,
    5,
    1,
    4,
    1,
    0,
    4,
    0,
    2,
    6,
    5,
    4,
    6,
    4,
    0,
    6,
    1,
    5,
    3,
    5,
    2,
    3,
    2,
    1,
    3,
    2,
    5,
    7,
    5,
    6,
    7,
    6,
    2,
    7,
];

pub fn map_box_edge_3_jump_1_collider() -> ComplexCollider {
    let map_box_edge_3_jump_1_convex_hull_0: ConvexHullCollider = ConvexHullCollider { vertices_local: MAP_BOX_EDGE_3_JUMP_1_CONVEX_HULL0_VERTICES.to_vec(), triangle_indices_local: MAP_BOX_EDGE_3_JUMP_1_CONVEX_HULL0_TRIANGLE_INDICES_LOCAL.to_vec(), margin: 0.0 };
    let plane_convex_hulls: Vec<ConvexHullCollider> = vec![map_box_edge_3_jump_1_convex_hull_0];
    ComplexCollider { convex_hulls: plane_convex_hulls, center_point: DbVector3 { x: -19.0, y: 0.5, z: -12.5 } }
}

