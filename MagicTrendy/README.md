### MagicTrendy.cs (Version 5.2)

*   **Inherits From:** `KCAlgoBase`

*   **Overall Purpose:**
    *   This strategy follows the trend indicated by the **`TrendMagic`** indicator. It generates entry signals based purely on the change in the `TrendMagic` indicator's value from the previous bar to the one before it.

*   **Entry Logic (Specific Conditions):**
    *   This strategy defines its specific entry signals using the `TrendMagic` indicator.
    *   It uses a fixed `cciPeriod` of 20 and a fixed `atrPeriod` of 14 for the `TrendMagic` calculation (these are set in `SetDefaults` and not exposed as parameters in this file). The ATR multiplier (`atrMult`) *is* exposed as a parameter.
    *   It relies on the **`KCAlgoBase`** for the final entry decision, meaning trades are only placed if the base class conditions are also met (e.g., `uptrend`/`downtrend` check, time filters, PnL limits, chop filter are active).
    *   **Long Entry (`ValidateEntryLong`):**
        *   Returns `true` if the `TrendMagic` value on the *prior bar close* (`[1]`) was **greater than** its value on the bar before that (`[2]`). This indicates the indicator started moving up on the last completed bar.
        *   The actual `EnterLongPosition()` is only called by `KCAlgoBase` if this condition AND the base class `uptrend` condition (and other base checks) are met.
    *   **Short Entry (`ValidateEntryShort`):**
        *   Returns `true` if the `TrendMagic` value on the *prior bar close* (`[1]`) was **less than** its value on the bar before that (`[2]`). This indicates the indicator started moving down on the last completed bar.
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
    *   `atrMult`: (Double) The ATR multiplier used within the `TrendMagic` indicator calculation. Controls the sensitivity/offset of the TrendMagic line.
    *   *(Note: The `cciPeriod` (20) and `atrPeriod` (14) for the `TrendMagic` indicator are currently fixed within this strategy's code and not exposed as user-configurable parameters.)*

*   **Best Time/Conditions (Inferred):**
    *   Like all strategies using `KCAlgoBase`, benefits from **trending markets** and **non-choppy conditions** due to the base class filters.
    *   The strategy is designed to enter trades as soon as the `TrendMagic` indicator signals a potential trend shift based on its slope change.
    *   Performance will depend heavily on how well the `TrendMagic` indicator (with its fixed periods and adjustable multiplier) captures the prevailing trend on the chosen instrument and timeframe, and how well it avoids generating signals during sideways movement that isn't filtered out by the base class.
    *   **Disclaimer:** Effectiveness is highly dependent on tuning the `atrMult` parameter and the underlying `KCAlgoBase` settings. The fixed internal periods (20, 14) limit adaptability without code changes. **Requires thorough backtesting and optimization.**

*   **Notes:**
    *   The core signal logic is very direct: follow the slope of the `TrendMagic` indicator from the previous bar.
    *   The reliance on fixed `cciPeriod` and `atrPeriod` for `TrendMagic` is a significant characteristic – tuning the indicator's core behavior requires modifying the code, not just parameters. Only the ATR multiplier offset is adjustable via parameters.
