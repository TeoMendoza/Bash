## Overview

This document provides a technical overview of the backend architecture of **Bash**, developed in Unity with SpacetimeDB. It focuses on the core runtime systems that govern authority, state, physics, and interaction between players, describing how the system operates structurally without diving into low-level implementation or gameplay design.

---

## 1. Initialization, Match Management & Ownership

### GameManager and Match Lifecycle

The GameManager is responsible only for initial setup and does not participate in gameplay. Runtime control is handled entirely by the MatchManager.

The MatchManager listens for when the local player joins a match. Upon joining, it subscribes exclusively to that match’s data, spawns all existing entities, and continues reacting to updates throughout the match. When the player leaves, it destroys all active objects, clears subscriptions, and returns to a passive listening state. This ensures each match operates as a fully isolated runtime environment.

### Instance Model and Authority

Each client maintains one local instance (player-controlled) and N–1 remote instances (other players). All instances read from the database and update visuals, but only the local instance is allowed to send reducers.

- Local instance handles input and sends state requests
- Remote instances are strictly read-only and reflect database state

Authority is strictly enforced: clients may only mutate their own state, and all cross-player interaction is validated server-side.

### Client Interpolation

Clients do not simulate gameplay state, but interpolate between server updates for position and rotation. This provides smooth rendering without affecting authoritative simulation.

---

## 2. Character State & Permissions

### State System

Character state is stored on the player row and divided into two independent layers.

Kinematic state represents continuous movement such as walking, running, crouching, and airborne behavior. While influenced by player input, it is resolved by the physics system, which determines the final velocity and position. This state is broadcast to clients and used as the base for full-body animation.

Action state represents discrete gameplay actions such as attacking, reloading, or abilities. It is triggered through reducers and applied as an upper-body override, allowing actions to layer cleanly on top of movement.

These two layers operate independently and are composed client-side into the final animation result.

### Permission System

Permissions are enforced through a shared config stored on the player row. Each permission maintains a set of active blockers rather than a simple boolean flag.

- Systems add themselves to block a permission
- Systems remove themselves when finished
- A permission is valid only when its blocking set is empty

This allows multiple systems to restrict the same action without conflict. All player intent is validated against this system before execution.

---

## 3. Effect System

### Centralized Processing

All gameplay effects are processed through a centralized system driven by a scheduled reducer per match. Effects are inserted into this system and resolved over time rather than being applied directly during actions.

### Effect Behavior

Effects follow a small set of lifecycle patterns:

- Instant effects apply once and are removed immediately
- Timed effects apply once and are reverted after a duration
- Reapplying effects execute repeatedly at intervals
- Conditional effects persist until a specific condition is met

All effect logic and state mutation occur within the reducer, ensuring deterministic and consistent behavior across all clients.

---

## 4. Physics & Movement System

### Server-Authoritative Simulation

The physics system is fully server-authoritative and runs per match through a scheduled reducer. It is responsible for all movement, collision detection, and collision resolution.

### Movement Resolution

Players submit movement intent such as directional input, jumping, or crouching. The physics system resolves collisions, computes the resulting velocity, and updates position and state on the player row.

Clients receive these updates and interpolate between them for smooth motion, but do not influence the simulation.

### Collision and Broadphase

Entities are represented using convex hulls, with GJK used for collision detection and EPA for penetration resolution.

To reduce computational cost, a broadphase filtering system is used:

- Client-side trigger colliders track nearby entities
- A set of potential interaction candidates is maintained
- Physics only evaluates detailed collisions on this filtered set

This avoids full pairwise checks while preserving accuracy.

---

## 5. Combat & Raycast System

### Server-Side Reconstruction

Combat interactions are validated entirely on the server. When a player performs an attack, the client sends camera-relative data describing the shot at the moment of input.

The server reconstructs the player’s camera position and orientation using this data, allowing it to accurately reproduce the player’s perspective.

### Hit Detection and Resolution

Using the reconstructed data, the server performs raycasts to determine whether a hit occurred. If a valid hit is detected, the result is passed into the effect system rather than directly mutating state.

This ensures that all combat outcomes are processed through a consistent pipeline and remain fully server-authoritative.

---

## Summary

The system is built around a server-authoritative architecture with match-scoped execution and strict ownership rules.

- Clients handle input and rendering
- All simulation and validation occur server-side
- Movement is physics-driven and authoritative
- Actions are layered through state and permissions
- Combat is resolved through server-side raycasting
- Effects are processed through a unified system
- Matches operate as fully isolated environments

This structure ensures consistency, scalability, and predictable behavior across all gameplay systems.
