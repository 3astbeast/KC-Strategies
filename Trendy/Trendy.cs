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
    public class Trendy : KCAlgoBase
    {
        // Strategy variables
        private T3TrendFilter T3TrendFilter1;
        private double trendyUp;
        private double trendyDown;

		public override string DisplayName { get { return Name; } }

        protected override void OnStateChange()
        {
            base.OnStateChange();

            if (State == State.SetDefaults)
            {
                Description = "Strategy based on the T3TrendFilter indicator.";
                Name = "Trendy v5.2";
                StrategyName = "Trendy";
                Version = "5.2 Apr. 2025";
                Credits = "Strategy by Khanh Nguyen";
                ChartType = "Tbars 20";	
                
//                InitialStop		= 140;
//				ProfitTarget	= 32;

                // T3 Trend Filter settings
                Factor = 0.5;
                Period1 = 1;
                Period2 = 1;
                Period3 = 1;
                Period4 = 1;
                Period5 = 9;
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

            // Update trend filter values
            trendyUp = T3TrendFilter1.Values[0][0];
            trendyDown = T3TrendFilter1.Values[1][0];

			longSignal = trendyUp >= 5 && trendyDown == 0;
            shortSignal = trendyDown <= -5 && trendyUp == 0;	
			
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
            // Initialize the T3TrendFilter indicator
            T3TrendFilter1 = T3TrendFilter(Close, Factor, Period1, Period2, Period3, Period4, Period5, false);
			AddChartIndicator(T3TrendFilter1);
        }
        #endregion

        #region Properties

        [NinjaScriptProperty]
        [Display(Name = "Factor", Order = 1, GroupName = "04. Filters Settings - T3TrendFilter")]
        public double Factor { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Period1", Order = 2, GroupName = "04. Filters Settings - T3TrendFilter")]
        public int Period1 { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Period2", Order = 3, GroupName = "04. Filters Settings - T3TrendFilter")]
        public int Period2 { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Period3", Order = 4, GroupName = "04. Filters Settings - T3TrendFilter")]
        public int Period3 { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Period4", Order = 5, GroupName = "04. Filters Settings - T3TrendFilter")]
        public int Period4 { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Period5", Order = 6, GroupName = "04. Filters Settings - T3TrendFilter")]
        public int Period5 { get; set; }

        #endregion
    }
}
