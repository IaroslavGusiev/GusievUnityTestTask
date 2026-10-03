# UnityTestTask

An extension of the existing Bludoku project for the Unity Developer Test Assignment: a combo system, combo VFX, and extensible analytics.

## Approach

The implementation keeps the existing gameplay foundation and visual identity. New features reuse the board events, score calculation, UI, and particle effects. Refactoring focuses on the code involved in those features.

Plain C# classes hold combo rules, combo state, and analytics data. Mediators connect these systems to gameplay, while presenters and views handle visual feedback. Explicit scene references and the existing assembly structure keep the scope small.

## Combo system

Each successful placement that clears cells increases the combo by one. The combo becomes active at two clears and enables the existing **1.5x score multiplier**, with the original integer rounding preserved.

Two strategies implement `IComboRule`:

- **Consecutive Clears:** a placement without a clear breaks the combo.
- **Grace Moves:** configurable non-clearing placements preserve the combo; another clear refills the allowance. The current scene allows two grace moves, with the third non-clearing placement breaking the combo.

Rejected placements do not affect the combo. `ComboSystem` owns the authoritative state; a rule returns an advance, hold, or break decision. `ComboMediator` connects board placements to that state, and `ScoreMediator` uses it to award score. Additional rules can use the same interface without changing the views or score calculation.

## Combo feedback and VFX

The existing visual style is extended with:

- A combo badge and animated grace indicators, including a last-chance warning.
- Combo text that pops, rises, pauses, and follows a Bezier curve to the badge, with a colored particle trail and an arrival response.
- A repeating heart heartbeat with sparkles while the combo remains active.
- Confetti at the cleared area's center and stronger existing clear particles during combos.

Higher combos use stronger text appearance, trail colors, heart animation, and particle intensity. Dedicated view classes own the animation sequences and particle integration. A reusable pool of flight views supports overlapping feedback, with presentation canceled when gameplay UI is hidden or the combo resets. Effects stay on the UI and cleared cells.

## Analytics

`IAnalyticsProvider` separates event delivery from gameplay. The current `ConsoleAnalyticsProvider` writes yellow `[Analytics]` logs to the Unity Console; no real analytics service is required for this assignment.

| Event | Tracked action |
| --- | --- |
| `piece_moved` | A piece is placed or returned after release |
| `bonus_received` | Extra score is awarded by an active combo |
| `power_up_used` | Second Chance replaces the available pieces |
| `combo_broken` | An active combo breaks |
| `game_started` | A new or restored game begins |
| `game_over` | No remaining piece can be placed; includes final and best score |

Event names are centralized in `AnalyticsEvents`. `AnalyticsEvent` carries extensible string, integer, and decimal parameters. Adding an event means adding its constant and tracking it at the gameplay action; adding fields means extending its payload. A future Firebase provider can implement the same interface and replace the provider created by `GameController`.

## Refactoring and saves

`GameController` coordinates initialization and disposal of the feature modules. Their gameplay subscriptions follow that lifecycle, while views manage their animation visibility. Touched classes use smaller methods and clearer responsibilities.

`JsonSaveStorage` provides shared storage under `Application.persistentDataPath`, with separate `board.json`, `figures.json`, and `score.json` files. Domain adapters handle each data format. Figures retain their original slots, including empty slots, when restored. New systems can reuse the same storage, concentrating future persistence changes in one place.

**Tools > Bludoku > Hot Actions > Clear Save Files** deletes these three files, including the best score. The action is available outside Play Mode and preserves unrelated files and settings.

## Assumptions and limitations

Second Chance is treated as the existing power-up, and extra combo score as the bonus. Analytics uses Console logging. Previous score and figure PlayerPrefs data is not migrated. On restart, the saved booster flag restores combo activation at count two; the exact streak and grace usage are not persisted.

## Further development

- Consider VContainer as dependencies grow, to simplify composition and isolated testing.
- Add an application state machine for boot, loading, play, game over, and reset transitions.
- Refine VFX timing and composition, and explore custom 2D shaders.
- Persist the complete combo state if exact session restoration becomes a requirement.

## Running and Android delivery

Open the project in **Unity 2022.3.62f3**. For direct gameplay review, open `Assets/_Bludoku/Scenes/GameScene 1.unity` and enter Play Mode.

The APK is available locally at `Builds/UnityTestTask.apk` (build outputs are ignored by Git). It uses IL2CPP, supports ARMv7 and ARM64, and is signed with Unity's debug key for installation and testing. The Android build, package metadata, and signature have been verified; physical device testing is pending.

To reproduce it, select Android in Build Settings, keep **MainMenu → GameScene 1** enabled in that order, leave **Build App Bundle** and **Export Project** disabled, and select **Build**. Use Unity's bundled Android tools and leave **Custom Keystore** disabled. The deprecated `GameScene.unity` is excluded from the build.
