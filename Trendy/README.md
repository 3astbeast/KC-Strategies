### Trendy.cs (Version 5.2)

*   **Inherits From:** `KCAlgoBase`

*   **Overall Purpose:**
    *   This strategy uses the **`T3TrendFilter`** indicator to identify the prevailing trend and generate entry signals based on the strength and direction indicated by its output values.

*   **Entry Logic (Specific Conditions):**
    *   This strategy defines its specific entry signals using the `T3TrendFilter` indicator. The calculation of this indicator depends on several parameters (`Factor`, `Period1` through `Period5`).
    *   The strategy accesses two output values from the indicator: `T3TrendFilter1.Values[0][0]` (assigned to `trendyUp`) and `T3TrendFilter1.Values[1][0]` (assigned to `trendyDown`). These likely represent the strength of the upward and downward trend components, respectively.
    *   It relies on the **`KCAlgoBase`** for the final entry decision, meaning trades are only placed if the base class conditions are also met (e.g., `uptrend`/`downtrend` check, time filters, PnL limits, chop filter are active).
    *   **Long Entry (`ValidateEntryLong`):**
        *   Returns `true` if the "up-trend" value (`trendyUp`) is **greater than or equal to 5** AND the "down-trend" value (`trendyDown`) is **exactly 0**. This signifies a clear and strong upward trend signal according to the indicator.
        *   The actual `EnterLongPosition()` is only called by `KCAlgoBase` if this condition AND the base class `uptrend` condition (and other base checks) are met.
    *   **Short Entry (`ValidateEntryShort`):**
        *   Returns `true` if the "down-trend" value (`trendyDown`) is **less than or equal to -5** AND the "up-trend" value (`trendyUp`) is **exactly 0**. This signifies a clear and strong downward trend signal according to the indicator.
        *   The actual `EnterShortPosition()` is only called by `KCAlgoBase` if this condition AND the base class `downtrend` condition (and other base checks) are met.

*   **Exit Logic:**
    *   Primarily relies on **`KCAlgoBase`** for all standard exits:
        *   Initial Stop Loss (`InitialStop` parameter in base).
        *   Trailing Stop (based on `TrailStopType` parameter in base - Tick, RegChan, ATR, 3-Step).
        *   Breakeven (`BESetAuto` logic in base).
        *   Profit Target (based on `EnableFixedProfitTarget`, `EnableRegChanProfitTarget`, `EnableDynamicProfitTarget` parameters in base).
        *   Session Close (`IsExitOnSessionCloseStrategy` in base).
        *   KillSwitch (Daily P/L limits, Trailing Drawdown in base).
    *   This strategy **does not** implement its own specific exit signals via `ValidateExitLong`/`ValidateExitShort` beyond checking the `enableExit` flag from the base class (similar to other strategies, this likely enables a reverse-on-opposite-signal if `enableExit` is true, but is inactive by default).

*   **Risk Management:**
    *   Fully managed by **`KCAlgoBase`** (Initial Stop, Trailing options, BE, Daily P/L limits, Trailing Drawdown, KillSwitch). This strategy adds no specific risk parameters of its own.

*   **Key Parameters (Added by this Strategy):**
    *   Parameters for the `T3TrendFilter` indicator:
        *   `Factor`: (Double) Smoothing factor.
        *   `Period1`: (Integer) Lookback period.
        *   `Period2`: (Integer) Lookback period.
        *   `Period3`: (Integer) Lookback period.
        *   `Period4`: (Integer) Lookback period.
        *   `Period5`: (Integer) Lookback period.
    *   *(Consult the documentation for the `T3TrendFilter` indicator itself to understand how these parameters affect its calculation).*

*   **Best Time/Conditions (Inferred):**
    *   Like all strategies using `KCAlgoBase`, benefits from **trending markets** and **non-choppy conditions** as identified by the base class filters.
    *   This strategy specifically aims to enter when the `T3TrendFilter` shows a clear, established trend (value >= 5 for longs, <= -5 for shorts) with no opposing signal present (other value is 0).
    *   It likely performs best during sustained, strong trends where the `T3TrendFilter` provides unambiguous signals. It might lag during trend initiations or generate false signals if the market becomes choppy in a way that causes the indicator values to fluctuate around zero without reaching the +/- 5 threshold clearly.
    *   **Disclaimer:** Performance depends heavily on tuning the `T3TrendFilter` parameters (`Factor`, `Period1`-`Period5`) to match the characteristics of the traded instrument and timeframe. **Requires thorough backtesting and optimization.**

*   **Notes:**
    *   A trend-following strategy based purely on the signals derived from the `T3TrendFilter` indicator.
    *   Relies entirely on `KCAlgoBase` for execution filtering, exits, and risk control.
