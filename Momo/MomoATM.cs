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

namespace NinjaTrader.NinjaScript.Strategies.KCStrategies
{
    public class MomoATM : ATMAlgoBase
    {
		// Indicators
		private Momentum Momentum1;
		
        private bool activeOrder = false;

		public override string DisplayName { get { return Name; } }

        protected override void OnStateChange()
        {
            base.OnStateChange();
            
            if (State == State.SetDefaults)
            {
                Description = "This strategy based on the Momentum indicator.";
                Name = "Momo ATM v5.2";
                StrategyName = "Momo ATM";
                Version = "5.2 Apr. 2025";
                Credits = "Strategy by Khanh Nguyen";
                ChartType = "Tbars 20";	
				
				MomoUp			= 5;
				MomoDown		= -5;
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
			
			longSignal = Momentum1[0] > MomoUp && Momentum1[0] > Momentum1[1];
            shortSignal = Momentum1[0] < MomoDown && Momentum1[0] < Momentum1[1];;
			
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
			Momentum1			= Momentum(Close, 14);	
			Momentum1.Plots[0].Brush = Brushes.Yellow;
			Momentum1.Plots[0].Width = 2;
			AddChartIndicator(Momentum1);
        }
        #endregion

        #region Properties
		
		[NinjaScriptProperty]
		[Display(Name="Momo Up", Order=1, GroupName="08a. Strategy Settings")]
		public int MomoUp
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Momo Down", Order=2, GroupName="08a. Strategy Settings")]
		public int MomoDown
		{ get; set; }
		
        #endregion
    }
}
