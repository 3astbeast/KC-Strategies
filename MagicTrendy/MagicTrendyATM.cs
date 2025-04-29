#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

/*
    MagicTrendy v3.0 MA
    Version: 3.0 Oct. 19, 2024
    Strategy by Khanh Nguyen
*/

namespace NinjaTrader.NinjaScript.Strategies.KCStrategies
{
    public class MagicTrendyATM : ATMAlgoBase
    {		
		private TrendMagic TrendMagic1;
		private int cciPeriod;
		private int atrPeriod;
        private bool activeOrder = false;

		public override string DisplayName { get { return Name; } }

        protected override void OnStateChange()
        {
            base.OnStateChange();
            
            if (State == State.SetDefaults)
            {
                Description = "Strategy based on the TrendMagic indicator";
                Name = "MagicTrendy ATM v5.2";
                StrategyName = "MagicTrendy ATM";
                Version = "5.2 Apr. 2025";
                Credits = "Strategy by Khanh Nguyen";
                ChartType =  "Tbars 20";
				
				cciPeriod		= 20;
				atrPeriod		= 14;
				atrMult			= 0.1;	
            }
            else if (State == State.DataLoaded)
            {
                InitializeIndicators();
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBars[0] < BarsRequiredToTrade)
                return;
            
			longSignal = TrendMagic1.Trend[1] > TrendMagic1.Trend[2];
            shortSignal = TrendMagic1.Trend[1] < TrendMagic1.Trend[2];	
			
            base.OnBarUpdate();
        }

        protected override bool ValidateEntryLong()
        {
            // Logic for validating long entries
			if (longSignal) return true;
			else return false;
        }

        protected override bool ValidateEntryShort()
        {
            // Logic for validating short entries
			if (shortSignal) return true;
            else return false;
        }

        protected override bool ValidateExitLong()
        {
            // Logic for validating long exits
            return enableExit? true : false;
        }

        protected override bool ValidateExitShort()
        {
			// Logic for validating short exits
			return enableExit? true : false;
        }

        #region Indicators
        protected override void InitializeIndicators()
        {				
			TrendMagic1		 	= TrendMagic(cciPeriod, atrPeriod, atrMult, false);
            AddChartIndicator(TrendMagic1);
        }
        #endregion

        #region Properties
        [NinjaScriptProperty]
		[Display(Name="TrendMagic ATR Multiplier", Order=1, GroupName="08a. Strategy Settings")]
		public double atrMult
		{ get; set; }
		
        #endregion
    }
}
