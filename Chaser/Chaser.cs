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
using NinjaTrader.NinjaScript.Strategies;
#endregion

namespace NinjaTrader.NinjaScript.Strategies.KCStrategies
{
    public class Chaser : KCAlgoBase
    {
        // Parameters
        private LinReg LinReg1;

		public override string DisplayName { get { return Name; } }
		
        protected override void OnStateChange()
        {
            base.OnStateChange();

            if (State == State.SetDefaults)
            {
                Description = "Strategy based on the Linear Regression Curve indicator.";
                Name = "Chaser v5.2";
                StrategyName = "Chaser";
                Version = "5.2 Feb. 2025";
                Credits = "Strategy by Khanh Nguyen";
                ChartType =  "Tbars 20";

                LinRegPeriod 	= 9;
				
//                InitialStop		= 97;
//				ProfitTarget	= 40;
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

            // Generate signals
            longSignal = LinReg1[1] > LinReg1[2] && LinReg1[2] > LinReg1[3];
            shortSignal = LinReg1[1] < LinReg1[2] && LinReg1[2] < LinReg1[3];
			
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
            return enableExit? true: false;
        }

        protected override bool ValidateExitShort()
        {
			// Logic for validating short exits
			return enableExit? true: false;
        }

        #region Indicators
        protected override void InitializeIndicators()
        {
            LinReg1 = LinReg(Close, LinRegPeriod);
			LinReg1.Plots[0].Brush = Brushes.Yellow;
			LinReg1.Plots[0].Width = 2;
            AddChartIndicator(LinReg1);
        }
        #endregion

        #region Properties

        [NinjaScriptProperty]
        [Display(Name = "LinReg Period", Order = 0, GroupName = "08a. Strategy Settings")]
        public int LinRegPeriod { get; set; }

        #endregion
    }
}
