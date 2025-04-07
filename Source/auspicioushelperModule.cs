using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public override Type SaveDataType => typeof(auspicioushelperModuleSaveData);
    public static auspicioushelperModuleSaveData SaveData=> (auspicioushelperModuleSaveData) Instance._SaveData;

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
    public static ActionList OnEnterMap = new ActionList();
    public static ActionList OnNewScreen = new ActionList();
    public static ActionList OnReset = new ActionList();

    public override void Load() {
        Everest.Events.Level.OnTransitionTo += OnTransition;
        Everest.Events.Player.OnDie += OnDie;
        Everest.Events.Level.OnEnter += OnEnter;
        Everest.Events.Level.OnLoadLevel += EverestOnLoadLevel;
        Everest.Events.AssetReload.OnAfterReload += OnReload;

        On.Celeste.ChangeRespawnTrigger.OnEnter += ChangerespawnHandler;
        DebugConsole.Write("Loading");
        ConditionalStrawb.hooks.enable();
        //TrackedCassette.hooks.enable();
    }
    public static void tinyCleanup(){
        PortalGateH.intersections.Clear();
    }
    void OnTransition(Level level, LevelData next, Vector2 direction){
        Session.save();
        ChannelState.unwatchTemporary();
        tinyCleanup();

        OnReset.run();
        OnNewScreen.run();
    } 
    static void ChangerespawnHandler(On.Celeste.ChangeRespawnTrigger.orig_OnEnter orig, ChangeRespawnTrigger self, Player player){
        orig(self, player);
        //if ((!session.RespawnPoint.HasValue || session.RespawnPoint.Value != Target))
        Session session = (self.Scene as Level).Session;
        if(!session.RespawnPoint.HasValue || session.RespawnPoint.Value != self.Target){
            Session.save();
        }
    }
    static void OnDie(Player player){
        Session.load(false);
        ChannelState.unwatchAll();
        ConditionalStrawb.handleDie(player);
        tinyCleanup();

        OnReset.run();
    }
    static void OnEnter(Session session, bool fromSave){
        Session?.load(!fromSave);
        ChannelState.unwatchAll();
        JumpListener.releaseHooks();
        portalHooks.unsetupHooks();
        MarkedRoomParser.parseMapdata(session.MapData);
        DebugConsole.Write("Entered Level");

        OnReset.run();
        OnNewScreen.run();
        OnEnterMap.run();
    }
    static void OnReload(bool silent){
        DebugConsole.Write($"reloaded {Everest.Content.Map.Count} {Settings.hideHelperMaps}");
        //foreach (ModAsset item in Everest.Content.Map.Values.Where((ModAsset asset) => asset.Type == typeof(AssetTypeMap)))
        if(Settings.hideHelperMaps)MapHider.hideListed();
        //DebugConsole.Write(Engine.Instance.scene.ToString());
        ChannelState.unwatchAll();
        if(Engine.Instance.scene is LevelLoader l){
            MarkedRoomParser.parseMapdata(l.Level.Session.MapData);
        }
        DebugConsole.Write(Engine.Scene.ToString());
        if(Session != null){
            try{
                Session.load(false);
            } catch(Exception ex){
                DebugConsole.Write($"reloading error: {ex}");
            }
        }
    }
    static void EverestOnLoadLevel(Level level, Player.IntroTypes t, bool fromLoader){
        //trash is called after constructors
        MaterialPipe.redoLayers();
    }

    public override void LoadContent(bool firstLoad){
        base.LoadContent(firstLoad);
        SpeedrunToolIop.hooks.enable();
        auspicioushelperGFX.loadContent();
        MaterialPipe.setup();
    }
    

    public override void Unload() {
        Everest.Events.Level.OnTransitionTo -= OnTransition;
        Everest.Events.Player.OnDie -= OnDie;
        Everest.Events.Level.OnEnter -= OnEnter;
        Everest.Events.Level.OnLoadLevel -= EverestOnLoadLevel;
        Everest.Events.AssetReload.OnAfterReload -= OnReload;

        HookManager.disableAll();
        DebugConsole.Close();
        On.Celeste.ChangeRespawnTrigger.OnEnter -= ChangerespawnHandler;
    }
}