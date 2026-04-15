use spacetimedb::{ReducerContext, Table};
use crate::*;

pub fn raycast_match(ctx: &ReducerContext, ray_origin: DbVector3, ray_direction: DbVector3, max_distance: f32) -> Raycast { // Returns single closest raycast target within max distance
    let mut has_hit: bool = false;
    let mut best_distance: f32 = max_distance;
    let mut best_point: DbVector3 = DbVector3 { x: 0.0, y: 0.0, z: 0.0 };
    let mut best_type: RaycastHitType = RaycastHitType::None;
    let mut best_entity_id: u64 = 0;
    let mut best_collider_type: ConvexHullColliderType = ConvexHullColliderType::None;
    let mut best_hit_name: String = "".to_string();

    let ray_direction_unit: DbVector3 = normalize_small_vector(ray_direction, DbVector3 { x: 0.0, y: 0.0, z: 1.0 });
    let ray_padding: f32 = 0.05;

    let magician_option = ctx.db.magician().identity().find(ctx.sender());
    if let Some(magician) = magician_option {
        for other in ctx.db.magician().game_id().filter(magician.game_id) {
            if other.identity == ctx.sender() { continue; }
            if segment_intersects_collider_bounding_sphere(ray_origin, ray_direction_unit, best_distance, &other.collider, other.position, to_radians(other.rotation.yaw), ray_padding) == false { continue; }

            let hit: Raycast = raycast_complex_collider(ray_origin, ray_direction_unit, best_distance, &other.collider, other.position, to_radians(other.rotation.yaw), RaycastHitType::Magician, other.id);
            if hit.hit && hit.hit_distance < best_distance {
                has_hit = true;
                best_distance = hit.hit_distance;
                best_point = hit.hit_point;
                best_type = hit.hit_type;
                best_entity_id = hit.hit_entity_id;
                best_collider_type = hit.collider_type;
                best_hit_name = other.name.clone();
            }
        }

        for map_piece in ctx.db.map().iter() {
            if segment_intersects_collider_bounding_sphere(ray_origin, ray_direction_unit, best_distance, &map_piece.collider, DbVector3 { x: 0.0, y: 0.0, z: 0.0 }, 0.0, ray_padding) == false { continue; }

            let hit: Raycast = raycast_complex_collider_world_space(ray_origin, ray_direction_unit, best_distance, &map_piece.collider, RaycastHitType::MapPiece, map_piece.id);
            if hit.hit && hit.hit_distance < best_distance {
                has_hit = true;
                best_distance = hit.hit_distance;
                best_point = hit.hit_point;
                best_type = hit.hit_type;
                best_entity_id = hit.hit_entity_id;
                best_collider_type = hit.collider_type;
                best_hit_name = "".to_string();
            }
        }
    }

    Raycast { hit: has_hit, hit_distance: best_distance, hit_point: best_point, hit_type: best_type, hit_entity_id: best_entity_id, collider_type: best_collider_type, hit_name: best_hit_name }
}

pub fn raycast_map_only_match_for_pieces(map_pieces: &[Map], ray_origin: DbVector3, ray_direction: DbVector3, max_distance: f32) -> Raycast {
    let mut has_hit: bool = false;
    let mut best_distance: f32 = max_distance;
    let mut best_point: DbVector3 = DbVector3 { x: 0.0, y: 0.0, z: 0.0 };
    let mut best_entity_id: u64 = 0;
    let ray_direction_unit: DbVector3 = normalize_small_vector(ray_direction, DbVector3 { x: 0.0, y: 0.0, z: 1.0 });
    let ray_padding: f32 = 0.05;

    for map_piece in map_pieces.iter() {
        if segment_intersects_collider_bounding_sphere(ray_origin, ray_direction_unit, best_distance, &map_piece.collider, DbVector3 { x: 0.0, y: 0.0, z: 0.0 }, 0.0, ray_padding) == false { continue; }

        let hit = raycast_complex_collider_world_space(ray_origin, ray_direction_unit, best_distance, &map_piece.collider, RaycastHitType::MapPiece, map_piece.id);
        if hit.hit && hit.hit_distance < best_distance {
            has_hit = true;
            best_distance = hit.hit_distance;
            best_point = hit.hit_point;
            best_entity_id = hit.hit_entity_id;
        }
    }

    Raycast {
        hit: has_hit,
        hit_distance: best_distance,
        hit_point: best_point,
        hit_type: if has_hit { RaycastHitType::MapPiece } else { RaycastHitType::None },
        hit_entity_id: best_entity_id,
        collider_type: ConvexHullColliderType::None,
        hit_name: "".to_string()
    }
}

pub fn raycast_map_only_match(ctx: &ReducerContext, ray_origin: DbVector3, ray_direction: DbVector3, max_distance: f32) -> Raycast {
    let map_pieces: Vec<Map> = ctx.db.map().iter().collect();
    raycast_map_only_match_for_pieces(&map_pieces, ray_origin, ray_direction, max_distance)
}

pub fn raycast_beam_match(ctx: &ReducerContext, ray_origin: DbVector3, ray_direction: DbVector3, max_distance: f32, beam_half_angle_degrees: f32) -> Raycast { // Samples several individual raycasts to emulate a small beam - Uses offset origins so close range feels less strict
    let ray_direction_unit: DbVector3 = normalize_small_vector(ray_direction, DbVector3 { x: 0.0, y: 0.0, z: 1.0 });
    let beam_right: DbVector3 = any_perpendicular_unit(ray_direction_unit);
    let beam_up: DbVector3 = normalize_small_vector(cross(beam_right, ray_direction_unit), DbVector3 { x: 0.0, y: 1.0, z: 0.0 });

    let beam_reference_distance: f32 = 6.0;
    let min_beam_radius: f32 = 0.08;
    let beam_radius: f32 = (to_radians(beam_half_angle_degrees).tan() * beam_reference_distance).max(min_beam_radius);
    let diagonal: f32 = 0.70710677;

    let sample_offsets: [(f32, f32); 9] = [
        (0.0, 0.0),
        (1.0, 0.0),
        (-1.0, 0.0),
        (0.0, 1.0),
        (0.0, -1.0),
        (diagonal, diagonal),
        (diagonal, -diagonal),
        (-diagonal, diagonal),
        (-diagonal, -diagonal)
    ];

    let beam_end_point: DbVector3 = add(ray_origin, mul(ray_direction_unit, max_distance));
    let mut best_magician_hit: Option<Raycast> = None;
    let mut best_other_hit: Option<Raycast> = None;
    let mut best_magician_distance: f32 = max_distance;
    let mut best_other_distance: f32 = max_distance;

    for (sample_x, sample_y) in sample_offsets {
        let sample_origin = add(ray_origin, add(mul(beam_right, sample_x * beam_radius), mul(beam_up, sample_y * beam_radius)));
        let sample_direction = normalize_small_vector(sub(beam_end_point, sample_origin), ray_direction_unit);
        let hit = raycast_match(ctx, sample_origin, sample_direction, max_distance);
        if hit.hit == false { continue; }

        let center_distance = magnitude(sub(hit.hit_point, ray_origin));

        if hit.hit_type == RaycastHitType::Magician {
            if best_magician_hit.is_none() || center_distance < best_magician_distance {
                best_magician_distance = center_distance;
                best_magician_hit = Some(hit);
            }
        }

        else if best_other_hit.is_none() || center_distance < best_other_distance {
            best_other_distance = center_distance;
            best_other_hit = Some(hit);
        }
    }

    if let Some(hit) = best_magician_hit {
        return hit;
    }

    if let Some(hit) = best_other_hit {
        return hit;
    }

    Raycast { hit: false, hit_distance: max_distance, hit_point: DbVector3 { x: 0.0, y: 0.0, z: 0.0 }, hit_type: RaycastHitType::None, hit_entity_id: 0, collider_type: ConvexHullColliderType::None, hit_name: "".to_string() }
}

pub fn raycast_cone_match(ctx: &ReducerContext, ray_origin: DbVector3, ray_forward: DbVector3, max_distance: f32, cone_half_angle_degrees: f32) -> Vec<Raycast> { // Returns all targets with cone of max distance - Target must be facing player (enables flash dodges)
    let forward_unit = normalize_small_vector(ray_forward, DbVector3 { x: 0.0, y: 0.0, z: 1.0 });
    let cone_half_angle_radians = to_radians(cone_half_angle_degrees);
    let min_dot = cone_half_angle_radians.cos();

    let auto_hit_distance: f32 = 1.25;
    let mut hits: Vec<Raycast> = Vec::new();

    let magician_option = ctx.db.magician().identity().find(ctx.sender());
    if let Some(magician) = magician_option { 
        let map_pieces: Vec<Map> = ctx.db.map().iter().collect();
        for other in ctx.db.magician().game_id().filter(magician.game_id) {
            if other.identity == ctx.sender() { continue; }

            let other_yaw_radians = to_radians(other.rotation.yaw);
            let other_center_world = get_collider_center_world(&other.collider, other.position, other_yaw_radians);

            let center_vector_to_other = sub(other_center_world, ray_origin);
            let center_distance = magnitude(center_vector_to_other);

            if center_distance > max_distance { continue; }

            if center_distance <= auto_hit_distance { // Sensitive zone where raycast detection is wonky due to origin being outside of player hitbox
                let map_blocker = raycast_map_only_match_for_pieces(&map_pieces, ray_origin, center_vector_to_other, center_distance);
                if map_blocker.hit { continue; }

                hits.push(Raycast {
                    hit: true,
                    hit_distance: center_distance,
                    hit_point: other_center_world,
                    hit_type: RaycastHitType::Magician,
                    hit_entity_id: other.id,
                    collider_type: ConvexHullColliderType::None,
                    hit_name: other.name.clone()
                });
                continue;
            }

            let center_unit_to_other = normalize_small_vector(center_vector_to_other, forward_unit);
            let center_alignment = dot(forward_unit, center_unit_to_other);
            if center_alignment < min_dot { continue; }

            let collider_hit = raycast_complex_collider(ray_origin, center_unit_to_other, max_distance, &other.collider, other.position, other_yaw_radians, RaycastHitType::Magician, other.id);
            if collider_hit.hit == false { continue; }

            let map_blocker = raycast_map_only_match_for_pieces(&map_pieces, ray_origin, center_unit_to_other, collider_hit.hit_distance);
            if map_blocker.hit && map_blocker.hit_distance + 1e-4 < collider_hit.hit_distance { continue; }

            hits.push(collider_hit);
        }
    }
    
    hits
}

pub fn raycast_complex_collider(ray_origin: DbVector3, ray_direction_unit: DbVector3, max_distance: f32, collider: &ComplexCollider, collider_world_position: DbVector3, collider_yaw_radians: f32, hit_type: RaycastHitType, hit_entity_id: u64) -> Raycast // Raycast logic pipeline that functions with engine collider system
{
    let mut has_hit: bool = false;
    let mut best_distance: f32 = max_distance;
    let mut best_point: DbVector3 = DbVector3 { x: 0.0, y: 0.0, z: 0.0 };
    let mut best_collider_type: ConvexHullColliderType = ConvexHullColliderType::None;

    if segment_intersects_collider_bounding_sphere(ray_origin, ray_direction_unit, max_distance, collider, collider_world_position, collider_yaw_radians, 0.05) == false {
        return Raycast { hit: false, hit_distance: best_distance, hit_point: best_point, hit_type: RaycastHitType::None, hit_entity_id: hit_entity_id, collider_type: ConvexHullColliderType::None, hit_name: "".to_string() };
    }

    let local_origin: DbVector3 = rotate_around_y_axis(sub(ray_origin, collider_world_position), -collider_yaw_radians);
    let local_direction: DbVector3 = rotate_around_y_axis(ray_direction_unit, -collider_yaw_radians);

    for hull in collider.convex_hulls.iter() {
        let mut hit_distance_local: f32 = best_distance;
        if raycast_convex_hull_triangles(local_origin, local_direction, best_distance, hull, &mut hit_distance_local) {
            has_hit = true;
            best_distance = hit_distance_local;
            best_collider_type = hull.collider_type;

            let local_hit_point: DbVector3 = add(local_origin, mul(local_direction, hit_distance_local));
            best_point = add(collider_world_position, rotate_around_y_axis(local_hit_point, collider_yaw_radians));
        }
    }

    Raycast { hit: has_hit, hit_distance: best_distance, hit_point: best_point, hit_type: if has_hit { hit_type } else { RaycastHitType::None }, hit_entity_id: hit_entity_id, collider_type: if has_hit { best_collider_type } else { ConvexHullColliderType::None }, hit_name: "".to_string() }
}

pub fn raycast_complex_collider_world_space(ray_origin: DbVector3, ray_direction_unit: DbVector3, max_distance: f32, collider: &ComplexCollider, hit_type: RaycastHitType, hit_entity_id: u64) -> Raycast {
    let mut has_hit: bool = false;
    let mut best_distance: f32 = max_distance;
    let mut best_point: DbVector3 = DbVector3 { x: 0.0, y: 0.0, z: 0.0 };
    let mut best_collider_type: ConvexHullColliderType = ConvexHullColliderType::None;

    if segment_intersects_collider_bounding_sphere(ray_origin, ray_direction_unit, max_distance, collider, DbVector3 { x: 0.0, y: 0.0, z: 0.0 }, 0.0, 0.05) == false {
        return Raycast { hit: false, hit_distance: best_distance, hit_point: best_point, hit_type: RaycastHitType::None, hit_entity_id: hit_entity_id, collider_type: ConvexHullColliderType::None, hit_name: "".to_string() };
    }

    for hull in collider.convex_hulls.iter() {
        let mut hit_distance: f32 = best_distance;
        if raycast_convex_hull_triangles(ray_origin, ray_direction_unit, best_distance, hull, &mut hit_distance) {
            has_hit = true;
            best_distance = hit_distance;
            best_point = add(ray_origin, mul(ray_direction_unit, hit_distance));
            best_collider_type = hull.collider_type;
        }
    }

    Raycast { hit: has_hit,hit_distance: best_distance, hit_point: best_point, hit_type: if has_hit { hit_type } else { RaycastHitType::None }, hit_entity_id: hit_entity_id, collider_type: if has_hit { best_collider_type } else { ConvexHullColliderType::None }, hit_name: "".to_string() }
}

pub fn raycast_convex_hull_triangles(ray_origin_local: DbVector3, ray_direction_local: DbVector3, max_distance: f32, hull: &ConvexHullCollider, hit_distance_out: &mut f32) -> bool {
    *hit_distance_out = max_distance;
    let mut has_hit: bool = false;

    let (aabb_min, aabb_max) = compute_hull_local_aabb(hull);
    if ray_intersects_aabb(ray_origin_local, ray_direction_local, max_distance, aabb_min, aabb_max) == false {
        return false;
    }

    let vertices: &Vec<DbVector3> = &hull.vertices_local;
    let triangles: &Vec<i32> = &hull.triangle_indices_local;

    let mut index: usize = 0;
    while index + 2 < triangles.len() {
        let a: DbVector3 = vertices[triangles[index] as usize];
        let b: DbVector3 = vertices[triangles[index + 1] as usize];
        let c: DbVector3 = vertices[triangles[index + 2] as usize];

        let mut triangle_distance: f32 = 0.0;
        if ray_intersects_triangle(ray_origin_local, ray_direction_local, a, b, c, &mut triangle_distance) {
            if triangle_distance >= 0.0 && triangle_distance < *hit_distance_out {
                has_hit = true;
                *hit_distance_out = triangle_distance;
            }
        }

        index += 3;
    }

    has_hit
}

pub fn ray_intersects_triangle(ray_origin: DbVector3, ray_direction: DbVector3, a: DbVector3, b: DbVector3, c: DbVector3, distance_out: &mut f32) -> bool {
    *distance_out = 0.0;

    let edge1: DbVector3 = sub(b, a);
    let edge2: DbVector3 = sub(c, a);

    let pvec: DbVector3 = cross(ray_direction, edge2);
    let det: f32 = dot(edge1, pvec);

    let epsilon: f32 = 1e-7;
    if det > -epsilon && det < epsilon { return false; }

    let inverse_det: f32 = 1.0 / det;

    let tvec: DbVector3 = sub(ray_origin, a);
    let u: f32 = dot(tvec, pvec) * inverse_det;
    if u < 0.0 || u > 1.0 { return false; }

    let qvec: DbVector3 = cross(tvec, edge1);
    let v: f32 = dot(ray_direction, qvec) * inverse_det;
    if v < 0.0 || u + v > 1.0 { return false; }

    let t: f32 = dot(edge2, qvec) * inverse_det;
    if t < 0.0 { return false; }

    *distance_out = t;
    true
}
