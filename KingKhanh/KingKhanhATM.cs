#region Using declarations
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.Core;
using RegressionChannel = NinjaTrader.NinjaScript.Indicators.RegressionChannel;
#endregion

namespace NinjaTrader.NinjaScript.Strategies.KCStrategies
{
    public class KingKhanhATM : ATMAlgoBase
    {
       	private RegressionChannel RegressionChannel1, RegressionChannel2;
		private RegressionChannelHighLow RegressionChannelHighLow1;		
				
		private double highestHigh;
		private double lowestLow;
		
		public override string DisplayName { get { return Name; } }
		
        protected override void OnStateChange()
        {
            base.OnStateChange();

            if (State == State.SetDefaults)
            {
                Description = "Strategy based on the Linear Regression Channel indicator.";
                Name = "KingKhanh ATM v5.2";
                StrategyName = "KingKhanh ATM";
                Version = "5.2 Apr. 2025";
                Credits = "Strategy by Khanh Nguyen";
                ChartType = "Tbars 20";		

				RegChanPeriod		= 40;
				RegChanWidth		= 5;
				RegChanWidth2		= 4;
				showMomo			= false;					
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
			
			longSignal = 
				// Condition group 1
				(((RegressionChannel1.Middle[1] > RegressionChannel1.Middle[2]) && (RegressionChannel1.Middle[2] <= RegressionChannel1.Middle[3]))
				
				// Condition group 2
				|| ((RegressionChannel1.Middle[0] > RegressionChannel1.Middle[1]) && (Low[0] > Low[2]) && (Low[2] <= RegressionChannel1.Lower[2]))
				
				// Condition group 3
				|| (Low[0] > RegressionChannelHighLow1.Lower[2]));

            shortSignal = 
				 // Condition group 1
				 (((RegressionChannel1.Middle[1] < RegressionChannel1.Middle[2]) && (RegressionChannel1.Middle[2] >= RegressionChannel1.Middle[3]))
			
				 // Condition group 2
				 || ((RegressionChannel1.Middle[0] < RegressionChannel1.Middle[1])  && (High[0] < High[2]) && (High[2] >= RegressionChannel1.Upper[2]))
			
				 // Condition group 3
				 || (High[0] < RegressionChannelHighLow1.Upper[2]));  
			
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
            if (exitLong) return true;
            else return false;
        }

        protected override bool ValidateExitShort()
        {
			if (exitShort) return true;
			else return false;
        }

        #region Indicators
        protected override void InitializeIndicators()
        {
			RegressionChannel1			= RegressionChannel(Close, RegChanPeriod, RegChanWidth);
			AddChartIndicator(RegressionChannel1);
			
            RegressionChannel2			= RegressionChannel(Close, RegChanPeriod, RegChanWidth2);
			AddChartIndicator(RegressionChannel2);
			
			RegressionChannelHighLow1	= RegressionChannelHighLow(Close, RegChanPeriod, RegChanWidth);	
			RegressionChannelHighLow1.Plots[0].Width = 2;
			RegressionChannelHighLow1.Plots[1].Width = 2;
			RegressionChannelHighLow1.Plots[2].Width = 2;	
			AddChartIndicator(RegressionChannelHighLow1);	
        }
        #endregion


		#region Properties - Strategy Settings
	
		[NinjaScriptProperty]
		[Display(Name="Tick Distance from High / Low Lines", Order=2, GroupName="02. Order Settings")]
		public int TickDistance
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Regression Channel Period", Order=1, GroupName="08a. Strategy Settings")]
		public int RegChanPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Regression Channel Width", Order=2, GroupName="08a. Strategy Settings")]
		public double RegChanWidth
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Inner Regression Channel Width", Order=3, GroupName="08a. Strategy Settings")]
		public double RegChanWidth2
		{ get; set; }

		#endregion
	}
}
