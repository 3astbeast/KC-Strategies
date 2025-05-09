## ATMAlgoBase.cs - Core Framework for ATM Strategy Integration

*   **Type:** Abstract Base Strategy (Cannot be run directly)
*   **Purpose:** `ATMAlgoBase` serves as a foundational framework for developing NinjaTrader 8 strategies that primarily leverage NinjaTrader's **built-in ATM (Advanced Trade Management) Strategies** for order execution, stop loss, and profit target management. It integrates a suite of common indicators for signal generation and market analysis, provides UI controls, and handles common strategy lifecycle events.

---

### Core Functionality & Design

1.  **ATM Strategy Integration (Primary Feature):**
    *   **Execution Model:** Trades are entered by creating an ATM Strategy instance using `AtmStrategyCreate()`. This method takes an `OrderAction` (Buy/SellShort), `OrderType` (Market/Limit), entry price, and importantly, an `ATMStrategyTemplate` name.
    *   **Stop/Target Management:** Once an ATM strategy is active, the stop loss and profit target orders are managed by the **NinjaTrader ATM engine** according to the rules defined in the selected template. This base class does *not* directly manage stop/target prices or trailing logic programmatically.
    *   **Parameter:** A key parameter `ATMStrategyTemplate` (string) allows users to select a pre-configured ATM template from their NinjaTrader setup. A custom converter (`FriendlyAtmConverter`) is included to populate this parameter with a dropdown list of available ATM templates.
    *   **Tracking:** Internal variables `atmStrategyId` and `orderId` track the active ATM and its entry order. `isAtmStrategyCreated` flags successful ATM creation.
    *   **Entry Order Cancellation:** The `CancelUnfilledEntry()` method is designed to close the ATM strategy (which cancels its entry order) if the entry order is still working at the start of the next bar.

2.  **Indicator Suite & Signal Generation:**
    *   Initializes and manages a comprehensive set of indicators for market analysis:
        *   `BlueZHMAHooks` (Hull Moving Average variant)
        *   `BuySellPressure`
        *   `RegressionChannel` (Outer and Inner)
        *   `RegressionChannelHighLow`
        *   `VMA` (Variable Moving Average)
        *   `Momentum`
        *   `ADX` (Average Directional Index)
        *   `ATR` (Average True Range)
    *   Calculates boolean flags based on these indicators (e.g., `hmaUp`, `regChanUp`, `adxUp`, `buyPressureUp`) which contribute to the overall `uptrend` and `downtrend` conditions.
    *   **Trend Definition:**
        *   `uptrend = momoUp && buyPressureUp && hmaUp && volMaUp && regChanUp && adxUp && atrUp;`
        *   `downtrend = momoDown && sellPressureUp && hmaDown && volMaDown && regChanDown && adxUp && atrUp;`
        *   This requires a confluence of signals from multiple enabled indicators.
    *   Relies on inheriting strategies to define the specific `longSignal` and `shortSignal` in their `ValidateEntryLong()` and `ValidateEntryShort()` methods.

3.  **Market Condition Analysis:**
    *   **Choppiness Filter:** Implements a choppiness detection mechanism (`EnableChoppinessDetection`) based on:
        *   Regression Channel slope (`SlopeLookBack`, `FlatSlopeFactor`)
        *   ADX level (`ChopAdxThreshold`)
    *   Can automatically disable auto-trading (`isAutoEnabled = false`) when chop is detected and re-enable it when conditions improve.
    *   Optionally sets chart background color based on trend or chop status (`enableBackgroundSignal`).

4.  **Risk & Session Management:**
    *   **PnL Limits:** Supports Daily Profit Limit (`DailyProfitLimit`) and Daily Loss Limit (`DailyLossLimit`).
    *   **Trailing Drawdown:** Implements an equity-based Trailing Drawdown (`TrailingDrawdown` from `maxProfit`, starting after `StartTrailingDD` is reached).
    *   **KillSwitch:** Monitors P/L limits and trailing drawdown. If breached, it attempts to close the active ATM strategy (or flatten directly if no ATM ID) and disables auto-trading.
    *   **Time Filters:** Allows configuration of up to six distinct time windows (`Start`/`End` pairs) for trading activity.
    *   **Trades Per Direction:** Optional limit on consecutive entries in the same direction.
    *   **Session Handling:** Exits positions at session close (`IsExitOnSessionCloseStrategy`). Resets daily PnL tracking at the start of a new session.

5.  **Custom Chart Trader UI Panel:**
    *   Adds a panel with buttons for manual control:
        *   Toggle Auto/Manual mode (`AutoBtn`/`ManualBtn`).
        *   Enable/Disable Auto Long/Short entries (`LongBtn`/`ShortBtn`).
        *   Quick Manual Entry (`QuickLongBtn`/`QuickShortBtn`) - submits trades using the selected ATM template.
        *   Close Current ATM Strategy / All Positions (`CloseBtn`).
        *   Flatten All Instrument Positions (`PanicBtn`).
        *   Donate Link (`paypalBtn`).
    *   *Note: Buttons for direct stop/target manipulation (like moving a stop to BE or trailing) are not included, as this is managed by the ATM.*

6.  **Robustness & Structure:**
    *   Uses `OnStateChange` with a `switch` statement to manage different stages of the strategy lifecycle (SetDefaults, Configure, DataLoaded, Terminated).
    *   Employs `try-catch` in `OnBarUpdate` for error handling.
    *   Includes `orderLock` for thread safety (though less critical for ATM-based entries compared to direct order submissions).
    *   Basic rogue order detection (`ReconcileAccountOrders`).
    *   Parameter UI customization via `ICustomTypeDescriptor`.

---

### How It's Used

1.  **User Defines ATM Templates:** The user first creates and configures one or more ATM Strategy templates within NinjaTrader's interface, defining stop loss distances, profit targets, number of targets, auto-breakeven rules, and auto-trailing stop rules.
2.  **Strategy Development:** A developer creates a new `.cs` strategy file that inherits from `ATMAlgoBase`.
3.  **Implement Signals:** The developer implements the `ValidateEntryLong()` and `ValidateEntryShort()` methods to define the specific conditions for entry signals based on indicators or price action. `InitializeIndicators()` is used for any indicators specific to the child strategy (though many are already in the base).
4.  **Select ATM Template:** When applying the strategy to a chart or in the Strategy Analyzer, the user selects one of their pre-defined ATM templates from the `ATMStrategyTemplate` parameter dropdown.
5.  **Execution Flow:**
    *   The child strategy provides `longSignal` or `shortSignal`.
    *   `ATMAlgoBase` checks base conditions (trend, chop, time, PnL limits).
    *   If all conditions are met, `CreateAtmStrategy()` is called.
    *   `AtmStrategyCreate()` submits the entry order (Market or Limit) and attaches the selected ATM template.
    *   Once the entry fills, the ATM template takes over management of the stop loss and profit target(s).

---

### Inferred Design Intent & Philosophy

*   **Simplify Trade Management Code:** To abstract away the complexities of coding stop-loss, profit-target, breakeven, and trailing logic by leveraging NinjaTrader's built-in ATM functionality.
*   **User-Friendly Configuration for Exits:** Allow users to define and modify their exit strategies (stops, targets, trailing) through the graphical ATM interface rather than code parameters, making it accessible to non-programmers.
*   **Flexibility via Templates:** Provide a robust framework for signal generation while allowing diverse exit and risk management styles by simply choosing different ATM templates.
*   **Focus on Entry Signals:** Enable developers to concentrate primarily on crafting effective entry signals, with the base class and ATM engine handling much of the rest.
*   **Suitability for Specific Bar Types:** The default "Orenko 34-40-40" chart type suggests a potential focus on non-time-based charts where ATM strategies based on fixed tick offsets can be effective.

---

**Note:** While this base class handles signal generation and initiating ATM strategies, the actual risk/reward characteristics and exit behavior of trades will be heavily dictated by the specific ATM Strategy Template selected by the user. Thorough testing of both the strategy's signals and the chosen ATM templates is essential.
