## KCAlgoBase.cs - Core Trading Framework

*   **Type:** Abstract Base Strategy (Cannot be run directly)
*   **Purpose:** Acts as a foundational framework for building specific automated trading strategies (`KCStrategies`) in NinjaTrader 8, primarily targeting NQ and MNQ futures. It provides shared logic, indicators, order management, risk controls, and UI elements.

---

### Core Functionality Provided

*   **Indicator Suite:**
    *   Initializes and manages a wide array of common technical indicators:
        *   `BlueZHMAHooks` (Hull Moving Average variant)
        *   `RegressionChannel` (Standard and High/Low versions)
        *   `BuySellPressure` (Custom indicator, likely volume/order flow based)
        *   `VMA` (Variable Moving Average)
        *   `Momentum`
        *   `ADX` (Average Directional Index)
        *   `ATR` (Average True Range)
        *   `NTSvePivots` (Daily Pivot Points and Mid-Pivots)
        *   `EMA` (Exponential Moving Average - for filtering)
        *   `ChoppinessIndex`
    *   Provides boolean flags (e.g., `hmaUp`, `regChanUp`, `adxUp`) representing the state of these indicators.

*   **Trend & Chop Definition:**
    *   Defines `uptrend` and `downtrend` conditions based on a **confluence** of signals from the configured indicators (ADX, Momentum, Buy/Sell Pressure, HMA, VMA, RegChan, ATR, EMA).
    *   Features a **Choppiness Filter** (`EnableChoppinessDetection`) using:
        *   Regression Channel slope (`FlatSlopeFactor`)
        *   ADX level (`ChopAdxThreshold`)
        *   `ChoppinessIndex` value (`choppyThreshold`)
    *   Can **automatically disable auto-trading** (`isAutoEnabled = false`) when chop is detected and re-enable it when conditions improve (respects manual override).
    *   Optionally sets chart background color based on trend or chop (`enableBackgroundSignal`).

*   **Order Management Framework:**
    *   **Entry Structure:** Handles the *process* of entering trades (checking base conditions, submitting orders) but relies on inheriting strategies for the specific *signals*.
    *   **Initial Stop Loss:** Placed via `Exit...StopMarket` upon entry based on `InitialStop` ticks (with potential override logic).
    *   **Breakeven:** Automatic BE logic (`BESetAuto`) triggered by `BE_Trigger` ticks profit, moving the stop to entry +/- `BE_Offset` ticks.
    *   **Trailing Stops:** Offers multiple types via `TrailStopTypeKC` parameter:
        *   `Tick_Trail`: Standard tick-based trail (uses `InitialStop` as distance initially).
        *   `Regression_Channel_Trail`: Trails using the `RegressionChannel1` bands (Upper for shorts, Lower for longs) with fallback/override logic (`MinRegChanStopDistanceTicks`, `InitialStop`).
        *   `Three_Step_Trail`: Multi-stage trail tightening stop based on profit triggers (`step1/2/3ProfitTrigger`, `step1/2/3StopLoss`).
        *   `ATR_Trail`: Trails based on `ATR` value multiplied by `atrMultiplier`.
        *   `Fixed_Stop`: Disables trailing; stop remains fixed unless moved by BE or manually.
    *   **Profit Targets:** Offers multiple types via parameters:
        *   `EnableFixedProfitTarget`: Uses fixed tick values (`ProfitTarget`, `ProfitTarget2`, `ProfitTarget3`, `ProfitTarget4`) allowing for **scale-out** entries/targets.
        *   `EnableRegChanProfitTarget`: Uses `RegressionChannel2` bands as dynamic targets (with fallback logic `MinRegChanTargetDistanceTicks`).
        *   `EnableDynamicProfitTarget`: Uses `NTSvePivots` levels as price-based targets.
    *   **Order Tracking:** Maintains a dictionary (`activeOrders`) of strategy-submitted orders and handles execution updates (`OnExecutionUpdate`) to manage state.

*   **Risk Management:**
    *   **Daily P/L Limits:** Optional (`dailyLossProfit`) limits (`DailyProfitLimit`, `DailyLossLimit`) in currency.
    *   **Trailing Drawdown:** Optional (`enableTrailingDrawdown`) equity-based drawdown (`TrailingDrawdown`) from the peak profit (`maxProfit`) achieved since the session start (or strategy start). Starts tracking after `StartTrailingDD` profit is reached.
    *   **KillSwitch:** Monitors P/L limits and trailing drawdown. If breached, it **flattens the current position** and **disables auto-trading** (`isAutoEnabled = false`).
    *   **Time Filters:** Up to 6 configurable time windows (`Start`/`End` pairs) to restrict trading activity.
    *   **Trades Per Direction:** Optional limit (`TradesPerDirection`, `longPerDirection`, `shortPerDirection`) on consecutive entries in the same direction.

*   **Custom Chart Trader UI:**
    *   Adds a panel with buttons for extensive manual control:
        *   Toggle Auto/Manual mode (`AutoBtn`/`ManualBtn`).
        *   Enable/Disable Auto Long/Short entries (`LongBtn`/`ShortBtn`).
        *   Quick Manual Entry (`QuickLongBtn`/`QuickShortBtn`) - respects manual mode & base trend.
        *   Add/Close 1 Contract (`Add1Btn`/`Close1Btn`).
        *   Toggle Auto BE/TS (`BEBtn`/`TSBtn`).
        *   Manually Adjust Stop (`MoveTSBtn`, `MoveTS50PctBtn`, `MoveToBeBtn`).
        *   Close Strategy Positions (`CloseBtn`).
        *   Flatten All Instrument Positions (`PanicBtn`).
        *   Donate Link (`DonatePayPalBtn`).

*   **Robustness & Safety:**
    *   Uses `lock (orderLock)` for thread safety around order operations.
    *   Rate limits order submissions (`minOrderActionInterval`).
    *   Includes basic rogue order detection (`ReconcileAccountOrders`).
    *   Flags order errors (`orderErrorOccurred`) to potentially halt trading.

*   **Parameter Customization:**
    *   Uses `ICustomTypeDescriptor` to dynamically hide/show parameters in the NT8 UI based on selected modes (e.g., hide ATR settings if Tick Trail is chosen).

---

### How It's Used

1.  A developer creates a new `.cs` strategy file that **inherits** from `KCAlgoBase`.
2.  The developer **must implement** the abstract methods:
    *   `ValidateEntryLong()`: Define the specific conditions for a long entry signal.
    *   `ValidateEntryShort()`: Define the specific conditions for a short entry signal.
    *   `InitializeIndicators()`: (Often left empty if no *extra* indicators specific to the child are needed).
3.  The developer **can optionally override** virtual methods like:
    *   `ValidateExitLong()` / `ValidateExitShort()`: To add custom signal-based exits.
    *   `addDataSeries()`: To add secondary data series if required.
4.  When the child strategy runs, `KCAlgoBase` handles the heavy lifting: indicator calculations, trend/chop detection, time checks, risk limit checks, order placement/management (stops, targets, BE), and UI interactions. The child strategy primarily focuses on providing the core entry/exit *signals*.

---

### Inferred Design Intent

*   **Target Market:** NQ/MNQ Futures (indicated by user and typical use of these indicators/settings).
*   **Trading Style:** Primarily designed for **trend-following** strategies, leveraging indicator confluence and actively trying to **filter out choppy/ranging** conditions.
*   **Flexibility:** Provides significant flexibility through parameters for different trailing stop/profit target methods and time filters.
*   **User Interaction:** Offers substantial manual control via the custom UI panel, allowing discretionary overrides or adjustments.
*   **Complexity:** This is a complex base class requiring a good understanding of NinjaScript and the underlying trading concepts to use effectively.

---

**Note:** This base class provides the engine and safety features. The actual performance of any bot built upon it will heavily depend on the quality of the specific entry/exit logic defined in the inheriting strategy (`ValidateEntryLong`/`Short`, `ValidateExitLong`/`Short`) and the parameter tuning achieved through backtesting and optimization.
