# Changelog

## [Unreleased]

### Fixed
- Paddles no longer respawn if they are destroyed on a game-winning point.
- Paddles are now properly restored to their original state and position upon restarting a match from the Game Over screen.
- Prevents the ball from being served after the game has ended by managing and cancelling the serve coroutine on game over.
- Powerup spawner now explicitly listens to the game over event to stop spawning and clear all active powerups.
