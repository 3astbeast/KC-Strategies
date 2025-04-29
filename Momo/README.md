### Momo.cs (Version 5.2)

*   **Inherits From:** `KCAlgoBase`

*   **Overall Purpose:**
    *   This strategy attempts to capture strong directional price movements by entering trades when the **`Momentum`** indicator crosses a defined threshold (`MomoUp`/`MomoDown`) and is also accelerating in that direction compared to the previous bar.

*   **Entry Logic (Specific Conditions):**
    *   This strategy defines its specific entry signals using the standard `Momentum` indicator (with a fixed period of 14 set in `InitializeIndicators`) and compares its value against the `MomoUp` and `MomoDown` parameters.
    *   It relies on the **`KCAlgoBase`** for the final entry decision, meaning trades are only placed if the base class conditions are also met (e.g., `uptrend`/`downtrend` check, time filters, PnL limits, chop filter are active).
    *   **Long Entry (`ValidateEntryLong`):**
        *   Returns `true` if the current `Momentum` value (`Momentum1[0]`) is **above** the `MomoUp` threshold AND is **greater than** the `Momentum` value of the previous bar (`Momentum1[1]`). This requires both strong positive momentum and *increasing* positive momentum.
        *   The actual `EnterLongPosition()` is only called by `KCAlgoBase` if this condition AND the base class `uptrend` condition (and other base checks) are met.
    *   **Short Entry (`ValidateEntryShort`):**
        *   Returns `true` if the current `Momentum` value (`Momentum1[0]`) is **below** the `MomoDown` threshold (a negative value) AND is **less than** the `Momentum` value of the previous bar (`Momentum1[1]`). This requires both strong negative momentum and *increasing* negative momentum (i.e., becoming more negative).
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
    *   `MomoUp`: (Integer) The positive threshold the Momentum indicator must cross above (while increasing) to signal a potential long entry.
    *   `MomoDown`: (Integer, typically negative) The negative threshold the Momentum indicator must cross below (while decreasing) to signal a potential short entry.
    *   *(Note: The `Momentum` indicator period is fixed at 14 within this strategy's code (`InitializeIndicators`) and is not exposed as a user-configurable parameter.)*

*   **Best Time/Conditions (Inferred):**
    *   Like all strategies using `KCAlgoBase`, it requires **trending markets** and **non-choppy conditions** identified by the base class filters.
    *   Specifically designed to enter during periods of **strong, accelerating momentum**. It aims to catch powerful impulse moves rather than slow grinds or reversals.
    *   May be prone to entering late in a move if acceleration only occurs after a significant price change, or getting whipsawed if momentum spikes above/below the threshold but quickly reverses without follow-through.
    *   **Disclaimer:** Performance heavily depends on setting appropriate `MomoUp` and `MomoDown` levels for the instrument's volatility and timeframe. The fixed Momentum period (14) limits adaptability without code changes. **Requires thorough backtesting and optimization.**

*   **Notes:**
    *   A straightforward momentum breakout/acceleration strategy built on the `KCAlgoBase`.
    *   The requirement for momentum to be *both* above/below a threshold *and* increasing/decreasing compared to the prior bar adds a filter for accelerating moves.
    *   The Momentum indicator period (14) is hardcoded; changing it requires editing the `InitializeIndicators` method.
