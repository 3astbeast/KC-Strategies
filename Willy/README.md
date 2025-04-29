### Willy.cs (Version 5.2)

*   **Inherits From:** `KCAlgoBase`

*   **Overall Purpose:**
    *   This strategy uses a custom Williams %R based indicator (`TOWilliamsTraderOracleSignalMOD`) combined with specific price action conditions to generate entry signals, likely aiming to identify potential exhaustion or reversal points near overbought/oversold levels.

*   **Entry Logic (Specific Conditions):**
    *   This strategy defines its specific entry signals using the `TOWilliamsTraderOracleSignalMOD` indicator (initialized with a fixed period of 14 in code) and compares its value (`WilliamsR1[1]`) from the **previous bar** against the `wrUp` and `wrDown` parameters.
    *   It also includes **confirming price action conditions** on the current and previous bars.
    *   It relies on the **`KCAlgoBase`** for the final entry decision, meaning trades are only placed if the base class conditions are also met (e.g., `uptrend`/`downtrend` check, time filters, PnL limits, chop filter are active).
    *   **Long Entry (`ValidateEntryLong`):**
        *   Returns `true` if all three conditions are met:
            *   Williams %R on the prior bar was **at or above** the `wrUp` threshold (`WilliamsR1[1] >= wrUp`). Williams %R typically ranges from -100 (oversold) to 0 (overbought), so `wrUp` is usually a value like -20 or -10, indicating the market was recently near overbought levels.
            *   Current bar close is **higher** than the previous bar close (`Close[0] > Close[1]`).
            *   The high of the previous bar was **higher** than the high of the bar before that (`High[1] > High[2]`).
        *   This logic seems counter-intuitive for a standard overbought/reversal signal (which would typically look for a *short* when W%R is high). This might be looking for a sign of strength or a breakout *after* reaching near overbought levels, confirmed by immediate price action. Careful analysis and testing are needed to understand the intended pattern.
        *   The actual `EnterLongPosition()` is only called by `KCAlgoBase` if these conditions AND the base class `uptrend` condition (and other base checks) are met.
    *   **Short Entry (`ValidateEntryShort`):**
        *   Returns `true` if all three conditions are met:
            *   Williams %R on the prior bar was **at or below** the `wrDown` threshold (`WilliamsR1[1] <= wrDown`). `wrDown` is typically a value like -80 or -90, indicating the market was recently near oversold levels.
            *   Current bar close is **lower** than the previous bar close (`Close[0] < Close[1]`).
            *   The low of the previous bar was **lower** than the low of the bar before that (`Low[1] < Low[2]`).
        *   This logic, similar to the long side, seems to look for confirmation of weakness *after* reaching near oversold levels, rather than a direct reversal entry.
        *   The actual `EnterShortPosition()` is only called by `KCAlgoBase` if these conditions AND the base class `downtrend` condition (and other base checks) are met.

*   **Exit Logic:**
    *   Primarily relies on **`KCAlgoBase`** for all standard exits:
        *   Initial Stop Loss (`InitialStop` parameter in base).
        *   Trailing Stop (based on `TrailStopType` parameter in base - Tick, RegChan, ATR, 3-Step).
        *   Breakeven (`BESetAuto` logic in base).
        *   Profit Target (based on `EnableFixedProfitTarget`, `EnableRegChanProfitTarget`, `EnableDynamicProfitTarget` parameters in base).
        *   Session Close (`IsExitOnSessionCloseStrategy` in base).
        *   KillSwitch (Daily P/L limits, Trailing Drawdown in base).
    *   This strategy **does not** implement its own specific exit signals via `ValidateExitLong`/`ValidateExitShort` beyond checking the `enableExit` flag from the base class (likely enabling a reverse-on-opposite-signal if `enableExit` is true, but inactive by default).

*   **Risk Management:**
    *   Fully managed by **`KCAlgoBase`** (Initial Stop, Trailing options, BE, Daily P/L limits, Trailing Drawdown, KillSwitch). This strategy adds no specific risk parameters of its own.

*   **Key Parameters (Added by this Strategy):**
    *   `wrPeriod`: (Integer) The lookback period for the `TOWilliamsTraderOracleSignalMOD` indicator. (Note: This parameter exists but the indicator is initialized with a fixed period of 14 in the code, so this parameter might not currently have an effect unless the initialization code is changed).
    *   `wrUp`: (Integer, typically negative, e.g., -20) The upper threshold for the Williams %R indicator component used in the long entry condition.
    *   `wrDown`: (Integer, typically negative, e.g., -80) The lower threshold for the Williams %R indicator component used in the short entry condition.

*   **Best Time/Conditions (Inferred):**
    *   Relies on **`KCAlgoBase`** filtering for **trending, non-choppy markets**, specific **time windows**, and **risk limits**.
    *   The strategy attempts to identify entry points after the market has reached potentially overbought/oversold levels (based on Williams %R) but requires immediate price action confirmation (`Close[0]` vs `Close[1]`, `High[1]` vs `High[2]` or `Low[1]` vs `Low[2]`).
    *   The specific pattern it's trying to capture (entering *after* W%R extremes *with* price confirmation) is somewhat unusual compared to standard oscillator reversal strategies. It might be looking for failed reversals or continuations after hitting extremes.
    *   **Disclaimer:** The fixed indicator period (14) and the non-standard use of Williams %R signals require careful validation. Performance depends heavily on tuning the `wrUp`/`wrDown` thresholds and the effectiveness of the price action confirmation rules. **Requires thorough backtesting and optimization.**

*   **Notes:**
    *   Uses a custom Williams %R indicator (`TOWilliamsTraderOracleSignalMOD`).
    *   The entry logic combines oscillator levels with specific bar-over-bar price action patterns.
    *   The `wrPeriod` parameter might be redundant as the indicator is initialized with a fixed period of 14. Verify this if modifications are considered.
    *   Relies entirely on `KCAlgoBase` for exits and risk control.
