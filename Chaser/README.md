### Chaser.cs (Version 5.2)

*   **Inherits From:** `KCAlgoBase`

*   **Overall Purpose:**
    *   This strategy aims to "chase" or follow short-term price direction momentum as indicated by the **Linear Regression Curve** indicator. It enters trades when the curve shows consistent movement over the last few bars.

*   **Entry Logic (Specific Conditions):**
    *   This strategy defines its specific entry signals using a `LinReg` (Linear Regression Curve) indicator with a configurable `LinRegPeriod`.
    *   It relies on the **`KCAlgoBase`** for the final entry decision, meaning trades are only placed if the base class conditions are also met (e.g., `uptrend`/`downtrend` check, time filters, PnL limits, chop filter are active).
    *   **Long Entry (`ValidateEntryLong`):**
        *   Returns `true` if the Linear Regression curve value was **rising** for the past 3 bars (`LinReg1[1] > LinReg1[2] && LinReg1[2] > LinReg1[3]`). This indicates accelerating upward momentum.
        *   The actual `EnterLongPosition()` is only called by `KCAlgoBase` if this condition AND the base class `uptrend` condition (and other base checks) are met.
    *   **Short Entry (`ValidateEntryShort`):**
        *   Returns `true` if the Linear Regression curve value was **falling** for the past 3 bars (`LinReg1[1] < LinReg1[2] && LinReg1[2] < LinReg1[3]`). This indicates accelerating downward momentum.
        *   The actual `EnterShortPosition()` is only called by `KCAlgoBase` if this condition AND the base class `downtrend` condition (and other base checks) are met.

*   **Exit Logic:**
    *   Primarily relies on **`KCAlgoBase`** for all exits:
        *   Initial Stop Loss (`InitialStop` parameter in base).
        *   Trailing Stop (based on `TrailStopType` parameter in base - Tick, RegChan, ATR, 3-Step).
        *   Breakeven (`BESetAuto` logic in base).
        *   Profit Target (based on `EnableFixedProfitTarget`, `EnableRegChanProfitTarget`, `EnableDynamicProfitTarget` parameters in base).
        *   Session Close (`IsExitOnSessionCloseStrategy` in base).
        *   KillSwitch (Daily P/L limits, Trailing Drawdown in base).
    *   This strategy **does not** implement its own specific exit signals via `ValidateExitLong`/`ValidateExitShort` beyond checking the `enableExit` flag from the base class. If `enableExit` is turned on in the parameters, it seems intended to trigger an immediate exit, but the base class currently uses these methods mainly for *auto* signal exits, which Chaser doesn't define conditions for. Practically, exits are handled by the base class stop/target/limit mechanisms.

*   **Risk Management:**
    *   Fully managed by **`KCAlgoBase`** (Initial Stop, Trailing options, BE, Daily P/L limits, Trailing Drawdown, KillSwitch). This strategy adds no specific risk parameters of its own.

*   **Key Parameters (Added by this Strategy):**
    *   `LinRegPeriod`: (Integer) The lookback period for the Linear Regression Curve indicator.

*   **Best Time/Conditions (Inferred):**
    *   Like all strategies using `KCAlgoBase`, it benefits from **trending markets** and **non-choppy conditions** due to the base class filters.
    *   Specifically, the `LinReg` logic seeks **accelerating momentum**. It might perform well during strong, smooth directional moves but could be prone to whipsaws if momentum quickly reverses or stalls.
    *   The reliance on the last 3 bars of the `LinReg` makes it quite sensitive to recent price action.
    *   **Disclaimer:** Effectiveness is highly dependent on the chosen `LinRegPeriod` and the overall market character. **Requires thorough backtesting and optimization.**

*   **Notes:**
    *   This is a relatively simple signal layer built on the complex `KCAlgoBase` framework.
    *   Its performance hinges on whether the 3-bar Linear Regression slope is a good predictor of continued movement *and* aligns with the broader trend defined by the base class's indicator suite.
    *   The strategy name "Chaser" aptly describes its attempt to jump onto recent, accelerating price moves.
