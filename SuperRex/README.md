### SuperRex.cs (Version 5.2)

*   **Inherits From:** `KCAlgoBase`

*   **Overall Purpose:**
    *   This strategy uses the **`CMO`** (Chande Momentum Oscillator) indicator to generate entry signals. It enters trades when the CMO value crosses above a specified upper threshold (`CmoUp`) or below a specified lower threshold (`CmoDown`).

*   **Entry Logic (Specific Conditions):**
    *   This strategy defines its specific entry signals using the standard `CMO` indicator (with a fixed period of 14 set in `InitializeIndicators`) and compares its value against the `CmoUp` and `CmoDown` parameters.
    *   It relies on the **`KCAlgoBase`** for the final entry decision, meaning trades are only placed if the base class conditions are also met (e.g., `uptrend`/`downtrend` check, time filters, PnL limits, chop filter are active).
    *   **Long Entry (`ValidateEntryLong`):**
        *   Returns `true` if the current `CMO` value (`CMO1[0]`) is **greater than or equal to** the `CmoUp` threshold.
        *   The actual `EnterLongPosition()` is only called by `KCAlgoBase` if this condition AND the base class `uptrend` condition (and other base checks) are met.
    *   **Short Entry (`ValidateEntryShort`):**
        *   Returns `true` if the current `CMO` value (`CMO1[0]`) is **less than or equal to** the `CmoDown` threshold (a negative value).
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
    *   `CmoUp`: (Integer) The positive threshold the CMO indicator must cross above (or equal) to signal a potential long entry.
    *   `CmoDown`: (Integer, typically negative) The negative threshold the CMO indicator must cross below (or equal) to signal a potential short entry.
    *   *(Note: The `CMO` indicator period is fixed at 14 within this strategy's code (`InitializeIndicators`) and is not exposed as a user-configurable parameter.)*

*   **Best Time/Conditions (Inferred):**
    *   Like all strategies using `KCAlgoBase`, requires **trending markets** and **non-choppy conditions** identified by the base class filters for optimal performance.
    *   This strategy specifically looks for strong momentum readings via the CMO crossing defined thresholds. It might perform well during strong trending phases where CMO reaches relatively high or low levels.
    *   However, using fixed thresholds on an oscillator like CMO can be challenging. In choppy markets not filtered out by the base, CMO might oscillate across the `CmoUp`/`CmoDown` levels frequently, leading to whipsaws. The effectiveness depends on whether these threshold crossings align with sustained directional moves.
    *   **Disclaimer:** Performance heavily depends on setting appropriate `CmoUp` and `CmoDown` levels for the instrument's volatility and timeframe. The fixed CMO period (14) limits adaptability without code changes. **Requires thorough backtesting and optimization.**

*   **Notes:**
    *   A simple strategy layer focused solely on the CMO indicator for signals.
    *   The CMO period (14) is hardcoded; changing it requires editing the `InitializeIndicators` method.
    *   Relies entirely on `KCAlgoBase` for exits and risk control.
