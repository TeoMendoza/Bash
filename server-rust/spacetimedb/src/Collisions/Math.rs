use glam::{Quat, Vec3};
use spacetimedb::ReducerContext;
use crate::*;

pub fn add(x: DbVector3, y: DbVector3) -> DbVector3 { DbVector3 { x: x.x + y.x, y: x.y + y.y, z: x.z + y.z } }

pub fn sub(x: DbVector3, y: DbVector3) -> DbVector3 { DbVector3 { x: x.x - y.x, y: x.y - y.y, z: x.z - y.z } }

pub fn mul(x: DbVector3, s: f32) -> DbVector3 { DbVector3 { x: x.x * s, y: x.y * s, z: x.z * s } }

pub fn dot(x: DbVector3, y: DbVector3) -> f32 { x.x * y.x + x.y * y.y + x.z * y.z }

pub fn length_sq(x: DbVector3) -> f32 { dot(x, x) }

pub fn magnitude(x: DbVector3) -> f32 { dot(x, x).sqrt() }

pub fn distance_sq(a: DbVector3, b: DbVector3) -> f32 { length_sq(sub(a, b)) }

pub fn clamp_01(t: f32) -> f32 { if t < 0.0 { 0.0 } else if t > 1.0 { 1.0 } else { t } }

pub fn clamp(x: f32, a: f32, b: f32) -> f32 { if x < a { a } else if x > b { b } else { x } }

pub fn to_radians(degrees: f32) -> f32 { degrees * (std::f32::consts::PI / 180.0) }

pub fn cross(a: DbVector3, b: DbVector3) -> DbVector3 { DbVector3 { x: a.y * b.z - a.z * b.y, y: a.z * b.x - a.x * b.z, z: a.x * b.y - a.y * b.x } }

pub fn to_vec3(v: DbVector3) -> Vec3 { Vec3::new(v.x, v.y, v.z) }

pub fn to_dbvec3(v: Vec3) -> DbVector3 { DbVector3 { x: v.x, y: v.y, z: v.z } }

pub fn rotate(v: DbVector3, q: Quat) -> DbVector3 { to_dbvec3(q * to_vec3(v)) }

pub fn negate(vector: DbVector3) -> DbVector3 { DbVector3 { x: -vector.x, y: -vector.y, z: -vector.z } }

pub fn triple_cross(vector_a: DbVector3, vector_b: DbVector3, vector_c: DbVector3) -> DbVector3 { cross(cross(vector_a, vector_b), vector_c) }

pub fn near_zero(vector: DbVector3) -> bool { length_sq(vector) <= 1e-12 }

pub fn normalize_small_vector(v: DbVector3, fallback: DbVector3) -> DbVector3 {
    let mag_sq: f32 = length_sq(v);
    if mag_sq <= 1e-12 { return fallback; }
    let inv_mag: f32 = 1.0 / mag_sq.sqrt();
    DbVector3 { x: v.x * inv_mag, y: v.y * inv_mag, z: v.z * inv_mag }
}

pub fn any_perpendicular_unit(unit_axis: DbVector3) -> DbVector3 {
    let ref_vec = if unit_axis.y.abs() < 0.99 { DbVector3 { x: 0.0, y: 1.0, z: 0.0 } } else { DbVector3 { x: 1.0, y: 0.0, z: 0.0 } };
    let perp = cross(unit_axis, ref_vec);
    normalize_small_vector(perp, DbVector3 { x: 1.0, y: 0.0, z: 0.0 })
}

pub fn normalize(v: DbVector3) -> DbVector3 {
    let mag_sq: f32 = length_sq(v);
    if mag_sq <= 1e-12 { return DbVector3 { x: 0.0, y: 0.0, z: 0.0 }; }
    let inv_mag: f32 = 1.0 / mag_sq.sqrt();
    DbVector3 { x: v.x * inv_mag, y: v.y * inv_mag, z: v.z * inv_mag }
}

pub fn perpendicular(vector: DbVector3) -> DbVector3 {
    if vector.x.abs() > vector.z.abs() { return DbVector3 { x: -vector.y, y: vector.x, z: 0.0 }; }
    DbVector3 { x: 0.0, y: -vector.z, z: vector.y }
}

pub fn rotate_around_y_axis(vector: DbVector3, yaw_radians: f32) -> DbVector3 {
    let cos_yaw: f32 = yaw_radians.cos();
    let sin_yaw: f32 = yaw_radians.sin();

    let rotated_x: f32 = vector.x * cos_yaw + vector.z * sin_yaw;
    let rotated_z: f32 = -vector.x * sin_yaw + vector.z * cos_yaw;

    DbVector3 { x: rotated_x, y: vector.y, z: rotated_z }
}

pub fn get_collider_center_world(collider: &ComplexCollider, position: DbVector3, yaw_radians: f32) -> DbVector3 {
    let rotated_center = rotate_around_y_axis(collider.center_point, yaw_radians);
    add(position, rotated_center)
}

pub fn create_convex_hull_collider(vertices_local: &[DbVector3], triangle_indices_local: &[i32], margin: f32, collider_type: ConvexHullColliderType) -> ConvexHullCollider { // Builds a convex hull collider and caches its local AABB for fast raycast pruning
    let vertices_local_vec: Vec<DbVector3> = vertices_local.to_vec();
    let triangle_indices_local_vec: Vec<i32> = triangle_indices_local.to_vec();

    let mut aabb_min_local: DbVector3 = DbVector3 { x: 0.0, y: 0.0, z: 0.0 };
    let mut aabb_max_local: DbVector3 = DbVector3 { x: 0.0, y: 0.0, z: 0.0 };

    if vertices_local_vec.is_empty() == false {
        aabb_min_local = vertices_local_vec[0];
        aabb_max_local = vertices_local_vec[0];

        for vertex in vertices_local_vec.iter().skip(1) {
            if vertex.x < aabb_min_local.x { aabb_min_local.x = vertex.x; }
            if vertex.y < aabb_min_local.y { aabb_min_local.y = vertex.y; }
            if vertex.z < aabb_min_local.z { aabb_min_local.z = vertex.z; }

            if vertex.x > aabb_max_local.x { aabb_max_local.x = vertex.x; }
            if vertex.y > aabb_max_local.y { aabb_max_local.y = vertex.y; }
            if vertex.z > aabb_max_local.z { aabb_max_local.z = vertex.z; }
        }

        aabb_min_local.x -= margin;
        aabb_min_local.y -= margin;
        aabb_min_local.z -= margin;
        aabb_max_local.x += margin;
        aabb_max_local.y += margin;
        aabb_max_local.z += margin;
    }

    ConvexHullCollider { vertices_local: vertices_local_vec, triangle_indices_local: triangle_indices_local_vec, margin, collider_type, aabb_min_local, aabb_max_local }
}

pub fn create_complex_collider(convex_hulls: Vec<ConvexHullCollider>, center_point: DbVector3) -> ComplexCollider { // Builds a complex collider and caches a broadphase radius around its center
    let mut bounding_radius: f32 = 0.0;

    for hull in convex_hulls.iter() {
        for vertex in hull.vertices_local.iter() {
            let radius = magnitude(sub(*vertex, center_point)) + hull.margin;
            if radius > bounding_radius {
                bounding_radius = radius;
            }
        }
    }

    ComplexCollider { convex_hulls, center_point, bounding_radius }
}

pub fn get_complex_collider_bounding_radius(collider: &ComplexCollider) -> f32 { // Used for cheap broadphase pruning before expensive narrow-phase checks
    collider.bounding_radius
}

pub fn broadphase_radius_overlap(collider_a: &ComplexCollider, position_a: DbVector3, yaw_radians_a: f32, collider_b: &ComplexCollider, position_b: DbVector3, yaw_radians_b: f32, padding: f32) -> bool {
    let center_a = get_collider_center_world(collider_a, position_a, yaw_radians_a);
    let center_b = get_collider_center_world(collider_b, position_b, yaw_radians_b);

    let radius_a = get_complex_collider_bounding_radius(collider_a);
    let radius_b = get_complex_collider_bounding_radius(collider_b);
    let combined_radius = radius_a + radius_b + padding;

    distance_sq(center_a, center_b) <= combined_radius * combined_radius
}

pub fn distance_point_to_segment_sq(point: DbVector3, segment_start: DbVector3, segment_end: DbVector3) -> f32 {
    let segment = sub(segment_end, segment_start);
    let segment_len_sq = dot(segment, segment);

    if segment_len_sq <= 1e-12 {
        return distance_sq(point, segment_start);
    }

    let start_to_point = sub(point, segment_start);
    let t = clamp_01(dot(start_to_point, segment) / segment_len_sq);
    let closest_point = add(segment_start, mul(segment, t));
    distance_sq(point, closest_point)
}

pub fn segment_intersects_collider_bounding_sphere(ray_origin: DbVector3, ray_direction_unit: DbVector3, max_distance: f32, collider: &ComplexCollider, collider_world_position: DbVector3, collider_yaw_radians: f32, padding: f32) -> bool {
    let center = get_collider_center_world(collider, collider_world_position, collider_yaw_radians);
    let radius = get_complex_collider_bounding_radius(collider) + padding;
    let ray_end = add(ray_origin, mul(ray_direction_unit, max_distance));

    distance_point_to_segment_sq(center, ray_origin, ray_end) <= radius * radius
}

pub fn compute_hull_local_aabb(hull: &ConvexHullCollider) -> (DbVector3, DbVector3) { // Used to reject raycasts before iterating every triangle in a hull
    (hull.aabb_min_local, hull.aabb_max_local)
}

pub fn ray_intersects_aabb(ray_origin: DbVector3, ray_direction: DbVector3, max_distance: f32, aabb_min: DbVector3, aabb_max: DbVector3) -> bool {
    let epsilon: f32 = 1e-8;
    let mut t_min: f32 = 0.0;
    let mut t_max: f32 = max_distance;

    if ray_direction.x.abs() <= epsilon {
        if ray_origin.x < aabb_min.x || ray_origin.x > aabb_max.x { return false; }
    } else {
        let inv_dir_x = 1.0 / ray_direction.x;
        let mut t1 = (aabb_min.x - ray_origin.x) * inv_dir_x;
        let mut t2 = (aabb_max.x - ray_origin.x) * inv_dir_x;
        if t1 > t2 { std::mem::swap(&mut t1, &mut t2); }
        if t1 > t_min { t_min = t1; }
        if t2 < t_max { t_max = t2; }
        if t_min > t_max { return false; }
    }

    if ray_direction.y.abs() <= epsilon {
        if ray_origin.y < aabb_min.y || ray_origin.y > aabb_max.y { return false; }
    } else {
        let inv_dir_y = 1.0 / ray_direction.y;
        let mut t1 = (aabb_min.y - ray_origin.y) * inv_dir_y;
        let mut t2 = (aabb_max.y - ray_origin.y) * inv_dir_y;
        if t1 > t2 { std::mem::swap(&mut t1, &mut t2); }
        if t1 > t_min { t_min = t1; }
        if t2 < t_max { t_max = t2; }
        if t_min > t_max { return false; }
    }

    if ray_direction.z.abs() <= epsilon {
        if ray_origin.z < aabb_min.z || ray_origin.z > aabb_max.z { return false; }
    } else {
        let inv_dir_z = 1.0 / ray_direction.z;
        let mut t1 = (aabb_min.z - ray_origin.z) * inv_dir_z;
        let mut t2 = (aabb_max.z - ray_origin.z) * inv_dir_z;
        if t1 > t2 { std::mem::swap(&mut t1, &mut t2); }
        if t1 > t_min { t_min = t1; }
        if t2 < t_max { t_max = t2; }
        if t_min > t_max { return false; }
    }

    t_max >= 0.0 && t_min <= max_distance
}

pub fn compute_contact_normal(_ctx: &ReducerContext, raw_normal: DbVector3, center_a: DbVector3, center_b: DbVector3) -> DbVector3 { // Orients collision normal outward towards target (center_a) - Rounds normals to approximate collisions
    let mut normal = raw_normal;
    if dot(normal, normal) < 1e-6 { return DbVector3 { x: 0.0, y: 1.0, z: 0.0 }; }
    normal = normalize(normal);

    let center_delta = sub(center_a, center_b);
    let center_delta_sq: f32 = dot(center_delta, center_delta);

    if center_delta_sq > 1e-8 {
        if dot(normal, center_delta) < 0.0 { normal = negate(normal); }
    }

    let world_up = DbVector3 { x: 0.0, y: 1.0, z: 0.0 };
    let up_dot: f32 = dot(normal, world_up);

    let floor_snap_dot: f32 = 0.98;
    let ceiling_snap_dot: f32 = -0.98;
    let wall_snap_abs_dot: f32 = 0.05;

    if up_dot >= floor_snap_dot { return world_up; } // Floor collision
    if up_dot <= ceiling_snap_dot { return negate(world_up); } // Ceiling collision

    if up_dot.abs() <= wall_snap_abs_dot { // Wall collision
        normal.y = 0.0;
        return normalize(normal);
    }

    normal
}
