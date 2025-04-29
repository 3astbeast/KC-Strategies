### KhanhZilla.cs (Version 5.2)

*   **Inherits From:** `KCAlgoBase2`

*   **Overall Purpose:**
    *   This strategy attempts to identify potential reversal or pullback entries by looking for specific price action patterns occurring exactly at the upper or lower bands of the **`RegressionChannelHighLow`** indicator provided by the `KCAlgoBase2` framework.

*   **Entry Logic (Specific Conditions):**
    *   This strategy **does not initialize its own indicators**. It relies entirely on the indicators initialized within `KCAlgoBase2`, specifically using the public `RegressionChannelHighLow1` instance.
    *   The signal logic depends heavily on the `RegChanPeriod` and `RegChanWidth` parameters set in `KCAlgoBase2`, as these define the channel bands used.
    *   It relies on the **`KCAlgoBase2`** for the final entry decision, meaning trades are only placed if the base class conditions are also met (e.g., `uptrend`/`downtrend` check based on ADX/BuySellPressure/ATR, time filters, PnL limits, chop filter are active).
    *   **Long Entry (`ValidateEntryLong`):**
        *   Returns `true` if both conditions are met:
            *   The **previous bar's low** (`Low[1]`) **exactly matched** the lower band of the `RegressionChannelHighLow` indicator on that same bar (`RegressionChannelHighLow1.Lower[1]`).
            *   The **current bar's low** (`Low[0]`) is **strictly greater than** the lower band level from the previous bar (`RegressionChannelHighLow1.Lower[1]`).
        *   This identifies a specific "touch and bounce" pattern off the lower High/Low channel band.
        *   The actual `EnterLongPosition()` is only called by `KCAlgoBase2` if this pattern occurs AND the base class `uptrend` condition (and other base checks) are met.
    *   **Short Entry (`ValidateEntryShort`):**
        *   Returns `true` if both conditions are met:
            *   The **previous bar's high** (`High[1]`) **exactly matched** the upper band of the `RegressionChannelHighLow` indicator on that same bar (`RegressionChannelHighLow1.Upper[1]`).
            *   The **current bar's high** (`High[0]`) is **strictly less than** the upper band level from the previous bar (`RegressionChannelHighLow1.Upper[1]`).
        *   This identifies a specific "touch and reject" pattern off the upper High/Low channel band.
        *   The actual `EnterShortPosition()` is only called by `KCAlgoBase2` if this pattern occurs AND the base class `downtrend` condition (and other base checks) are met.

*   **Exit Logic:**
    *   Primarily relies on **`KCAlgoBase2`** for all standard exits:
        *   Initial Stop Loss (`InitialStop` parameter in base).
        *   Trailing Stop (based on `TrailStopType` parameter in base - Tick, RegChan, ATR, 3-Step). Note that `KCAlgoBase2` uses `RegressionChannelHighLow1` for its RegChan trail.
        *   Breakeven (`BESetAuto` logic in base).
        *   Profit Target (based on `EnableFixedProfitTarget`, `EnableRegChanProfitTarget`, `EnableDynamicProfitTarget` parameters in base). Note that `KCAlgoBase2` uses `RegressionChannelHighLow1` for its RegChan target mode.
        *   Session Close (`IsExitOnSessionCloseStrategy` in base).
        *   KillSwitch (Daily P/L limits, Trailing Drawdown in base).
    *   This strategy **does not** implement its own specific exit signals via `ValidateExitLong`/`ValidateExitShort` beyond checking the `enableExit` flag from the base class (likely enabling a reverse-on-opposite-signal if `enableExit` is true, but inactive by default).

*   **Risk Management:**
    *   Fully managed by **`KCAlgoBase2`** (Initial Stop, Trailing options, BE, Daily P/L limits, Trailing Drawdown, KillSwitch).

*   **Key Parameters (Added by this Strategy):**
    *   **None.** This strategy adds no new parameters. Its signaling behavior is entirely dependent on the parameters inherited from `KCAlgoBase2`, especially `RegChanPeriod` and `RegChanWidth` which control the `RegressionChannelHighLow1` indicator used for signals. It also sets different default values for some base parameters like `InitialStop` and `ProfitTarget` compared to the base.

*   **Best Time/Conditions (Inferred):**
    *   Relies on **`KCAlgoBase2`** filtering for **trending (based on ADX/Pressure/ATR), non-choppy markets**, specific **time windows**, and **risk limits**.
    *   The entry logic specifically looks for precise touches and immediate reversals away from the High/Low Regression Channel bands. This suggests it might be suited for markets where these bands act as strong, temporary support/resistance levels within a broader trend defined by the base.
    *   There's a potential conflict: the entry signal is somewhat mean-reverting (fading the touch of the band), while the `KCAlgoBase2` trend filter requires alignment (ADX/Pressure/ATR) with the trade direction. This might mean entries only trigger during pullbacks to the band within an established trend.
    *   The requirement for an *exact* match (`==`) between the prior bar's high/low and the band level is very strict and might lead to fewer signals compared to checking for crosses or touches within a small tolerance.
    *   **Disclaimer:** Performance depends heavily on how well the `RegressionChannelHighLow` bands (configured via `RegChanPeriod`/`Width` in the base) contain price action and whether the specific "touch-and-go" pattern is predictive. The interaction with the simplified base trend filter is crucial. **Requires thorough backtesting and optimization.**

*   **Notes:**
    *   Relies entirely on indicators initialized in `KCAlgoBase2`, most importantly `RegressionChannelHighLow1`.
    *   The entry condition is very specific, requiring an exact touch on the previous bar followed by a move away on the current bar.
    *   Exits are fully managed by the `KCAlgoBase2` framework.
