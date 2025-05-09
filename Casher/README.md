### Casher.cs (Version 5.4.4)

*   **Please note this strategy requires the installation of the HiLoBands indicator, which is also included as a download on this page**

*   **Inherits From:** `KCAlgoBase`

*   **Overall Purpose:**
    *   This strategy aims to enter trades based on the direction of the midline of the **`HiLoBands`** indicator, confirmed by accelerating **`Momentum`**.
    *   A key feature is its **dynamic calculation of `InitialStop` and `ProfitTarget`** just before entry, based on the `HiLoBands` (for the stop) and a specified `RiskToReward` ratio (for the target).

*   **Entry Logic (Specific Conditions):**
    *   Signals are derived from the `HiLoBands` indicator (specifically its midline) and the standard `Momentum` indicator (fixed 14 period).
    *   It relies on the **`KCAlgoBase`** for the final entry decision, meaning trades are only placed if the base class conditions are also met (e.g., `uptrend`/`downtrend` check, time filters, PnL limits, chop filter are active).
    *   **Dynamic Stop/Target Calculation (Pre-Entry):**
        *   If `isFlat` (no current position) and a `longSignal` or `shortSignal` is generated:
            *   **Long:** `InitialStop` (in ticks) is calculated based on the distance from the current `Close[0]` to the `lowestLow[1]` (lower HiLoBand of the previous bar), minus a `TrailOffset`. The `ProfitTarget` is then set to `InitialStop` * `RiskRewardRatio`.
            *   **Short:** `InitialStop` (in ticks) is calculated based on the distance from the `highestHigh[1]` (upper HiLoBand of the previous bar) to the current `Close[0]`, plus a `TrailOffset`. The `ProfitTarget` is then set to `InitialStop` * `RiskRewardRatio`.
        *   This means the strategy dynamically adjusts its initial risk and reward based on recent volatility as captured by the HiLoBands. These calculated values will override the `InitialStop` and `ProfitTarget` defaults set in `KCAlgoBase` for that specific trade.
    *   **Long Entry (`ValidateEntryLong`):**
        *   Returns `true` if `longSignal` is true.
        *   `longSignal` conditions:
            *   The midline of `HiLoBands` has turned up (either `midline[0] > midline[1] && midline[1] <= midline[2]`) **OR** is simply rising (`midline[0] > midline[1]`).
            *   **AND** `Momentum` is accelerating upwards (`Momentum1[0] > Momentum1[1]`).
        *   The actual `EnterLongPosition()` is only called by `KCAlgoBase` if these conditions AND the base class `uptrend` condition (and other base checks) are met.
    *   **Short Entry (`ValidateEntryShort`):**
        *   Returns `true` if `shortSignal` is true.
        *   `shortSignal` conditions:
            *   The midline of `HiLoBands` has turned down (either `midline[0] < midline[1] && midline[1] >= midline[2]`) **OR** is simply falling (`midline[0] < midline[1]`).
            *   **AND** `Momentum` is accelerating downwards (`Momentum1[0] < Momentum1[1]`).
        *   The actual `EnterShortPosition()` is only called by `KCAlgoBase` if these conditions AND the base class `downtrend` condition (and other base checks) are met.
    *   *(Note: More complex, commented-out signal logic suggests previous versions involved direct checks against the HiLoBands themselves, price action, and Momentum levels, but the current active logic is simpler.)*

*   **Exit Logic:**
    *   **Initial Stop and Profit Target:** As described above, these are **dynamically calculated** by `Casher` just before entry and then managed by `KCAlgoBase`.
    *   **Other Exits:** Relies on **`KCAlgoBase`** for:
        *   Trailing Stop (if `enableTrail` is true in base, it will use the dynamically calculated `InitialStop` as its starting point if `Tick_Trail` is selected, or its own logic for other trail types).
        *   Breakeven (`BESetAuto` logic in base).
        *   Session Close (`IsExitOnSessionCloseStrategy` in base).
        *   KillSwitch (Daily P/L limits, Trailing Drawdown in base).
    *   This strategy **does not** implement its own specific exit signals via `ValidateExitLong`/`ValidateExitShort` beyond checking the `enableExit` flag from the base class (likely enabling a reverse-on-opposite-signal if `enableExit` is true, but inactive by default).

*   **Risk Management:**
    *   While `KCAlgoBase` manages the execution of risk rules, `Casher` takes an active role by **dynamically setting the initial risk (stop loss) and potential reward (profit target) per trade** based on market volatility (via HiLoBands) and the `RiskToReward` ratio.
    *   Other risk aspects (Daily P/L, Trailing Drawdown, BE) are managed by `KCAlgoBase`.

*   **Key Parameters (Added by this Strategy):**
    *   `RiskToReward`: (Double) The desired risk-to-reward ratio for calculating the profit target based on the dynamically set initial stop.
    *   `LookbackPeriod`: (Integer) The lookback period for the `HiLoBands` indicator.
    *   `Width`: (Integer) The width/multiplier for the `HiLoBands` calculation.
    *   `showHighLow`: (Boolean) Toggles visibility of the `HiLoBands` on the chart.
    *   `showMomo`: (Boolean) Toggles visibility of the `Momentum` indicator on the chart.
    *   `TrailOffset`: (Integer) Number of ticks to adjust the dynamically calculated stop loss (e.g., to place it slightly further or closer than the raw HiLoBand level).

*   **Best Time/Conditions (Inferred):**
    *   Like all strategies using `KCAlgoBase`, it should perform better in **trending, non-choppy markets** filtered by the base class, within specified **time windows** and **risk limits**.
    *   The dynamic stop/target based on `HiLoBands` suggests it attempts to adapt to recent volatility. It might perform well when the HiLoBands provide meaningful support/resistance levels for stop placement.
    *   The combination of HiLoBands midline direction and accelerating Momentum aims to catch entries as a trend (defined by the midline) gains momentum.
    *   **Disclaimer:** The effectiveness of dynamically setting stops/targets based on HiLoBands and a fixed R:R ratio needs thorough validation. Performance will depend on tuning `LookbackPeriod`, `Width`, `RiskToReward`, `TrailOffset` and the `KCAlgoBase` settings. **Requires thorough backtesting and optimization.**

*   **Notes:**
    *   The dynamic calculation of `InitialStop` and `ProfitTarget` is a significant feature, making this strategy more adaptive to market volatility than those using fixed tick-based stops/targets.
    *   It disables several indicators from `KCAlgoBase` by default in `SetDefaults` (HMA, VMA, RegChan), suggesting it primarily relies on its own HiLoBands/Momentum logic and the base class's ADX/BuySellPressure/ATR for overall trend context.
    *   The Momentum indicator's period is fixed at 14 in `InitializeIndicators`.
