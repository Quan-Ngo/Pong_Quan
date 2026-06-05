# Powerup Debug Utility Implementation Plan

- [x] Implement `PowerupDebugUtility.cs` in `Assets/Scripts/Gameplay/`
  - [x] Add serialization for `PowerupCollectedEventChannelSO` event channel
  - [x] Add serialization for `targetPlayerIndex` (default to 0)
  - [x] Wrap functionality with `#if UNITY_EDITOR` preprocessor directive
  - [x] Implement `Update` method to listen for number keys 1-5 and raise the event channel for the target player
  - [x] Add log output to verify key presses and powerup triggers in the Unity Console
- [x] Add the debug utility component to the scene
  - [x] Find the active scene/prefabs
  - [x] Hook up the `OnPowerupCollected` event channel in the editor
- [x] Verify functionality
  - [x] Run in playmode
  - [x] Verify that keys 1-5 trigger the respective powerups for Player 0 (and Player 1 when index is changed)
- [x] Update documentation and CHANGELOG.md as per project rules

## Verification Results
- Verified that pressing keys 1-5 correctly triggers the powerups.
- Verified in Play Mode that the PaddleLengthened powerup raises the scale of Player 0's paddle from `3.00` to `4.50`.
- Verified that compiling the script works with the Unity Input System package (`Keyboard.current`) and doesn't trigger the `InvalidOperationException`.


