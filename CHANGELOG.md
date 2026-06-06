# Changelog

All notable changes to this project will be documented in this file.

## [0.3.1] - 2026-06-06

### Added
- **Goal Visual Juice**:
  - Added strong screenshake, paddle explosion particles, and confetti effects when a goal is scored.
  - Paused the serve timer until the losing paddle completely scales back into existence.
  - Implemented decoupled, event-driven paddle respawn logic using a new `PaddleRespawnedEvent` Event Channel.
  - Added a C# Editor Utility script (`GoalJuiceSetupUtility`) to programmatically build perfectly styled square-particle prefabs and bind them to scene dependencies.

### Changed
- **Visual Effects Color Extraction**: Extracted speed-scaled HSV color generation into a reusable `GenerateSpeedScaledColor` helper method inside `GameVisualEffectsManager.cs` to eliminate duplicate color generation logic.
- **Visual Effects Saturation Scaling**: Refactored background flash and shockwave colors in `GameVisualEffectsManager.cs` to scale their saturation (S) with ball speed using the HSV color model, while maintaining a constant 90% brightness (V = 0.9f) by default to keep a vibrant, consistent aesthetic.
- **Customizable Saturation Parameters**: Added configurable fields (`initialSaturationValue` and `speedScaleFactor`) to `GameVisualEffectsManager.cs` to scale saturation dynamically according to the formula: `(initialSaturationValue + normalizedSpeed) * speedScaleFactor`.
- **Shockwave Ring Fade Curve**: Adjusted `ShockwaveRing.cs` to use `Ease.InQuad` so the vector ring stays solid for most of its duration and fades out rapidly at the end.
- **Custom Background Flash Brightness**: Utilized the newly added `backGroundFlashBrightness` field in `GameVisualEffectsManager.cs` to set the brightness of the background flash, allowing independent control from other visual effect brightness values.

## [0.3.0] - 2026-06-06

### Added
- **Vector Shockwave Ring Effect**: Spawns a clean expanding vector ring at the exact impact point of any ball collision.
  - Generated high-quality anti-aliased $256 \times 256$ vector ring texture (`Ring.png`) and imported as a Sprite.
  - Created `ShockwaveRing.cs` to handle the scaling-up and fading-out animation cycles of the ring, with auto-destruction on completion.
  - Integrated `ShockwaveRing` prefab spawning in `GameVisualEffectsManager.cs`, scaling final size with the ball speed and assigning randomized vibrant colors.
  - Created, saved, and linked the `ShockwavePrefab` asset to the `VisualEffectsManager` scene object.

### Removed
- **Particle System Explosions**: Removed the old particle system prefab reference and spawning code from `GameVisualEffectsManager.cs`, fully replacing it with the clean vector shockwave ring effect.

### Fixed
- **Shockwave Typing & Binding**: Resolved C# compilation error `CS0029` by changing `shockwavePrefab` instantiation logic to return `GameObject` and retrieving the `ShockwaveRing` component. Re-bound the correct `ShockwavePrefab.prefab` asset reference to the `VisualEffectsManager` script component in `PongScene.unity` using a C# editor script.

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
- `PowerupDebugUtility.cs` — Editor-only debug utility allowing developers to trigger any of the 5 powerups using the number keys 1–5 in the Unity Editor (configured to target either Player 0 or Player 1).
- **Visual Feedback System**:
  - Speed-scaled **Camera Shake** (position and roll) upon ball collisions using DOTween.
  - Speed-scaled, random vibrant color **Background Flash** using DOTween.
  - **Particle Explosions** at the exact contact point of ball collisions.
- `BallCollisionEventChannelSO.cs` — Event channel decoupling visual feedback from physics logic.
- `GameVisualEffectsManager.cs` — Centralized manager handling all visual feedback events.

### Fixed
- Re-added the missing `EventSystem` to `PongScene`, fixing an issue where the game over Restart button was unclickable.

### Changed
- `PowerupSpawner.cs` — Pauses the spawn timer countdown when a powerup box is active on the field, resuming it only after collection/destruction.
- `BallController.cs` — Added `lastHitterIndex` tracking, `onBallServed` event broadcast, and dynamic speed multiplier based on active ball powerups. Refactored `FriendlySlowBall` out of polling loop in favor of trigger zones. Renamed `_zoneMultipliers` to `_speedMultipliers` (along with Add/Remove helper methods) for clarity. Modified `lastHitterIndex` to be a property that invokes an `OnLastHitterChanged` event and removed polling logic for `OpponentFastBall`.
- `PowerupManager.cs` — Changed `FriendlySlowBall` implementation to activate physical `SlowZone` areas on the field instead of polling. Added subscription to `OnLastHitterChanged` to event-drive the `OpponentFastBall` multiplier directly onto the ball.
- `BallCollisionHandler.cs` — Now updates `lastHitterIndex` on paddle hits and GoalGuard wall hits.
- `PaddleController.cs` — Added `playerIndex` field, `SetSpeedMultiplier()` and `SetScaleMultiplier()` methods. Paddle is fully decoupled from powerup logic.

## [0.1.0] - 2026-06-05

### Added
- Core Pong game: 2-player local multiplayer with W/S and Up/Down arrow controls.
- Ball physics: bounce off walls, angle deflection based on paddle hit position, speed increase per hit.
- Scoring: first to 7 wins, score displayed on screen.
- Serve direction: randomized at game start, directed towards losing player after each point.
- Game over screen with restart functionality.
