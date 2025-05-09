# NQ/MNQ Trading Bot Collection for NinjaTrader 8

**🛑 RISK DISCLAIMER 🛑**

**Use these automated trading strategies at your own sole risk. Futures trading is extremely risky and can result in significant losses. This code is provided for educational purposes only, NOT as financial advice. Past performance does not guarantee future results. Thorough backtesting and understanding are critical before any use.**

## Core Framework

*   [**KCAlgoBase**](./KCAlgoBase/README.md): The foundational class providing shared indicators, risk management, and order logic.
*   [**KCAlgoBase**](./!ATMAlgoBase): The foundational class providing shared indicators, risk management, and order logic that also allows you to choose your own ATM.

## Strategies

Here are the individual trading strategies built on the KCAlgoBase framework. Each strategy uses the base class for overall trend/chop filtering, risk management, and order execution, but defines its own specific entry signals:

*   [**Chaser**](./Chaser/README.md): Enters trades following short-term momentum indicated by the slope of the Linear Regression Curve.
*   [**Hooker**](./Hooker/README.md): Uses trend change "hook" signals or the slope of the BlueZHMAHooks indicator for entries.
*   [**KingKhanh**](./KingKhanh/README.md): Generates signals based on Regression Channel slope changes and price interaction with the channel bands.
*   [**MagicTrendy**](./MagicTrendy/README.md): Follows the trend signaled by the slope change of the TrendMagic indicator.
*   [**Momo**](./Momo/README.md): Enters when the Momentum indicator shows acceleration across defined thresholds.
*   [**SuperBot**](./SuperBot/README.md): Aggregates configurable signals from numerous indicators (HMA, LinReg, RegChan, TrendMagic, Momo, CMO, Willy, T3) using OR logic for entries.
*   [**SuperBot2**](./SuperBot2/README.md): Aggregates configurable signals from multiple indicators (RegChan, HMA, LinReg, Momo, CMO, T3) using OR logic for entries (variant of SuperBot).
*   [**SuperRex**](./SuperRex/README.md): Triggers entries when the Chande Momentum Oscillator (CMO) crosses defined upper or lower thresholds.
*   [**Trendy**](./Trendy/README.md): Enters based on strong, confirmed trend signals identified by the T3TrendFilter indicator.
*   [**Vima**](./Vima/README.md): Uses the direction (slope) of the Volume Moving Average (VMA), calculated in the base class, as its entry signal.
*   [**Willy**](./Willy/README.md): Employs a custom Williams %R indicator combined with price action confirmation near potential overbought/oversold levels.

---
