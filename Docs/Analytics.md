# Analytics

`GameController` creates a `ConsoleAnalyticsProvider` during initialization and exposes it through `IAnalyticsProvider Analytics`. `FiguresController` and `ScoreMediator` receive that interface through their initialization methods.

`AnalyticsEvents` contains the event-name constants. `AnalyticsEvent` contains a name and parameters, with fluent `Add` overloads for strings, integers (`long`), and decimal numbers (`double`). Gameplay has no Firebase dependency.

| Event | Trigger | Parameters |
| --- | --- | --- |
| `piece_moved` | A piece is placed or returned after release | `figure_id`, `outcome` |
| `bonus_received` | A combo awards extra score | `bonus_type`, `combo_count`, `base_score`, `bonus_score`, `score` |
| `power_up_used` | Second Chance replaces the available pieces | `power_up_type`, `score`, `combo_count` |
| `combo_broken` | An active combo breaks during gameplay | `combo_count`, `grace_moves_used` |
| `game_over` | No available piece can be placed | `score`, `high_score`, `combo_count` |
| `game_started` | Initial gameplay begins or New Game finishes resetting | `source`, `score`, `combo_count` |

Second Chance is treated as the existing power-up for this assignment because it replaces the available pieces with easier pieces. There is no separate consumable power-up inventory. The combo score reward is the existing bonus.

`game_started.source` is `restored` when a board save can be loaded at startup, and `new` otherwise or after New Game. Restoring a combo, refreshing UI, and resetting a combo do not emit bonus or combo-break events. Moves are tracked at release rather than on every drag update.

Bonus amounts use the actual score difference and the existing base reward calculation. A nine-point clear with the combo multiplier awards thirteen points after rounding, so `bonus_score` is four.

## Add an event

Add a constant to `AnalyticsEvents`, then track it where the gameplay action succeeds:

```csharp
analytics.Track(new AnalyticsEvent(AnalyticsEvents.BonusReceived)
    .Add("bonus_type", "combo")
    .Add("combo_count", state.Count)
    .Add("bonus_score", bonusScore));
```

Add another `.Add(...)` call to extend its payload. The provider accepts new event names and parameters without a registration list or event-specific dispatch. Create a new event object for each call.

## Add a provider

Implement `IAnalyticsProvider.Track`, then replace the provider created in `GameController.InitializeGame`. A future Firebase implementation can map event names and scalar parameters to Firebase's logging API. It should own SDK initialization, provider-specific restrictions, and delivery failures. Asynchronous providers should snapshot the event before retaining it.

The Console provider logs each event and all its parameters with invariant numeric formatting. Real Firebase delivery is outside this implementation; the assignment permits Console logging or a mock provider.
