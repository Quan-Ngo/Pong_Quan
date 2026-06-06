# Changelog

## [Unreleased]

### Added
- Title Screen UI with a play button and interactive panning animation.
- Camera and gameplay UI panning (tweening camera Y from -15 to 0, and ScoreDisplay from Y = 300 to 0) over 1.5 seconds.
- Lock paddles, timers, and ball serving mechanics until the panning transition completes, signaled by the `StartGameEventChannel`.
- Game Over panel slide-in tween animation that smoothly slides the panel from offscreen (Y = 800) to its design position using DOTween upon activation.


### Fixed
- Paddles no longer respawn if they are destroyed on a game-winning point.
- Paddles are now properly restored to their original state and position upon restarting a match from the Game Over screen.
- Prevents the ball from being served after the game has ended by managing and cancelling the serve coroutine on game over.
- Powerup spawner now explicitly listens to the game over event to stop spawning and clear all active powerups.
