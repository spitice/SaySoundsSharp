﻿using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

namespace SaySoundsSharp;

public class SaySoundsSharp: BasePlugin {

    public override string ModuleName => "SaySoundsSharp";

    public override string ModuleVersion => "0.1.0";

    public override string ModuleDescription => "CounterStrikeSharp implementation of SaySounds";

    public override string ModuleAuthor => "faketuna, Spitice";

    private SaySoundConfig saySoundConfig = default!;
    private string CHAT_PREFIX = $" {ChatColors.Purple}[SaySounds]{ChatColors.Default}";

    private FakeConVar<string> soundPath = new("saysounds_sound_path", "Sound path of say sound", "soundevents/soundevents_saysounds.vsndevts");

    private const string saySoundMessageFormat = "%player% played %soundname%";

    public override void Load(bool hotReload) {
        AddCommandListener("say", CommandListener_Say);
        AddCommandListener("say_team", CommandListener_SayTeam);
        saySoundConfig = new SaySoundConfig(this.ModuleDirectory + "/config/saysounds.txt");
        RegisterListener<Listeners.OnServerPrecacheResources>((ResourceManifest res) => {
            res.AddResource(soundPath.Value);
        });

        AddCommand("css_sss", "Finds a SaySounds that contains the specified argument.", CommandSaySoundSearch);
    }

    private HookResult CommandListener_Say(CCSPlayerController? client, CommandInfo commandInfo) {
        if(client == null)
            return HookResult.Continue;
        
        
        UserSaySoundInput saySound = SaySoundUtil.processUserInput(commandInfo.ArgString);

        string? sound = saySoundConfig!.saySounds!.GetValueOrDefault(saySound.soundName, null);

        if(sound == null)
            return HookResult.Continue;

        playSaySound(client, saySound);

        printSaySoundNotification(client, saySound);
        return HookResult.Handled;
    }

    private HookResult CommandListener_SayTeam(CCSPlayerController? client, CommandInfo commandInfo) {
        if(client == null)
            return HookResult.Continue;
        
        
        UserSaySoundInput saySound = SaySoundUtil.processUserInput(commandInfo.ArgString);

        string? sound = saySoundConfig!.saySounds!.GetValueOrDefault(saySound.soundName, null);

        if(sound == null)
            return HookResult.Continue;

        playSaySound(client, saySound);

        printSaySoundNotification(client, saySound);
        return HookResult.Handled;
    }


    private void CommandSaySoundSearch(CCSPlayerController? client, CommandInfo commandInfo) {
        if(client == null)
            return;
        
        string arg1 = commandInfo.GetArg(1);


        if(arg1.Equals("", StringComparison.OrdinalIgnoreCase)) {
            client.PrintToChat($"{CHAT_PREFIX} usage: css_sss <sound name>");
            return;
        }

        if(arg1.Length < 3) {
            client.PrintToChat($"{CHAT_PREFIX} You need specify at least 3 characters!");
            return;
        }

        List<string> searchResult = new ();

        foreach(var sound in saySoundConfig.saySounds) {
            if(sound.Key.Contains(arg1, StringComparison.OrdinalIgnoreCase))
                searchResult.Add(sound.Key);
        }


        if(searchResult.Count() == 0) {
            client.PrintToChat($"{CHAT_PREFIX} No sounds found with {arg1}.");
            return;
        }

        client.PrintToChat($"{CHAT_PREFIX} {searchResult.Count()} sounds found. See client console to full list");
        foreach(string sound in searchResult) {
            client.PrintToConsole(sound);
        }

    }

    private void playSaySound(CCSPlayerController client, UserSaySoundInput saySound) {
        var soundEvtName = "saysounds." + saySound.soundName;
        soundEvtName = soundEvtName.ToLower();
        soundEvtName = soundEvtName.Replace("!", "_EXCL_");
        soundEvtName = soundEvtName.Replace("-", "_MIN_");
        soundEvtName = soundEvtName.Replace("+", "_PLS_");
        soundEvtName = soundEvtName.Replace(" ", "_SPC_");
        soundEvtName = soundEvtName.Replace("~", "_TILD_");
        soundEvtName = soundEvtName.Replace("^", "_CARET_");
        soundEvtName = soundEvtName.Replace("(", "_LPAR_");
        soundEvtName = soundEvtName.Replace(")", "_RPAR_");

        if (saySound.isPitchModified)
        {
            soundEvtName += ".p" + saySound.pitch.ToString();
        }


        for (var i = 65; i < 69; i++)
        {
        var world = Utilities.GetEntityFromIndex<CCSTeam>(65)!;
        Console.WriteLine(world.DesignerName);
        world.EmitSound(soundEvtName);

        }
    }

    private void printSaySoundNotification(CCSPlayerController client, UserSaySoundInput saySound) {
        var msg = " " + saySoundMessageFormat.Replace("%player%", $"{ChatColors.LightPurple}{client.PlayerName}{ChatColors.Default}").Replace("%soundname%", $"{ChatColors.Lime}{saySound.soundName}{ChatColors.Default}");

        if (saySound.isPitchModified)
        {
            msg += $" @{ChatColors.Red}{saySound.pitch}{ChatColors.Default}";
        }

        foreach (CCSPlayerController cl in Utilities.GetPlayers()) {
            if(!cl.IsValid || cl.IsBot || cl.IsHLTV)
                continue;

            cl.PrintToChat(msg);
        }
    }
}