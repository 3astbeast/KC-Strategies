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
using BlueZ = NinjaTrader.NinjaScript.Indicators.BlueZ; // Alias for better readability
using RegressionChannel = NinjaTrader.NinjaScript.Indicators.RegressionChannel;
#endregion

namespace NinjaTrader.NinjaScript.Strategies.KCStrategies
{
    public class Hooker : KCAlgoBase
    {
        // Parameters
		private BlueZ.BlueZHMAHooks hullMAHooks;
		private RegressionChannel RegressionChannel1, RegressionChannel2;
		private RegressionChannelHighLow RegressionChannelHighLow1;	
		
		private bool regChanUp;
		private bool regChanDown;

		public override string DisplayName { get { return Name; } }
		
        protected override void OnStateChange()
        {
            base.OnStateChange();

            if (State == State.SetDefaults)
            {
                Description = "Strategy based on the BlueZHMAHooks indicator.";
                Name = "Hooker v5.2";
                StrategyName = "Hooker";
                Version = "5.2 Apr. 2025";
                Credits = "Strategy by Heldani and Khanh Nguyen";
                ChartType = "Tbars 20";		
				
				HmaPeriod		= 16;
				showMomo		= false;
				RegChanPeriod	= 40;
				RegChanWidth	= 4;
				RegChanWidth2	= 3;				
				
//                InitialStop		= 93;
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
			
			regChanUp = RegressionChannel1.Middle[0] > RegressionChannel1.Middle[2] && Close[0] < RegressionChannel2.Upper[0];
			regChanDown = RegressionChannel1.Middle[0] < RegressionChannel1.Middle[2] && Close[0] > RegressionChannel2.Lower[0];
			
			longSignal = ((Close[0] > hullMAHooks[0] && hullMAHooks.trend[0] == 1 && hullMAHooks.trend[1] == -1) 
				|| (hullMAHooks[0] > hullMAHooks[1]));
			
            shortSignal = ((Close[0] < hullMAHooks[0] && hullMAHooks.trend[0] == -1 && hullMAHooks.trend[1] == 1)  
				|| (hullMAHooks[0] < hullMAHooks[1])); 
			
//			longSignal = (Close[0] > hullMAHooks[0] && hullMAHooks.trend[0] == 1 && hullMAHooks.trend[1] == -1) 
//				|| (hullMAHooks[0] > hullMAHooks[2]) && (Close[0] < RegressionChannel2.Upper[0]);
			
//            shortSignal = (Close[0] < hullMAHooks[0] && hullMAHooks.trend[0] == -1 && hullMAHooks.trend[1] == 1)  
//				|| (hullMAHooks[0] < hullMAHooks[2]) && (Close[0] > RegressionChannel2.Lower[0]); 
			
			exitLong = shortSignal;
			exitShort = longSignal;
			
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
			hullMAHooks	= BlueZHMAHooks(Close, HmaPeriod, 0, false, false, true, Brushes.Lime, Brushes.Red);
			hullMAHooks.Plots[0].Brush = Brushes.White;
			hullMAHooks.Plots[0].Width = 2;
			if (showHmaHooks) AddChartIndicator(hullMAHooks);
			
			RegressionChannel1 = RegressionChannel(Close, Convert.ToInt32(RegChanPeriod), RegChanWidth);			
			RegressionChannel2 = RegressionChannel(Close, Convert.ToInt32(RegChanPeriod), RegChanWidth2);
			RegressionChannelHighLow1 = RegressionChannelHighLow(Close, Convert.ToInt32(RegChanPeriod), RegChanWidth);
			AddChartIndicator(RegressionChannel1);
			AddChartIndicator(RegressionChannel2);
			AddChartIndicator(RegressionChannelHighLow1);
        }
        #endregion

        #region Properties

//		[NinjaScriptProperty]
//		[Display(Name="HMA Period", Order=1, GroupName="08a. Strategy Settings")]
//		public int HmaPeriod
//		{ get; set; }

//		[NinjaScriptProperty]
//		[Display(Name="Regression Channel Period", Order=2, GroupName="08a. Strategy Settings")]
//		public int RegChanPeriod
//		{ get; set; }

//		[NinjaScriptProperty]
//		[Display(Name="Outer Regression Channel Width", Order=3, GroupName="08a. Strategy Settings")]
//		public double RegChanWidth
//		{ get; set; }

//		[NinjaScriptProperty]
//		[Display(Name="Inner Regression Channel Width", Order=4, GroupName="08a. Strategy Settings")]
//		public double RegChanWidth2
//		{ get; set; }

        #endregion
    }
}
