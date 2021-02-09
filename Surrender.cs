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
		public static bool AmISurrendering { get; set; }

		public Main()
		{
			LoadValuesFromIniFile();

			KeyDown += OnKeyDown;

			Interval = 1000; // Try reducing to 500
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{

			if (e.KeyCode == SurrenderKey && e.Modifiers == SurrenderModifierKey)

			{
				
				if (AmISurrendering == true) {
					// Release
					Game.Player.Character.Task.ClearAll();
					AmISurrendering = false;
				} 
				else
				{
					// Surrender
					Surrender();
				}
				
			}
			else if (e.KeyCode == ClearKey && e.Modifiers == ClearModifierKey)
			{
				// Clear the wanted level
				Game.Player.WantedLevel = 0;
				Game.Player.Character.Task.ClearAll();
				AmISurrendering = false;
			}

//////////////////  TESTING -- REMOVE ////////////////// 
			else if (e.KeyCode == Keys.F9)
			{
				// Remove after testking kill... 
				Game.Player.Character.Kill();
			}
			else if (e.KeyCode == Keys.F8)
			{
				// Testing - what is the state of things
				GTA.UI.Notification.Show("Am I Surrendering? " + AmISurrendering.ToString());
			}
//////////////////  TESTING -- END ////////////////// 
		}

		private void Surrender()
		{

			// Ped playerPed = Game.Player.Character;
			int Wanted = Function.Call<int>(Hash.GET_PLAYER_WANTED_LEVEL);
			if (Wanted > 0)
			{
				if (Wanted > 1)
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
					HandsUp();
				}
				else
				{
					// Not in vehicle
					HandsUp();
				}
			}
		}

		private void HandsUp()
		{
			bool AmIArrested = Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED);
			bool AmIWasted = Function.Call<bool>(Hash.IS_PLAYER_DEAD);

				// Inform code I am surrendering
				AmISurrendering = true;

				// If holding a weapon, drop it
				if (DropWeapon == true)
				{
					Function.Call(Hash.SET_PED_DROPS_WEAPON, Game.Player.Character);
				}

				// Put hands up until code breaks
				Game.Player.Character.Task.HandsUp(-1);

			////Attempt to keep wanted level at 1 star
			//while ((AmIArrested == false) && (AmIWasted == false))
			//{
			//	if (AmISurrendering == true)
			//	{
			//		AmIArrested = Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED);
			//		AmIWasted = Function.Call<bool>(Hash.IS_PLAYER_DEAD);
			//		if ((AmIArrested == true) || (AmIWasted == true))
			//		{
			//			// I have been arrested or killed, inform the code and breakout
			//			AmISurrendering = false;
			//			break;
			//		}
			//		else
			//		{
			//			// Attempt to keep wanted level at 1 star
			//			Game.Player.WantedLevel = 1;
			//			Wait(10);

			//		}
			//	}
			//	else
			//	{
			//		// No longer surrendering - break the loop
			//		break;
			//	}
			//}
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