## Overview
Bash is a fast-paced, skill-driven free-for-all arena brawler where 8 to 10 players compete in high-intensity matches built around mechanical precision, movement, and ability mastery. This document explores the design decisions behind the game; why it is structured the way it is, what problems it was designed to solve, and how its systems work together to create the intended experience. It covers the core design philosophy, player count, scoring, character framework, and the areas that went through the most iteration during development.

---

## Design Philosophy
Bash was built around a single question: what does a game look like if fighting is the objective, not a means to one?

Most competitive multiplayer games either reward team strategy and coordination, which naturally pulls players toward passive and supportive roles, or they are free-for-all formats like battle royales where permanent elimination discourages the kind of aggressive, chaotic play that makes those games most exciting. In both cases, players drawn to high mechanical skill and constant action are working against the format rather than with it.

Bash was designed to solve that. No objectives, no teams, no permanent elimination. The goal is simply to fight, and the game is built entirely around making that as skill expressive, chaotic, and rewarding as possible. The intended feel is somewhere between a casual lobby minigame and a competitive arena brawler; approachable and fun by default, but with enough mechanical depth that skilled play is always visible and always rewarded.

---

## Player Count
Bash is designed for 8 to 10 players, a deliberate balance decision rooted in how chaos scales with player count.

With complex ability kits and constant aggression as the default, player interactions multiply quickly. Too few players and the game loses its frenetic energy. Too many and the chaos becomes overwhelming, burying skill expression under a constant inability to set up plays or read the field. 8 to 10 players is the balance point where the arena feels active and unpredictable, but skilled players can still find windows to execute and outplay opponents.

---

## Scoring System
The scoring system rewards aggression and overall performance, not just eliminations.

In most competitive formats winning is the only thing that matters, whether that means capturing an objective or being the last one standing. How much damage you dealt or how aggressively you played along the way is largely irrelevant to the outcome. This creates a natural incentive for passive play; sitting back, letting others fight, and capitalizing at the end. In a game where fighting is the objective, that behavior works directly against the design.

Bash addresses this by awarding points for damage dealt, eliminations, streaks, and sustained pressure. A player who aggressively fights through a match is rewarded even without landing the final blow. This discourages third party opportunism and ensures that players who are actively engaged are recognized regardless of how they prefer to play. Aggressive duelists, calculated pressurers, and opportunistic finishers can all succeed within the same system.

---

## Character Design Framework
The three playable characters are built around a rock-paper-scissors matchup framework, where every character has clear strengths against one opponent and clear vulnerabilities against another. No character is universally dominant.

This structure creates meaningful matchup decisions, particularly since players can switch characters mid match, and keeps the meta dynamic rather than collapsing around a single dominant pick. The harder design challenge was not the framework itself but ensuring each kit felt distinct, viable, and expressive within the chaotic free-for-all format. Kits that are too complex get lost in the noise. Kits that are too simple do not give skilled players enough tools to separate themselves.

The core gameplay loop was established early and remained stable throughout development, but character kits went through significant iteration to find that balance. Each character went through multiple passes to ensure that mastery feels rewarding without the kit becoming a crutch or an unreadable nuisance for opponents.
