# NQ/MNQ Trading Bot Collection for NinjaTrader 8

(Your introduction and disclaimers here...)

## Core Framework

*   [**KCAlgoBase**](./KCAlgoBase/README.md): The foundational class providing shared indicators, risk management, and order logic. *(Clicking this takes you to the KCAlgoBase description)*

## Strategies

## Strategies

Here are the individual trading strategies built on the KCAlgoBase framework. Each strategy uses the base class for overall trend/chop filtering, risk management, and order execution, but defines its own specific entry signals:

*   [**Chaser**](./Chaser/README.md): Enters trades following short-term momentum indicated by the slope of the Linear Regression Curve.
*   [**Hooker**](./Strategies/Hooker/README.md): Uses trend change "hook" signals or the slope of the BlueZHMAHooks indicator for entries.
*   [**KingKhanh**](./Strategies/KingKhanh/README.md): Generates signals based on Regression Channel slope changes and price interaction with the channel bands.
*   [**MagicTrendy**](./Strategies/MagicTrendy/README.md): Follows the trend signaled by the slope change of the TrendMagic indicator.
*   [**Momo**](./Strategies/Momo/README.md): Enters when the Momentum indicator shows acceleration across defined thresholds.
*   [**SuperBot**](./Strategies/SuperBot/README.md): Aggregates configurable signals from numerous indicators (HMA, LinReg, RegChan, TrendMagic, Momo, CMO, Willy, T3) using OR logic for entries.
*   [**SuperBot2**](./Strategies/SuperBot2/README.md): Aggregates configurable signals from multiple indicators (RegChan, HMA, LinReg, Momo, CMO, T3) using OR logic for entries (variant of SuperBot).
*   [**SuperRex**](./Strategies/SuperRex/README.md): Triggers entries when the Chande Momentum Oscillator (CMO) crosses defined upper or lower thresholds.
*   [**Trendy**](./Strategies/Trendy/README.md): Enters based on strong, confirmed trend signals identified by the T3TrendFilter indicator.
*   [**Vima**](./Strategies/Vima/README.md): Uses the direction (slope) of the Volume Moving Average (VMA), calculated in the base class, as its entry signal.
*   [**Willy**](./Strategies/Willy/README.md): Employs a custom Williams %R indicator combined with price action confirmation near potential overbought/oversold levels.

---
