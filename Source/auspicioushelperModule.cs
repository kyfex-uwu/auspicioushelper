using System;
using System.Diagnostics;
using Celeleste.Mods.auspicioushelper;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class auspicioushelperModule : EverestModule {
    public static auspicioushelperModule Instance { get; private set; }

    public override Type SessionType => typeof(auspicioushelperModuleSession);
    public static auspicioushelperModuleSession Session => (auspicioushelperModuleSession) Instance._Session;
    public override Type SettingsType => typeof(auspicioushelperModuleSettings);
    public static auspicioushelperModuleSettings Settings => (auspicioushelperModuleSettings) Instance._Settings;

    public auspicioushelperModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(auspicioushelperModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(auspicioushelperModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        Everest.Events.Level.OnTransitionTo += OnTransition;
        On.Celeste.ChangeRespawnTrigger.OnEnter += ChangerespawnHandler;
        Everest.Events.Player.OnDie += OnDie;
        Everest.Events.Level.OnEnter += OnEnter;
        Everest.Events.Level.OnLoadLevel += EverestOnLoadLevel;

        On.Celeste.Booster.PlayerBoosted += ChannelBooster.PlayerboostHandler;
        On.Celeste.Booster.PlayerDied += ChannelBooster.PlayerdieHandler;
        On.Celeste.Booster.PlayerReleased += ChannelBooster.PlayerreleaseHandler;
        On.Celeste.Player.ctor += ConditionalStrawb.playerCtorHook;

        //DebugConsole.Open();  

        //EntityBinder.addHooks(); 
    }
    public void OnTransition(Level level, LevelData next, Vector2 direction){
        Session.save();
        ChannelState.unwatchTemporary();
    } 
    public static void ChangerespawnHandler(On.Celeste.ChangeRespawnTrigger.orig_OnEnter orig, ChangeRespawnTrigger self, Player player){
        orig(self, player);
        //if ((!session.RespawnPoint.HasValue || session.RespawnPoint.Value != Target))
        Session session = (self.Scene as Level).Session;
        if(!session.RespawnPoint.HasValue || session.RespawnPoint.Value != self.Target){
            Session.save();
        }
    }
    public static void OnDie(Player player){
        Session.load(false);
        ChannelState.unwatchAll();
        ConditionalStrawb.handleDie(player);
    }
    public static void OnEnter(Session session, bool fromSave){
        Session.load(!fromSave);
        ChannelState.unwatchAll();
        JumpListener.releaseHooks();
        DebugConsole.Write("Entered Level");
    }
    public static void EverestOnLoadLevel(Level level, Player.IntroTypes t, bool fromLoader){
        //trash is called after constructors
        MaterialPipe.redoLayers();
    }

    public override void LoadContent(bool firstLoad){
        base.LoadContent(firstLoad);
        ChannelState.speedruntoolinteropload();
        auspicioushelperGFX.loadContent();
        MaterialPipe.setup();
    }
    

    public override void Unload() {
        Everest.Events.Level.OnTransitionTo -= OnTransition;
        On.Celeste.ChangeRespawnTrigger.OnEnter -= ChangerespawnHandler;
        Everest.Events.Player.OnDie -= OnDie;
        Everest.Events.Level.OnEnter -= OnEnter;
        Everest.Events.Level.OnLoadLevel -= EverestOnLoadLevel;

        On.Celeste.Booster.PlayerBoosted -= ChannelBooster.PlayerboostHandler;
        On.Celeste.Booster.PlayerDied -= ChannelBooster.PlayerdieHandler;
        On.Celeste.Booster.PlayerReleased -= ChannelBooster.PlayerreleaseHandler;
        On.Celeste.Player.ctor -= ConditionalStrawb.playerCtorHook;

        DebugConsole.Close();
    }
}