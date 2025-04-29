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
    public class VimaATM : ATMAlgoBase
    {
        // Parameters
		private VMA VMA1;
		private bool volMaUp;
		private bool volMaDown;		

		public override string DisplayName { get { return Name; } }
		
        protected override void OnStateChange()
        {
            base.OnStateChange();

            if (State == State.SetDefaults)
            {
                Description = "Strategy based on Volume Moving Average.";
                Name = "Vima ATM v5.2";
                StrategyName = "Vima ATM";
                Version = "5.2 Apr. 2025";
                Credits = "Strategy by Khanh Nguyen";
                ChartType =  "Tbars 20";			
				
				enableVMA						= true;
				showVMA							= true;
				
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
            
			longSignal = volMaUp;
            shortSignal = volMaDown; 
			
//			longSignal = volMaUp && Close[0] > Open[0];
//            shortSignal = volMaDown && Close[0] < Open[0]; 
			
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
			VMA1				= VMA(Close, 9, 9);
			VMA1.Plots[0].Brush = Brushes.SkyBlue;
			VMA1.Plots[0].Width = 3;
			if (showVMA) AddChartIndicator(VMA1);	
        }
        #endregion

        #region Properties

		[NinjaScriptProperty]
		[Display(Name = "Enable VMA", Order = 8, GroupName = "08a. Strategy Settings")]
		public bool enableVMA { get; set; }
	
		[NinjaScriptProperty]
		[Display(Name = "Show VMA", Order = 9, GroupName = "08a. Strategy Settings")]
		public bool showVMA { get; set; }
	
        #endregion
    }
}
