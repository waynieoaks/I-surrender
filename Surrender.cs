using System;
using GTA;
using GTA.Native;
using System.Windows.Forms;

namespace Surrender
{
	public class Main : Script
	{
		//public static bool AreHandsUp { get; set; }
		public static string INIpath = "scripts\\Surrender.ini";
		public static ScriptSettings IniSettings;
		public static Keys SurrenderKey { get; set; }
		public static Keys SurrenderModifierKey { get; set; }
		public static Keys ClearKey { get; set; }
		public static Keys ClearModifierKey { get; set; }
		public static bool DropWeapon { get; set; }
		public static bool AmIArrested { get; set; }
		public static bool AmIWasted { get; set; }
		public static bool AmISurrendering { get; set; }
		public static int WantedLvL { get; set; }

		public Main()
		{
			LoadValuesFromIniFile();

			KeyDown += OnKeyDown;
			Tick += ChkWantedTick;

			//Interval = 50;
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{

			if (e.KeyCode == SurrenderKey && e.Modifiers == SurrenderModifierKey)
			{
				if (AmISurrendering == true) {
					DoEscape();
				} else {
					DoSurrender();
				}
				
			}
			else if (e.KeyCode == ClearKey && e.Modifiers == ClearModifierKey)
			{
				DoClearWanted();
			}
//////////////////  TESTING -- REMOVE ////////////////// 
			//else if (e.KeyCode == Keys.F10)
			//{
			//	// Remove after testking kill... 
			//	Game.Player.WantedLevel = 2;
			//}
			//else if (e.KeyCode == Keys.F9)
			//{
			//	// Remove after testking kill... 
			//	Game.Player.Character.Kill();
			//}
			//else if (e.KeyCode == Keys.F8)
			//{
			//	// Testing - what is the state of things
			//	GTA.UI.Notification.Show("Am I Surrendering? " + AmISurrendering.ToString());
			//}
//////////////////  TESTING -- END ////////////////// 
		}

		private void DoClearWanted()
		{
			Game.Player.WantedLevel = 0;
			ClearTasks();
		}

		private void DoEscape()
		{
			ClearTasks();
			GTA.UI.Screen.ShowSubtitle("I suggest you ~r~run~s~...", 6000);
		}

		private void ClearTasks()
		{
			Game.Player.Character.Task.ClearAll();
			AmISurrendering = false;
		}

		private void DoSurrender()
		{
			WantedLvL = Function.Call<int>(Hash.GET_PLAYER_WANTED_LEVEL);
			if (WantedLvL > 0)
			{
				if (WantedLvL > 1)
				{
					// Make sure wanted level is 1 star to prevent shooting
					Game.Player.WantedLevel = 1;
				}

				// If in vehicle, get out
				if (Game.Player.Character.IsInVehicle())
				{
					//Vehicle playerVehicle = Game.Player.Character.CurrentVehicle;
					Game.Player.Character.Task.LeaveVehicle(Game.Player.Character.CurrentVehicle, true);

					// Wait until out of vehicle before putting hands up
					Wait(2000);
				}
				
				HandsUp();
			}
		}

		private void HandsUp()
		{
			// Inform code I am surrendering
			AmISurrendering = true;

			// If holding a weapon, drop it
			if (DropWeapon == true)
			{
				Function.Call(Hash.SET_PED_DROPS_WEAPON, Game.Player.Character);
			}
			// Put hands up until code breaks
			Game.Player.Character.Task.HandsUp(-1);
		}

		private void ChkWantedTick(object sender, EventArgs e)
		{
			WantedLvL = Function.Call<int>(Hash.GET_PLAYER_WANTED_LEVEL);
			AmIArrested = Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED);
			AmIWasted = Function.Call<bool>(Hash.IS_PLAYER_DEAD);

			if ( (AmISurrendering == true) && (WantedLvL > 0) )
			{
				if ((AmIArrested == true) || (AmIWasted == true))
				{
					// I have been arrested or killed, inform the code and breakout
					AmISurrendering = false;
					return;
				}
				else
				{
					// Attempt to keep wanted level at 1 star
					Game.Player.WantedLevel = 1;
					return;
				}
			} else
			{
				// I should not be surrendering
				AmISurrendering = false;
				return;
			}
		}

		private void LoadValuesFromIniFile()
		{
			AmISurrendering = false;
			ScriptSettings scriptSettings = ScriptSettings.Load(INIpath);
			SurrenderKey = (Keys)scriptSettings.GetValue<Keys>("Controls", "Surrender_Key", Keys.K);
			SurrenderModifierKey = (Keys)scriptSettings.GetValue<Keys>("Controls", "Surrender_Modifier", Keys.ControlKey);
			ClearKey = (Keys)scriptSettings.GetValue<Keys>("Controls", "Clear_Wanted_Key", Keys.L);
			ClearModifierKey = (Keys)scriptSettings.GetValue<Keys>("Controls", "Clear_Wanted_Modifier", Keys.ControlKey);
			DropWeapon = (bool)scriptSettings.GetValue<bool>("Options", "DropWeapon", true);
		}
	}
}