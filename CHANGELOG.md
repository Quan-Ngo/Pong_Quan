# Changelog

All notable changes to this project will be documented in this file.

## [0.2.0] - 2026-06-05

### Added
- **Powerup System** with 5 powerup types:
  - **Paddle Lengthened**: Increases paddle Y-scale for the collecting player.
  - **Paddle Speed Boost**: Increases paddle movement speed for the collecting player.
  - **Opponent Fast Ball**: Ball travels faster when moving towards the opponent's side.
  - **Friendly Slow Ball**: Ball slows down when on the collecting player's half of the screen.
  - **Goal Guard**: Activates two barrier walls flanking the top and bottom of the goal zone, narrowing the scoring target.
- `PowerupBox.cs` — Floats vertically at screen center; awards a random powerup to the last ball hitter on collision.
- `PowerupSpawner.cs` — Spawns powerup boxes at random intervals after serve; clears all on goal.
- `PowerupManager.cs` — Central state tracker managing active powerup timers per player, supporting multiple concurrent powerups.
- `GoalGuardWall.cs` — Marker component on goal guard barrier walls; updates ball's lastHitterIndex on collision.
- `PowerupType.cs` — Enum defining the 5 powerup types.
- `PowerupCollectedEventChannelSO.cs` — Event channel for powerup collection broadcasts.
- Powerup configuration in `GameSettingsSO`: spawn intervals, duration, multipliers.

### Changed
- `PowerupSpawner.cs` — Pauses the spawn timer countdown when a powerup box is active on the field, resuming it only after collection/destruction.
- `BallController.cs` — Added `lastHitterIndex` tracking, `onBallServed` event broadcast, and dynamic speed multiplier based on active ball powerups.
- `BallCollisionHandler.cs` — Now updates `lastHitterIndex` on paddle hits and GoalGuard wall hits.
- `PaddleController.cs` — Added `playerIndex` field, `SetSpeedMultiplier()` and `SetScaleMultiplier()` methods. Paddle is fully decoupled from powerup logic.

## [0.1.0] - 2026-06-05

### Added
- Core Pong game: 2-player local multiplayer with W/S and Up/Down arrow controls.
- Ball physics: bounce off walls, angle deflection based on paddle hit position, speed increase per hit.
- Scoring: first to 7 wins, score displayed on screen.
- Serve direction: randomized at game start, directed towards losing player after each point.
- Game over screen with restart functionality.
