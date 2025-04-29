### KingKhanh.cs (Version 5.2)

*   **Inherits From:** `KCAlgoBase`

*   **Overall Purpose:**
    *   This strategy uses various conditions based on **Regression Channels** (`RegressionChannel` and `RegressionChannelHighLow` indicators) to identify potential entry points, likely looking for trend continuations or reversals near the channel boundaries or based on the channel's slope changes.

*   **Entry Logic (Specific Conditions):**
    *   This strategy defines its specific entry signals using Regression Channel indicators. The key parameters (`RegChanPeriod`, `RegChanWidth`, `RegChanWidth2`) are inherited from `KCAlgoBase` but might have different default values set here.
    *   It relies on the **`KCAlgoBase`** for the final entry decision, meaning trades are only placed if the base class conditions are also met (e.g., `uptrend`/`downtrend` check, time filters, PnL limits, chop filter are active).
    *   **Long Entry (`ValidateEntryLong`):**
        *   Returns `true` if **any** of the following three condition groups are met:
            *   **Group 1 (Slope Change):** The middle line of `RegressionChannel1` turned up 1 bar ago, after previously being flat or down (`RegressionChannel1.Middle[1] > RegressionChannel1.Middle[2] && RegressionChannel1.Middle[2] <= RegressionChannel1.Middle[3]`). Suggests a potential bottoming or start of an uptrend based on the channel's center.
            *   **Group 2 (Lower Band Bounce - Midline Rising):** The middle line of `RegressionChannel1` is currently rising (`RegressionChannel1.Middle[0] > RegressionChannel1.Middle[1]`), AND the current low is higher than the low 2 bars ago (`Low[0] > Low[2]`), AND the low 2 bars ago was at or below the *lower band* of `RegressionChannel1` (`Low[2] <= RegressionChannel1.Lower[2]`). Suggests a bounce off the lower channel boundary while the channel centerline is already moving up.
            *   **Group 3 (Lower Band Bounce - High/Low Channel):** The current low is above the lower band of the `RegressionChannelHighLow` indicator from 2 bars ago (`Low[0] > RegressionChannelHighLow1.Lower[2]`). A simpler condition potentially looking for price staying above the lower boundary defined by highs/lows.
        *   The actual `EnterLongPosition()` is only called by `KCAlgoBase` if one of these conditions AND the base class `uptrend` condition (and other base checks) are met.
    *   **Short Entry (`ValidateEntryShort`):**
        *   Returns `true` if **any** of the following three condition groups are met:
            *   **Group 1 (Slope Change):** The middle line of `RegressionChannel1` turned down 1 bar ago, after previously being flat or up (`RegressionChannel1.Middle[1] < RegressionChannel1.Middle[2] && RegressionChannel1.Middle[2] >= RegressionChannel1.Middle[3]`). Suggests a potential topping or start of a downtrend based on the channel's center.
            *   **Group 2 (Upper Band Bounce - Midline Falling):** The middle line of `RegressionChannel1` is currently falling (`RegressionChannel1.Middle[0] < RegressionChannel1.Middle[1]`), AND the current high is lower than the high 2 bars ago (`High[0] < High[2]`), AND the high 2 bars ago was at or above the *upper band* of `RegressionChannel1` (`High[2] >= RegressionChannel1.Upper[2]`). Suggests a rejection from the upper channel boundary while the channel centerline is already moving down.
            *   **Group 3 (Upper Band Bounce - High/Low Channel):** The current high is below the upper band of the `RegressionChannelHighLow` indicator from 2 bars ago (`High[0] < RegressionChannelHighLow1.Upper[2]`). A simpler condition potentially looking for price staying below the upper boundary defined by highs/lows.
        *   The actual `EnterShortPosition()` is only called by `KCAlgoBase` if one of these conditions AND the base class `downtrend` condition (and other base checks) are met.

*   **Exit Logic:**
    *   Primarily relies on **`KCAlgoBase`** for all standard exits:
        *   Initial Stop Loss (`InitialStop` parameter in base).
        *   Trailing Stop (based on `TrailStopType` parameter in base - Tick, RegChan, ATR, 3-Step). Note that this strategy might interact well with the `Regression_Channel_Trail` stop type in the base.
        *   Breakeven (`BESetAuto` logic in base).
        *   Profit Target (based on `EnableFixedProfitTarget`, `EnableRegChanProfitTarget`, `EnableDynamicProfitTarget` parameters in base). The `EnableRegChanProfitTarget` mode in the base might complement this strategy's entry logic.
        *   Session Close (`IsExitOnSessionCloseStrategy` in base).
        *   KillSwitch (Daily P/L limits, Trailing Drawdown in base).
    *   This strategy **does not** implement its own specific exit signals via `ValidateExitLong`/`ValidateExitShort` beyond checking the `enableExit` flag from the base class (similar to the Hooker strategy, this likely enables a reverse-on-opposite-signal if `enableExit` is true, but is inactive by default).

*   **Risk Management:**
    *   Fully managed by **`KCAlgoBase`** (Initial Stop, Trailing options, BE, Daily P/L limits, Trailing Drawdown, KillSwitch). This strategy adds no specific risk parameters of its own.

*   **Key Parameters (Added by this Strategy):**
    *   None. This strategy uses parameters inherited from `KCAlgoBase`, specifically `RegChanPeriod`, `RegChanWidth`, and `RegChanWidth2`. It sets default values for these which might differ from the base class defaults.

*   **Best Time/Conditions (Inferred):**
    *   As with others inheriting from `KCAlgoBase`, favors **trending markets** and **non-choppy conditions**.
    *   The entry logic specifically looks for signs of **trend continuation after pullbacks** (bouncing off channel bands) or **potential trend reversals** signaled by changes in the regression channel's slope.
    *   May perform well when price respects regression channel boundaries and the channel slope accurately reflects the medium-term trend. Could be susceptible to false signals during strong breakouts/breakdowns that violate the channel structure or during choppy periods where the slope changes frequently without follow-through.
    *   **Disclaimer:** Performance is highly dependent on tuning the `RegChanPeriod` and `RegChanWidth` parameters to suit the market's volatility and character. **Requires thorough backtesting and optimization.**

*   **Notes:**
    *   This strategy's logic is entirely centered around the behavior of price relative to Regression Channels.
    *   It uses both the standard Regression Channel (based on Close) and the Regression Channel High/Low variant.
    *   The combination of slope change detection and band bounce logic provides multiple ways to trigger entries based on channel dynamics.
