using System.IO;
using System.Collections;
using BepInEx;
using BepInEx.Logging;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace ConsoleCommands;

[BepInPlugin("nl.lunar.modding", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Techtonica.exe")]
//[BepInDependency("Tobey.UnityAudio", BepInDependency.DependencyFlags.SoftDependency)]
public class ConsoleCommands : BaseUnityPlugin
{
	public static new ManualLogSource Logger;
	private string InputText;
	public List<string> InputHistory = new List<string>();
	public List<string> OutputHistory = new List<string>();
	public bool bIsEnabled;
	public const int MaxTotalHistory = 20;
	public static bool bHasScanOverride;
	public static float ScanOverrideMultiplier;
	private void Awake()
	{
		Logger = base.Logger;
		Harmony.CreateAndPatchAll(typeof(OpenSesamePatch));
		Harmony.CreateAndPatchAll(typeof(ScannerPatch));
		Harmony.CreateAndPatchAll(typeof(InstaMolePatch));
		Harmony.CreateAndPatchAll(typeof(AccumulatorPatch));
		Logger.LogInfo("Thanks for downloading and using Techtonica Console Commands!\nThe mod has just finished initializing.");
		Logger.LogWarning("The mod is still in development, bugs and issues may occur.");
		this.gameObject.hideFlags = HideFlags.HideAndDontSave;
	}

 void OnGUI() 
	{
		if(!bIsEnabled) return;
		GUI.SetNextControlName("Console");
		Input.eatKeyPressOnTextFieldFocus = false;
		GUI.skin.textField.fontSize = 20;
        InputText = GUI.TextField(new Rect (0,Screen.height - 50,Screen.width,30), InputText);
    }

	void Update()
    {
		if(Player.instance == null) return; // aka: if in menu do nothing
		if(bIsNoclipping) Player.instance.transform.position = Player.instance.cam.transform.position;
		if(Bindings.Count > 0 && !bIsEnabled)	HandleKeyBinds();
		if(Input.GetKeyDown(KeyCode.Slash)) ToggleConsole();

		if(!bIsEnabled) return;

        //Detect when the Return key is pressed down
        if(Input.GetKeyDown(KeyCode.Return) && InputText != "")
        {
			HandleCommand(InputText);
			UpdateHistory(InputText, false);
			InputText=""; // clear input text so that a new command can be inputted without clearing the previous manually
        }
	}

	void HandleKeyBinds()
	{
		for(int i = 0; i < Bindings.Count; i++)
		{
			if(Input.GetKeyDown(Bindings[i].key))
			{
				if(Bindings[i].args != null) Bindings[i].command.Invoke(this, Bindings[i].args.ToArray<string>());
				else Bindings[i].command.Invoke(this, null);
			}
		}
	}

	void ToggleConsole()
	{
		bIsEnabled = !bIsEnabled;
		InputHandler.instance.uiInputBlocked = bIsEnabled;
	}
	public void UpdateHistory(string TextToAdd, bool bIsOutput)
	{
		if(!bIsOutput)
		{
			if(InputHistory.Count <= MaxTotalHistory/2) InputHistory.Add(TextToAdd);
			else
			{
				InputHistory.RemoveAt(0);
				InputHistory.Add(TextToAdd);
			}
		}
		else
		{
			if(OutputHistory.Count <= MaxTotalHistory/2) OutputHistory.Add(TextToAdd);
			else
			{
				OutputHistory.RemoveAt(0);
				OutputHistory.Add(TextToAdd);
			}
		}
	}
	public void HandleCommand(string UserInput)
	{
		string CommandName = UserInput.ToLower().Split(' ')[0];
		List<string> args = UserInput.Split(' ').ToList<string>();
		args.RemoveAt(0);
		MethodInfo m = GetType().GetMethod(CommandName);
		if(m != null) m.Invoke(this, args.ToArray());
		else DetermineAndLogError(m, UserInput, args);
	}

	void DetermineAndLogError(MethodInfo theMethod, string UserInput, List<string> args)
	{
		if(theMethod == null) LogCommandError("Command '"+UserInput.Split(' ')[0]+"' doesn't exist! Are you sure you typed it correctly?", true);
		else if(args.Count != theMethod.GetParameters().Length) LogCommandError("Missing or obsolete arguments! Expected "+theMethod.GetParameters().Length.ToString()+" arguments, got "+args.Count.ToString()+".", true);

		else LogCommandError("We don't exactly know what went wrong with your command! Please check for mistakes and try again.", true);
	}

	void LogCommandError(string StringToLog, bool bShouldAppearInHistory)
	{
		Logger.LogError(StringToLog);
		if(bShouldAppearInHistory) UpdateHistory(StringToLog, true);
	}

	void LogCommandOutput(string StringToLog, bool bShouldAppearInHistory) // TODO: make all commands output something at the end
	{
		Logger.LogInfo(StringToLog);
		if(bShouldAppearInHistory) UpdateHistory(StringToLog, true);
	}
 	public void give(string item, string amount) // // TODO: add support for all resourceinfos, not only the ones in player's inventory.
	{
		var ResourceTypes = GameDefines.instance.resources;
		if(!int.TryParse(amount, out int count))
		{ 
			LogCommandError("The amount you provided, '"+amount+"', doesn't seem to be a number! Are you sure you typed it correctly?", true);
			return;
		}
		ResourceInfo result = new ResourceInfo();
		for(var i = 0; i < ResourceTypes.Count; i++)
		{
			if(item.ToLower() == ResourceTypes[i].displayName.ToLower().Replace(" ", ""))
			{
				Player.instance.inventory.AddResources(ResourceTypes[i], count);
				result = ResourceTypes[i];
				break;
			}
			if(i == ResourceTypes.Count-1) 
			{
				LogCommandError("The item ('"+item+"') you provided doesn't seem to be correct! Try another name.", true);
				return;
			}
		}
		LogCommandOutput(amount+" of "+result.displayName+" has been given to player.", true);
	}

	public void echo(string logstring, string logtype)
	{
		switch(logtype.ToLower())
		{
			case "info":
				Logger.LogInfo(logstring);
				break;
			case "warning":
				Logger.LogWarning(logstring);
				break;
			case "error":
				Logger.LogError(logstring);
				break;
			case "fatal":
				Logger.LogFatal(logstring);
				break;
			case "message":
				Logger.LogMessage(logstring);
				break;
			default:
				LogCommandError("Unrecognized log type! Choose from: info, warning, error, fatal, or message", true);
				break;
		}
		LogCommandOutput("Logged '"+logstring+"'.", true);
	}

	// * Done!
	// // TODO: redo this with new Output/Input history system
	
	public string GetHistory()
	{
		string s = "";
		for(int i = 0; i < OutputHistory.Count+InputHistory.Count; i++)
		{
			if(i % 2 == 1) // i is odd
			{
				s += "\n"+OutputHistory[(int)i/2];
			}
			else if(i % 2 == 0) // i is even
			{
				s += "\n"+InputHistory[(int)i/2];
			}
		}
		return s;
	}

	public void setplayerparams(string paramtype, string value)
	{
		if(!float.TryParse(value, out float v))
		{
			LogCommandError("Unrecognized value '"+value+"'! Are you sure you typed it correctly?", true);
			return;
		}
		switch(paramtype.ToLower())
		{
			case "maxrunspeed":
				PlayerFirstPersonController.instance.maxRunSpeed = v;
				break;
			case "maxwalkspeed":
				PlayerFirstPersonController.instance.maxWalkSpeed = v;
				break;
			case "maxflyspeed":
				PlayerFirstPersonController.instance.maxFlySpeed = v;
				break;
			case "jumpspeed":
				PlayerFirstPersonController.instance.jumpSpeed = v;
				break;
			case "scanspeed":
				bHasScanOverride = true;
				ScanOverrideMultiplier = 1/v;
				break;
			case "gravity":
				PlayerFirstPersonController.instance.gravity = v;
				break;
			case "maxflyheight":
				Player.instance.equipment.hoverPack._stiltHeight = v;
				break;
			case "railrunnerspeed":
				Player.instance.equipment.railRunner._hookSpeed = v;
				break;
			default:
				LogCommandError("Unrecognized type '"+paramtype+"'! Are you sure you typed it correctly?\nPossible options are: maxrunspeed, maxwalkspeed, maxflyspeed, maxjumpvelocity, scanspeed, gravity, maxjumpheight", true);
				return;
		}
		LogCommandOutput("Player param '"+paramtype+"' set with value '"+value+"'. ", true);
	}

	public void weightless()
	{
		Player.instance.cheats.disableEncumbrance = !Player.instance.cheats.disableEncumbrance;
		if(Player.instance.cheats.disableEncumbrance) LogCommandOutput("Enabled weightlessness.", true);
		else LogCommandOutput("Disabled weightlessness.", true);
	}

	public void echolocation()
	{
		echo(PlayerFirstPersonController.instance.transform.position.ToString(), "info");
	}

	// * this wouldn't have been useful anyway
	// public void noclip(string value)
	// {
	// 	if(!bool.TryParse(value, out bool b)) LogCommandError("Invalid bool '"+value+"'! Are you sure you typed it correctly?", true);
	// 	bHasCollisionOverride = b;
	// }
	
	public void tp(string X, string Y, string Z)
	{
		if((float.TryParse(X, out float ix) || X == "~") && (float.TryParse(Y, out float iy) || Y == "~") && (float.TryParse(Z, out float iz) || Z == "~"))
		{
			if(X == "~") ix = PlayerFirstPersonController.instance.transform.position.x;
			else if(Y == "~") iy = PlayerFirstPersonController.instance.transform.position.y;
			else if(Z == "~") iz = PlayerFirstPersonController.instance.transform.position.z;
			PlayerFirstPersonController.instance.transform.position = new Vector3(ix, iy, iz);
			LogCommandOutput("Teleported player to "+X+", "+Y+", "+Z+".", true);
		}
		else LogCommandError("Your three vector components dont seem to be valid! ('"+X+"', '"+Y+"', '"+Z+"')", true);
	}

	public void warp(string Location)
	{
		switch(Location.ToLower())
		{
			case "victor":
				tp("138,00", "12,30", "-116,00");
				break;
			case "lima":
				tp("85,00", "-2,84", "-330,00");
				break;
			case "xray":
				tp("-307,57", "92,95", "20,11");
				break;
			case "freight":
				tp("-153,08", "36,30", "188,50");
				break;
			case "waterfall":
				tp("-265,88", "-17,85", "-131,33");
				break;
			default:
				LogCommandError("Your warp, '"+Location+"', doesn't seem to exist! Check info.txt for all possible warps.", true);
				return;
		}
		LogCommandOutput("Teleported to "+Location+".", true);
	}

 	// public const string WarpTXTPath = "C:\"
	//  public void setwarp(string name)
	// {
	// 	if(!File.Exists(WarpTXTPath)) File.Create(WarpTXTPath);
	// 	string filetext = File.ReadAllText(WarpTXTPath);
	// 	File.WriteAllText(filetext+Environment.NewLine+name+Environment.NewLine+Player.instance.transform.position, WarpTXTPath);
	// }

	// public void delwarp(string name)
	// {
	// 	string filetext = File.ReadAllText(WarpTXTPath);
	// }
	public void unlock(string name, string DrawPower)
	{
		if(!bool.TryParse(DrawPower, out bool b))
		{ 
			LogCommandError("The bool you provided, '"+DrawPower+"', doesn't seem to be valid!", true);
			return;
		}
		if(name.ToLower() == "all") UnlockAll(b);
		var unlocks = GameDefines.instance.unlocks;
		for(var i = 0; i < unlocks.Count; i++)
		{
			if(LocsUtility.TranslateStringFromHash(unlocks[i].displayNameHash, null) == null) continue;
			if(name.ToLower() == LocsUtility.TranslateStringFromHash(unlocks[i].displayNameHash, null).ToLower().Replace(" ", ""))
			{
				ResearchTechNoReq(unlocks[i].uniqueId, b);
				if(!b) LogCommandOutput("Unlocked tech "+unlocks[i].displayName+" without drawing power.", true);
				else LogCommandOutput("Unlocked tech "+unlocks[i].displayName+".", true);
				break;
			}
			if(i == unlocks.Count-1) LogCommandError("The name ('"+name+"') you provided doesn't seem to be correct! Try another name.", true);
		}
	}

	private void UnlockAll(bool DrawPower)
	{
		var unlocks = GameDefines.instance.unlocks;
		for(var i = 0; i < unlocks.Count; i++)
		{
			ResearchTechNoReq(unlocks[i].uniqueId, DrawPower);
		}
		LogCommandOutput("Unlocked all tech!", true);
	}
	
	private void ResearchTechNoReq(int unlockId, bool b)
	{
		var action = new UnlockTechAction
		{
			info = new UnlockTechInfo
			{
				unlockID = unlockId,
				drawPower = b
			}
		};
		NetworkMessageRelay.instance.SendNetworkAction(action);
	}

	public void opensesame()
	{
		// ResourceGateInstance rgi;
		if(!GetClosestDoor(8, out uint doorid, out ResourceGateInstance rgi))
		{
			LogCommandError("You're not looking at a door!", true);
			return;
		}
		DoorToOpen = rgi;
		AddRequiredResources(rgi);
		OpenDoor(doorid, rgi); // this won't work as further in network code the resources are checked again, so we must set the required resources before we do this.
		rgi.ProcessUpgrade();
		rgi.interactionState = 2;
		LogCommandOutput("Opened door "+rgi.myConfig.displayName+".", true);
	}

	public static ResourceGateInstance DoorToOpen;
	private bool GetClosestDoor(float MaxDist, out uint id, out ResourceGateInstance rgi)
	{
		var doors = MachineManager.instance.GetMachineList<ResourceGateInstance, ResourceGateDefinition>(MachineTypeEnum.ResourceGate);
        float closestDoor = float.MaxValue;
        ResourceGateInstance gate = doors.myArray[0];
        foreach (var door in doors.myArray)
        {
            var center = door.gridInfo.Center;
            var distanceToDoor = center.Distance(Player.instance.cam.transform.position);
            if(distanceToDoor < closestDoor)
            {
                Logger.LogInfo($" Found closer door {door.gridInfo.myRef.instanceId} is {distanceToDoor} away.");
                closestDoor = distanceToDoor;
                gate = door;
            }
            else
            {
                Logger.LogInfo($" Found further door {door.gridInfo.myRef.instanceId} is {distanceToDoor} away.");
            }
        }
		rgi = gate;
		id = gate.gridInfo.myRef.instanceId;
		if(closestDoor > MaxDist) return false;
		else return true;
	}
	private void OpenDoor(uint id, ResourceGateInstance rgi)
	{
		if (rgi.CheckForRequiredResources())
		{
			CompleteResourceGateAction action = new CompleteResourceGateAction
			{
				info = new CompleteResourceGateInfo
				{
					machineId = rgi.gridInfo.myRef.instanceId,
					unlockLevel = 1
				}
			};
			NetworkMessageRelay.instance.SendNetworkAction(action);
			Player.instance.audio.productionTerminalTierUpgrade.PlayRandomClip(true);
			return;
		}
		Player.instance.audio.error.PlayRandomClip(true);
	}

	private void AddRequiredResources(ResourceGateInstance rgi)
	{
		for(var i = 0; i < rgi.resourcesRequired.Length; i++)
		{
			// rgi.AddResources(rgi.resourcesRequired[i].resType.uniqueId, out int remainder, rgi.resourcesRequired[i].quantity);
			rgi.GetInputInventory().AddResourcesToSlot(rgi.resourcesRequired[i].resType.uniqueId, i, out int remainder, rgi.resourcesRequired[i].quantity, true);
		}
	}

	public static bool bShouldInstaMine;
	public void instamole() // * functionality handled in InstaMolePatch.cs
	{
		bShouldInstaMine = !bShouldInstaMine;
		if(bShouldInstaMine) LogCommandOutput("Enabled instamine.", true);
		else LogCommandOutput("Disabled instamine.", true);
	}

	public void gamespeed(string value) // * Built in simspeed variable in PlayerCheats.cs
	{
		if(!float.TryParse(value, out float v))
		{
			LogCommandError("The float parameter you provided, '"+value+"', doesn't seem to be valid!", true);
			return;
		}
		Player.instance.cheats.simSpeed = v;
		LogCommandOutput("Correctly set game simulation speed to "+value+"!", true);
	}

	public void cammode(string value)
	{
		if(!ParseCamMode(value, out PlayerCheats.FreeCameraMode fcm))
		{
			LogCommandError("The camera mode you inputted, '"+value+"', doesn't seem to be valid!", true);
			return;
		}
		Player.instance.cheats.freeCameraMode = fcm;
		LogCommandOutput("New camera mode set: "+value+".", true);
	}
	

	// ! This can be replaced with (PlayerCheats.FreeCameraMode) Enum.Parse(typeof(PlayerCheats.FreeCameraMode), string)!
	public bool ParseCamMode(string value, out PlayerCheats.FreeCameraMode fcm)
	{
		switch(value.ToLower())
		{
			case "normal":
				fcm =  PlayerCheats.FreeCameraMode.Normal;
				return true;
			case "free":
				fcm =  PlayerCheats.FreeCameraMode.Free;
				return true;
			case "scriptedanimation":
				fcm = PlayerCheats.FreeCameraMode.ScriptedAnimation;
				return true;
			default:
				fcm = PlayerCheats.FreeCameraMode.Normal;
				return false;
		}
	}

	public static bool bShouldFillAccumulators;

	public void fillaccumulators()
	{
		bShouldFillAccumulators = true;
	}

	public void camtp()
	{
		if(Player.instance.cheats.freeCameraMode == PlayerCheats.FreeCameraMode.Free)
		{
			Player.instance.transform.position = Player.instance.cam.transform.position;
			LogCommandOutput("Succesfully teleported to the freecam position!", true);
		}
		else LogCommandError("You're not in free camera mode!", true);
	}

	public bool bIsNoclipping;
	public void noclip()
	{
		if(!bIsNoclipping)
		{
			Player.instance.cheats.freeCameraMode = PlayerCheats.FreeCameraMode.Free;
			bIsNoclipping = true;
			LogCommandOutput("Noclip has been enabled!", true);
		}
		else
		{
			Player.instance.cheats.freeCameraMode = PlayerCheats.FreeCameraMode.Normal;
			bIsNoclipping = false;
			LogCommandOutput("Noclip has been disabled!", true);
		}
	}

	struct CommandKeyBindData
	{
		public KeyCode key;
		public MethodInfo command;
		public List<string> args;
	}

	private List<CommandKeyBindData> Bindings = new List<CommandKeyBindData>();

	// command formatting: CommandName{arg1,arg2,arg3,etc.}
	public void bind(string key, string command) // bind a hotkey to a command
	{
		// System.Enum.Parse(typeof(KeyCode), key)

		if(Enum.IsDefined(typeof(KeyCode), key)) // is key valid
		{
			KeyCode keybind;
			CommandKeyBindData BindingData = new CommandKeyBindData();
			List<string> args = null;
			string commandname;
			keybind = (KeyCode) Enum.Parse(typeof(KeyCode), key);
			if(command.Contains("{") || command.Contains("}"))
			{
				commandname = command.Substring(0, command.IndexOf('{'));
				args = command.Substring(command.IndexOf('{'), command.IndexOf('}')-command.IndexOf('{')).Replace(",", " ").Replace("}", "").Replace("{", "").Split(" ").ToList<string>();
				Debug.Log(args[0]);
			}
			else commandname = command;
			if(GetType().GetMethod(commandname) == null)
			{
				LogCommandError("The command you inputted, "+commandname+", doesn't exist!", true);
				return;
			}
			if(Bindings.Find(BindingData => BindingData.key == keybind).command == null) // check if the key has already been bound to something (true if not)
			{
				BindingData.key = keybind;
				BindingData.command = GetType().GetMethod(commandname.ToLower());
				if(command.Contains("{") || command.Contains("}")) BindingData.args = args;
				Bindings.Add(BindingData);
				LogCommandOutput("Bound "+key+" to '"+command+"'!", true);
			}
			else
			{
				LogCommandError("This key has already been bound to another command! Use 'unbind' to unbind keys!", true);
				return;
			}
		}
		else LogCommandError("The key you provided, '"+key+"', isn't valid!", true);
	}

	public void unbind(string key) // unbind a hotkey
	{
		if(!Enum.IsDefined(typeof(KeyCode), key))
		{
			LogCommandError("The key you provided, '"+key+"', isn't valid!", true);
			return;
		}
		KeyCode keybind = (KeyCode) Enum.Parse(typeof(KeyCode), key);
		if(Bindings.Find(BindingData => BindingData.key == keybind).command != null)
		{
			Bindings.Remove(Bindings.Find(BindingData => BindingData.key == keybind));
			LogCommandOutput("Key '"+keybind.ToString()+"' has been unbound!", true);
		}
		else LogCommandError("That key isn't bound to anything!", true);
	}

	public void setsize(string value, string bSyncParams)
	{
		if(!float.TryParse(value, out float f))
		{
			LogCommandError("The float you provided isn't valid!", true);
			return;
		}
		if(f <= 0)
		{
			LogCommandError("To prevent game-breaking bugs, scales below 0 are not accepted.", true);
			return;
		}
		if(!bool.TryParse(bSyncParams, out bool b))
		{
			LogCommandError("The bool you provided isn't valid!", true);
			return;
		}
		Player.instance.transform.localScale = new Vector3(f, f, f);
		Player.instance.cam.transform.localScale = new Vector3(f, f, f);
		if(b) ScalePlayerParamsToNewSize(f);
		LogCommandOutput("Set scale to "+f.ToString()+"!", true);
	}
	void ScalePlayerParamsToNewSize(float newsize)
	{
		PlayerFirstPersonController.instance.maxWalkSpeed = 5f * newsize;
		PlayerFirstPersonController.instance.maxRunSpeed = 8f * newsize;
		PlayerFirstPersonController.instance.peakMinHeight = 1f * newsize;
		PlayerFirstPersonController.instance.peakMaxHeight = 2f * newsize;
		PlayerFirstPersonController.instance.inAirDuration = 2f * newsize;
		PlayerFirstPersonController.instance.gravity = 20f * newsize;
		PlayerFirstPersonController.instance.maxFallSpeed = -15f * newsize;
		Player.instance.equipment.hoverPack._stiltHeight = 3f * newsize;
		Player.instance.equipment.hoverPack._raiseSpeed = 5f * newsize;
	}

	public void clear(string item, string amount)
	{
		ResourceInfo result = new ResourceInfo();
		for(int i = 0; i < GameDefines.instance.resources.Count; i++)
		{
			if(GameDefines.instance.resources[i].displayName.Replace(" ", "").ToLower() == item.ToLower())
			{
				result = GameDefines.instance.resources[i];
				break;
			}
			if(i == GameDefines.instance.resources.Count-1) LogCommandError("The item you provided doesn't seem to exist!", true);
		}
		if(int.TryParse(amount, out int a))
		{
			Player.instance.inventory.TryRemoveResources(result, a);
			LogCommandOutput("Removed "+a.ToString()+" of "+result.displayName+" from player's inventory.", true);
		}
		else if(amount.ToLower() == "all")
		{
			Player.instance.inventory.TryRemoveResources(result, Player.instance.inventory.GetResourceCount(result.uniqueId));
			LogCommandOutput("Removed all of "+result.displayName+" from player's inventory.", true);
		}
		else LogCommandError("The amount you provided doesn't seem to be valid!", true);
	}
	public void setmoledimensions(string valuex, string valuey, string valuez)
	{
		if(int.TryParse(valuex, out int x) && int.TryParse(valuey, out int y) && int.TryParse(valuez, out int z))
		{
			TerrainManipulator tm = Player.instance.equipment.GetAllEquipment<TerrainManipulator>()[0];
			tm.tunnelMode._currentDimensions.x = x;
			tm.tunnelMode._currentDimensions.y = y;
			tm.tunnelMode._currentDimensions.z = z;
			tm.flattenMode._currentDimensions.x = x;
			tm.flattenMode._currentDimensions.y = y;
			tm.flattenMode._currentDimensions.z = z;
			LogCommandOutput("Set dimension to "+x.ToString()+", "+y.ToString()+", "+z.ToString()+"!", true);
		}
		else LogCommandError("The integer you provided does not seem to be valid!", true);
	}
}
