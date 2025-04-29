### Vima.cs (Version 5.2)

*   **Inherits From:** `KCAlgoBase`

*   **Overall Purpose:**
    *   This strategy generates entry signals based solely on the direction (slope) of the **Volume Moving Average (`VMA`)** indicator calculated within the `KCAlgoBase` framework.

*   **Entry Logic (Specific Conditions):**
    *   This strategy does **not** initialize its own indicators. It directly uses the `volMaUp` and `volMaDown` boolean flags that are calculated by the `KCAlgoBase` during its `OnBarUpdate` cycle.
    *   These base class flags are typically derived from a `VMA` indicator (e.g., `VMA(Close, 9, 9)`) initialized within `KCAlgoBase`.
    *   It relies on the **`KCAlgoBase`** for the final entry decision, meaning trades are only placed if the base class conditions are also met (e.g., `uptrend`/`downtrend` check, time filters, PnL limits, chop filter are active, and importantly, the `enableVMA` parameter in the base must be true).
    *   **Long Entry (`ValidateEntryLong`):**
        *   Returns `true` if `longSignal` is true.
        *   `longSignal` is set directly to the `volMaUp` flag from `KCAlgoBase`. (`volMaUp` is true if the VMA is rising: `VMA1[0] > VMA1[1]`).
        *   The actual `EnterLongPosition()` is only called by `KCAlgoBase` if this condition AND the base class `uptrend` condition (and other base checks) are met.
    *   **Short Entry (`ValidateEntryShort`):**
        *   Returns `true` if `shortSignal` is true.
        *   `shortSignal` is set directly to the `volMaDown` flag from `KCAlgoBase`. (`volMaDown` is true if the VMA is falling: `VMA1[0] < VMA1[1]`).
        *   The actual `EnterShortPosition()` is only called by `KCAlgoBase` if this condition AND the base class `downtrend` condition (and other base checks) are met.
    *   *(Note: Commented-out code suggests a potential previous version required confirmation from the current bar's direction (`Close[0] > Open[0]` or `Close[0] < Open[0]`), but this is not active in the current logic.)*

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
    *   Fully managed by **`KCAlgoBase`** (Initial Stop, Trailing options, BE, Daily P/L limits, Trailing Drawdown, KillSwitch).

*   **Key Parameters (Added by this Strategy):**
    *   **None.** This strategy adds no parameters of its own. Its behavior related to the VMA signal is controlled by the `enableVMA` parameter within `KCAlgoBase`. The VMA indicator's periods appear fixed at (9, 9) within the `KCAlgoBase`.

*   **Best Time/Conditions (Inferred):**
    *   Requires **trending, non-choppy markets** filtered by `KCAlgoBase`, within allowed **time windows** and **risk limits**.
    *   Specifically looks for entries where the **Volume Moving Average slope aligns** with the broader trend identified by the base class.
    *   May perform well when volume trends precede or confirm price trends. Could struggle if VMA signals diverge from price or if the VMA slope changes frequently during consolidation before the base class filter engages.
    *   **Disclaimer:** Performance depends on the effectiveness of the VMA(9,9) slope as a confirming signal within the context of the overall `KCAlgoBase` trend definition and filters. **Requires thorough backtesting and optimization** of the `KCAlgoBase` parameters.

*   **Notes:**
    *   This is a very minimalistic strategy layer that isolates the VMA slope signal from the `KCAlgoBase` framework.
    *   It doesn't initialize any indicators itself; it consumes the VMA state provided by the base class.
    *   Its effectiveness is tightly coupled to the VMA indicator's settings (fixed at 9,9 in the base) and the overall filtering logic within `KCAlgoBase`.
