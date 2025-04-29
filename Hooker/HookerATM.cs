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
using BlueZ = NinjaTrader.NinjaScript.Indicators.BlueZ; // Alias for better readability
using RegressionChannel = NinjaTrader.NinjaScript.Indicators.RegressionChannel;
#endregion

namespace NinjaTrader.NinjaScript.Strategies.KCStrategies
{
    public class HookerATM : ATMAlgoBase
    {
		private BlueZ.BlueZHMAHooks hullMAHooks;
		private bool hmaHooksUp;
		private bool hmaHooksDown;
		private bool hmaUp;
		private bool hmaDown;
		
		public override string DisplayName { get { return Name; } }
		
        protected override void OnStateChange()
        {
            base.OnStateChange();

            if (State == State.SetDefaults)
            {
                Description = "Strategy based on the HMAHooks indicator.";
                Name = "Hooker ATM v5.2";
                StrategyName = "Hooker ATM";
                Version = "5.2 Apr. 2025";
                Credits = "Strategy by Heldani and Khanh Nguyen";
                ChartType =  "Tbars 20";				
				
				HmaPeriod 						= 16;
				enableHmaHooks 					= true;
				showHmaHooks 					= true;
				showMomo						= false;
	
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
			
			hmaHooksUp = ((Close[0] > hullMAHooks[0] && hullMAHooks.trend[0] == 1 && hullMAHooks.trend[1] == -1) 
				|| (hullMAHooks[0] > hullMAHooks[2]));
			
            hmaHooksDown = ((Close[0] < hullMAHooks[0] && hullMAHooks.trend[0] == -1 && hullMAHooks.trend[1] == 1)  
				|| (hullMAHooks[0] < hullMAHooks[2])); 
			
			longSignal = hmaHooksUp;
			shortSignal = hmaHooksDown;
			
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
            return false;
        }

        protected override bool ValidateExitShort()
        {
			// Logic for validating short exits
			return false;
        }

        #region Indicators
        protected override void InitializeIndicators()
        {			
			hullMAHooks = BlueZHMAHooks(Close, HmaPeriod, 0, false, false, true, Brushes.Lime, Brushes.Red);
			hullMAHooks.Plots[0].Brush = Brushes.White;
			hullMAHooks.Plots[0].Width = 2;
			if (showHmaHooks) AddChartIndicator(hullMAHooks);	
        }
        #endregion


		#region Properties - Strategy Settings
	
		[NinjaScriptProperty]
		[Display(Name = "Enable Hooker", Order = 1, GroupName = "08a. Strategy Settings")]
		public bool enableHmaHooks { get; set; }
	
		[NinjaScriptProperty]
		[Display(Name = "Show HMA Hooks", Order = 2, GroupName = "08a. Strategy Settings")]
		public bool showHmaHooks { get; set; }
	
		[NinjaScriptProperty]
		[Display(Name = "HMA Period", Order = 3, GroupName = "08a. Strategy Settings")]
		public int HmaPeriod { get; set; }
	
		#endregion
	}
}
