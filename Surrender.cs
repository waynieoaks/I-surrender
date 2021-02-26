﻿using System;
using GTA;
using GTA.Native;
using System.Windows.Forms;

namespace Surrender
{
	public class Main : Script
	{
		public static string INIpath = "scripts\\Surrender.ini";
		public static ScriptSettings IniSettings;
		public static Keys SurrenderKey { get; set; }
		public static Keys SurrenderModifierKey { get; set; }
		public static Keys ClearKey { get; set; }
		public static Keys ClearModifierKey { get; set; }
		public static ControllerKeybinds SurrenderBtn { get; set; }
		public static ControllerKeybinds SurrenderModifyBtn { get; set; }
		public static ControllerKeybinds ClearBtn { get; set; }
		public static ControllerKeybinds ClearModifyBtn { get; set; }
		public static bool DropWeapon { get; set; }
		public static bool AmIArrested { get; set; }
		public static bool AmIWasted { get; set; }
		public static bool AmISurrendering { get; set; }
		public static int WantedLvL { get; set; }
		public static string WhoAmI { get; set; }
		public static bool SkipFade { get; set; }

		public Main()
		{
			LoadValuesFromIniFile();

			KeyDown += OnKeyDown;
			Tick += OnControllerDown;
			Tick += ChkWantedTick;

			Interval = 0;
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
		}

		private void OnControllerDown(object sender, EventArgs e)
		{
			if (SurrenderBtn != ControllerKeybinds.None)
			{
				if (SurrenderModifyBtn != ControllerKeybinds.None)
				{
					if (Game.IsControlPressed((GTA.Control)SurrenderModifyBtn) && Game.IsControlJustReleased((GTA.Control)SurrenderBtn))
					{
						if (AmISurrendering == true)
						{
							DoEscape();
						}
						else
						{
							DoSurrender();
						}
					}
				}
				else if (SurrenderModifyBtn == ControllerKeybinds.None && Game.IsControlJustReleased((GTA.Control)SurrenderBtn))
				{
					if (AmISurrendering == true)
					{
						DoEscape();
					}
					else
					{
						DoSurrender();
					}
				} 
			}
			if (ClearBtn != ControllerKeybinds.None)
			{
				if (ClearModifyBtn != ControllerKeybinds.None)
				{
					if (Game.IsControlPressed((GTA.Control)ClearModifyBtn) && Game.IsControlJustReleased((GTA.Control)ClearBtn))
					{
						DoClearWanted();
					}
				}
				else if (ClearModifyBtn == ControllerKeybinds.None && Game.IsControlJustReleased((GTA.Control)ClearBtn))
				{
					DoClearWanted();
				}
			}
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
				Game.Player.WantedLevel = 0;
				Wait(1);
				Game.Player.WantedLevel = 1;
				Wait(1);

				// If in vehicle, get out
				if (Game.Player.Character.IsInVehicle())
				{
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
				Function.Call(Hash.SET_PED_DROPS_INVENTORY_WEAPON, Game.Player.Character, Game.Player.Character.Weapons.Current.Hash, 0.4, 0.7, -0.1, -1);
				Function.Call(Hash.SET_CURRENT_PED_WEAPON, Game.Player.Character, 0xA2719263, true);
			}
			// Put hands up until code breaks
			Game.Player.Character.Task.HandsUp(-1);
		}

		private void ChkWantedTick(object sender, EventArgs e)
		{
			if ((WhoAmI == "0xD7114C9") || (WhoAmI == "0x9B22DBAF") || (WhoAmI == "0x9B810FA2"))
			{
				SkipFade = true;
			} else
			{
				SkipFade = false;
			}

			WantedLvL = Function.Call<int>(Hash.GET_PLAYER_WANTED_LEVEL);
			AmIArrested = Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED);
			AmIWasted = Function.Call<bool>(Hash.IS_PLAYER_DEAD);
			WhoAmI = Game.Player.Character.Model.ToString();

			if (AmIWasted == true)
			{
				// I have been killed
				AmISurrendering = false;

				if (SkipFade == false)
				{
					//Wait for screen to have faded black
					while (GTA.UI.Screen.IsFadedOut == false)
					{
						if (Function.Call<int>(Hash.GET_TIME_SINCE_LAST_DEATH) < 6000)
						{
							Wait(1);
						}
						else
						{
							// Loop has been running too long, break out
							return;
						}
					}

					Wait(4500); // Wait a further 4.5 seconds
					GTA.UI.Screen.FadeIn(5000); // Fade in over 5 seconds
				}

				return;
			}
			else if (WantedLvL > 0)
			{
				if (AmIArrested == true)
				{
					AmISurrendering = false;

					if (SkipFade == false)
					{
						//Wait for screen to have faded black
						while (GTA.UI.Screen.IsFadedOut == false)
						{
							if (Function.Call<int>(Hash.GET_TIME_SINCE_LAST_ARREST) < 6000)
							{
								Wait(1);
							}
							else
							{
								// Loop has been running too long, break out
								return;
							}
						}

						Wait(4500); // Wait a further 4.5 seconds
						GTA.UI.Screen.FadeIn(5000); // Fade in over 5 seconds
					}

					return;
				}
				else if (AmISurrendering == true)
				{
					// Attempt to keep wanted level at 1 star
					Game.Player.WantedLevel = 1;
					return;
				} 
				else
				{
					// I should not be surrendering anyway
					AmISurrendering = false;
					return;
				}
			}
		}

		private void LoadValuesFromIniFile()
		{
			AmISurrendering = false;
			ScriptSettings scriptSettings = ScriptSettings.Load(INIpath);
			SurrenderKey = (Keys)scriptSettings.GetValue<Keys>("Keybinds", "Surrender_Key", Keys.K);
			SurrenderModifierKey = (Keys)scriptSettings.GetValue<Keys>("Keybinds", "Surrender_Modifier", Keys.ControlKey);
			ClearKey = (Keys)scriptSettings.GetValue<Keys>("Keybinds", "Clear_Wanted_Key", Keys.L);
			ClearModifierKey = (Keys)scriptSettings.GetValue<Keys>("Keybinds", "Clear_Wanted_Modifier", Keys.ControlKey);
			SurrenderBtn = (ControllerKeybinds)scriptSettings.GetValue<ControllerKeybinds>("Controller", "Surrender_Btn", ControllerKeybinds.None);
			SurrenderModifyBtn = (ControllerKeybinds)scriptSettings.GetValue<ControllerKeybinds>("Controller", "Surrender_Modifier_Btn", ControllerKeybinds.None);
			ClearBtn = (ControllerKeybinds)scriptSettings.GetValue<ControllerKeybinds>("Controller", "Clear_Wanted_Btn", ControllerKeybinds.None);
			ClearModifyBtn = (ControllerKeybinds)scriptSettings.GetValue<ControllerKeybinds>("Controller", "Clear_Wanted_Modifier_Btn", ControllerKeybinds.None);
			DropWeapon = (bool)scriptSettings.GetValue<bool>("Options", "DropWeapon", true);
		}

		public enum ControllerKeybinds
		{
			None = -1, // 0xFFFFFFFF
			A = 201, // 0x000000C9
			B = 202, // 0x000000CA
			X = 203, // 0x000000CB
			Y = 204, // 0x000000CC
			LB = 226, // 0x000000E2
			RB = 227, // 0x000000E3
			LT = 228, // 0x000000E4
			RT = 229, // 0x000000E5
			LS = 230, // 0x000000E6
			RS = 231, // 0x000000E7
			DPadUp = 232, // 0x000000E8
			DPadDown = 233, // 0x000000E9
			DPadLeft = 234, // 0x000000EA
			DPadRight = 235, // 0x000000EB
		}
	}
}