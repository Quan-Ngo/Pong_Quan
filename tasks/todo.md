# Pong Verification and Playtesting TODO

- [ ] Create assembly definitions for scripts and tests (`PongGame.asmdef` and `PongTests.asmdef`)
- [ ] Implement PlayMode integration tests:
  - [ ] Test ball boundary bouncing (top/bottom walls)
  - [ ] Test paddle deflection angles (hitting top, center, bottom of paddle)
  - [ ] Test speed increase per paddle hit (5% increment up to max)
  - [ ] Test scoring and serve direction (served towards the player who lost the point)
  - [ ] Test victory condition (first to 7 wins, game over state)
- [ ] Run PlayMode tests to verify all mechanics pass
- [ ] Perform manual playtesting/verification via screenshots if necessary
- [ ] Update documentation:
  - [ ] Add entry to `CHANGELOG.md`
  - [ ] Update other documentation if user-visible or configuration changes occur
