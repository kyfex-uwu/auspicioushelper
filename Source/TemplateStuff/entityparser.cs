


using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;
public static class EntityParser{
  public enum Types{
    unable,
    platformbasic,
    platformdisobedient,
    unwrapped,
    basic,
    removeSMbasic,
  }
  public class EWrap{
    public EntityData d;
    public Types t;
    public EWrap(EntityData d, Types t){
      this.d=d; this.t=t;
    }
  }
  static Dictionary<string, Types> parseMap = new Dictionary<string, Types>();
  static Dictionary<string, Level.EntityLoader> loaders = new Dictionary<string, Level.EntityLoader>();
  internal static void clarify(string name, Types t, Level.EntityLoader loader){
    if(name.StartsWith("auspicioushelper")) return; //no trolling
    parseMap[name] = t;
    loaders[name] = loader;
  }
  public static EWrap makeWrapper(EntityData e, LevelData ldat = null){
    Types etype;
    if(!parseMap.TryGetValue(e.Name, out etype)){
      bool bad=true;
      try {
        Entity t=null;
        if(Level.EntityLoaders.TryGetValue(e.Name, out var loader)){
          try{
            t = loader(null, ldat, Vector2.Zero, e);
          } catch(Exception ex){
            DebugConsole.Write(e.Name+" entityloader found and failed with cause "+ex.ToString());
          }
        }
        if(t==null && skitzoGuess(e)){
          loader = loaders[e.Name];
          t = loader(null, ldat, Vector2.Zero, e);
        }

        if(t is Platform){
          etype = parseMap[e.Name] = Types.platformbasic;
          DebugConsole.Write(e.Name +" registered as platformbasic");
          bad=false;
        }else if(t is Actor || t is ITemplateChild){
          etype = parseMap[e.Name] = Types.unwrapped;
          DebugConsole.Write(e.Name +" registered as unwrapped");
          bad=false;
        }else{
          etype = parseMap[e.Name] = Types.basic;
          DebugConsole.Write(e.Name +" registered as basic");
          bad=false;
        }
        if(!bad){
          loaders[e.Name] = loader;
        }
      } catch(Exception ex){
        DebugConsole.Write("Other error in template wrapper creation for "+e.Name+" of "+ex.ToString());
      }
      if(bad){
        DebugConsole.Write("template wrapper generator not yet implemented for "+e.Name);
        parseMap[e.Name] = etype = Types.unable;
      }
    }
    if(etype == Types.unable) return null;
    return new EWrap(e,etype);
  }
  public static Entity create(EWrap d, Level l, LevelData ld, Vector2 simoffset, Template t, string path){
    if(d.t == Types.unable) return null;
    
    loaders.TryGetValue(d.d.Name, out var loader);
    if(loader == null && !Level.EntityLoaders.TryGetValue(d.d.Name, out loader)) return null;
    Entity e = loader(l,ld,simoffset,d.d);
    if(e==null) return null;
    else{
      if(path!=null && EntityMarkingFlag.flagged.TryGetValue(path+$"/{d.d.ID}",out var ident)){
        new FoundEntity(d.d,ident).finalize(e);
      }
    }
    switch(d.t){
      case Types.platformbasic: case Types.platformdisobedient:
        if(e is Platform p){
          if(d.t == Types.platformdisobedient)
            return new Wrappers.BasicPlatformDisobedient(p,t,simoffset+d.d.Position-t.virtLoc);
          return new Wrappers.BasicPlatform(p,t,simoffset+d.d.Position-t.virtLoc);
        }else{
          DebugConsole.Write("Wrongly classified!!! "+d.d.Name);
          return null;
        }
      case Types.unwrapped:
        return e;
      case Types.basic:
        if(e!=null){
          t.AddBasicEnt(e,simoffset+d.d.Position-t.virtLoc);
          //return new Wrappers.BasicEnt(e,t,simoffset+d.d.Position-t.Position);
        }
        return null;
      case Types.removeSMbasic:
        List<StaticMover> SMRemove = new List<StaticMover>();
        foreach(Component c in e.Components) if(c is StaticMover sm) SMRemove.Add(sm);
        foreach(StaticMover sm in SMRemove) e.Remove(sm);
        t.AddBasicEnt(e,simoffset+d.d.Position-t.virtLoc);
        return null;
      default:
        return null;
    }
  }
  static EntityParser(){
    parseMap["dreamBlock"] = Types.platformbasic;
    loaders["dreamBlock"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new DreamBlock(e,offset);
    parseMap["jumpThru"] = Types.platformbasic;
    loaders["jumpThru"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new JumpthruPlatform(e,offset);
    parseMap["glider"] = Types.unwrapped;
    loaders["glider"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Glider(e,offset);
    parseMap["seekerBarrier"] = Types.platformbasic;
    loaders["seekerBarrier"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new SeekerBarrier(e,offset);
    
    parseMap["spikesUp"] = Types.removeSMbasic;
    loaders["spikesUp"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Spikes(e,offset,Spikes.Directions.Up);
    parseMap["spikesDown"] = Types.removeSMbasic;
    loaders["spikesDown"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Spikes(e,offset,Spikes.Directions.Down);
    parseMap["spikesLeft"] = Types.removeSMbasic;
    loaders["spikesLeft"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Spikes(e,offset,Spikes.Directions.Left);
    parseMap["spikesRight"] = Types.removeSMbasic;
    loaders["spikesRight"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Spikes(e,offset,Spikes.Directions.Right);
    parseMap["triggerSpikesUp"] = Types.removeSMbasic;
    loaders["triggerSpikesUp"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new TriggerSpikes(e,offset,TriggerSpikes.Directions.Up);
    parseMap["triggerSpikesDown"] = Types.removeSMbasic;
    loaders["triggerSpikesDown"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new TriggerSpikes(e,offset,TriggerSpikes.Directions.Down);
    parseMap["triggerSpikesLeft"] = Types.removeSMbasic;
    loaders["triggerSpikesLeft"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new TriggerSpikes(e,offset,TriggerSpikes.Directions.Left);
    parseMap["triggerSpikesRight"] = Types.removeSMbasic;
    loaders["triggerSpikesRight"] = static (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new TriggerSpikes(e,offset,TriggerSpikes.Directions.Right);
    clarify("spring", Types.removeSMbasic, (Level l, LevelData ld, Vector2 offset, EntityData e)=>new Spring(e,offset,Spring.Orientations.Floor));
    clarify("wallSpringLeft", Types.removeSMbasic, (Level l, LevelData ld, Vector2 offset, EntityData e)=>new Spring(e,offset,Spring.Orientations.WallLeft));
    clarify("wallSpringRight", Types.removeSMbasic, (Level l, LevelData ld, Vector2 offset, EntityData e)=>new Spring(e,offset,Spring.Orientations.WallRight));
    clarify("spinner", Types.unwrapped, static (Level l, LevelData ld, Vector2 offset, EntityData e)=>new Wrappers.Spinner(e,offset));
    
    clarify("lamp",Types.basic,(Level l, LevelData ld, Vector2 offset, EntityData e)=>new Lamp(offset + e.Position, e.Bool("broken")));
    clarify("hangingLamp",Types.basic,(Level l, LevelData ld, Vector2 offset, EntityData e)=>new HangingLamp(e,offset+e.Position));
    clarify("seeker",Types.basic,static (Level l, LevelData d, Vector2 o, EntityData e)=>new Seeker(e, o));
    clarify("dashSwitchH",Types.unwrapped,static (Level l, LevelData d, Vector2 o, EntityData e)=>new Wrappers.DashSwitchW(e,o,new EntityID(d.Name,e.ID)));
    clarify("dashSwitchV",Types.unwrapped,static (Level l, LevelData d, Vector2 o, EntityData e)=>new Wrappers.DashSwitchW(e,o,new EntityID(d.Name,e.ID)));
    clarify("lightning", Types.basic, static (Level l, LevelData d, Vector2 o, EntityData e)=>{
      if(!e.Bool("perLevel") && l.Session.GetFlag("disable_lightning")) return null;
      LightningRenderer lr = l.Tracker.GetEntity<LightningRenderer>();
      if(lr!=null) lr.StartAmbience();
      return new Lightning(e,o);
    });

    parseMap["refill"] = Types.unwrapped;
    loaders["refill"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Wrappers.RefillW(e,offset);
    parseMap["touchSwitch"] = Types.basic;
    loaders["touchSwitch"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new TouchSwitch(e,offset);
    parseMap["strawberry"] = Types.basic;
    loaders["strawberry"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>{
      EntityID id = new EntityID(ld.Name,e.ID);
      DebugConsole.Write("Trying to template berry: "+ id.ToString());
      if(l.Session.DoNotLoad.Contains(id)) return null;
      return (Entity) new Strawberry(e,offset,new EntityID(ld.Name,e.ID));
    };
  }

  static bool skitzoGuess(EntityData d){
    EntityData entity3 = d;
    Vector2 vector = Vector2.Zero;
    switch(d.Name){
      case "jumpThru": loaders["jumpThru"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new JumpthruPlatform(e, o); return true;
      case "refill": loaders["refill"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Refill(e, o); return true;
      case "infiniteStar": loaders["infiniteStar"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FlyFeather(e, o); return true;
      case "strawberry": loaders["strawberry"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Strawberry(e, o, new EntityID(d.Name,e.ID)); return true;
      case "summitgem": loaders["summitgem"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new SummitGem(e, o, new EntityID(d.Name,e.ID)); return true;
      case "fallingBlock": loaders["fallingBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FallingBlock(e, o); return true;
      case "zipMover": loaders["zipMover"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ZipMover(e, o); return true;
      case "crumbleBlock": loaders["crumbleBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new CrumblePlatform(e, o); return true;
      case "dreamBlock": loaders["dreamBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new DreamBlock(e, o); return true;
      case "touchSwitch": loaders["touchSwitch"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new TouchSwitch(e, o); return true;
      case "switchGate": loaders["switchGate"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new SwitchGate(e, o); return true;
      case "negaBlock": loaders["negaBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new NegaBlock(e, o); return true;
      case "key": loaders["key"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Key(e, o, new EntityID(d.Name,e.ID)); return true;
      case "lockBlock": loaders["lockBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new LockBlock(e, o, new EntityID(d.Name,e.ID)); return true;
      case "movingPlatform": loaders["movingPlatform"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new MovingPlatform(e, o); return true;
      case "blockField": loaders["blockField"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new BlockField(e, o); return true;
      case "cloud": loaders["cloud"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Cloud(e, o); return true;
      case "booster": loaders["booster"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Booster(e, o); return true;
      case "moveBlock": loaders["moveBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new MoveBlock(e, o); return true;
      case "light": loaders["light"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new PropLight(e, o); return true;
      case "swapBlock": loaders["swapBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new SwapBlock(e, o); return true;
      case "torch": loaders["torch"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Torch(e, o, new EntityID(d.Name,e.ID)); return true;
      case "seekerBarrier": loaders["seekerBarrier"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new SeekerBarrier(e, o); return true;
      case "theoCrystal": loaders["theoCrystal"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new TheoCrystal(e, o); return true;
      case "glider": loaders["glider"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Glider(e, o); return true;
      case "theoCrystalPedestal": loaders["theoCrystalPedestal"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new TheoCrystalPedestal(e, o); return true;
      case "badelineBoost": loaders["badelineBoost"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new BadelineBoost(e, o); return true;
      case "wallBooster": loaders["wallBooster"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new WallBooster(e, o); return true;
      case "bounceBlock": loaders["bounceBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new BounceBlock(e, o); return true;
      case "coreModeToggle": loaders["coreModeToggle"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new CoreModeToggle(e, o); return true;
      case "iceBlock": loaders["iceBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new IceBlock(e, o); return true;
      case "fireBarrier": loaders["fireBarrier"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FireBarrier(e, o); return true;
      case "eyebomb": loaders["eyebomb"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Puffer(e, o); return true;
      case "flingBird": loaders["flingBird"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FlingBird(e, o); return true;
      case "flingBirdIntro": loaders["flingBirdIntro"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FlingBirdIntro(e, o); return true;
      case "lightningBlock": loaders["lightningBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new LightningBreakerBox(e, o); return true;
      case "sinkingPlatform": loaders["sinkingPlatform"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new SinkingPlatform(e, o); return true;
      case "friendlyGhost": loaders["friendlyGhost"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new AngryOshiro(e, o); return true;
      case "seeker": loaders["seeker"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Seeker(e, o); return true;
      case "seekerStatue": loaders["seekerStatue"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new SeekerStatue(e, o); return true;
      case "slider": loaders["slider"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Slider(e, o); return true;
      case "templeBigEyeball": loaders["templeBigEyeball"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new TempleBigEyeball(e, o); return true;
      case "crushBlock": loaders["crushBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new CrushBlock(e, o); return true;
      case "bigSpinner": loaders["bigSpinner"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Bumper(e, o); return true;
      case "starJumpBlock": loaders["starJumpBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new StarJumpBlock(e, o); return true;
      case "floatySpaceBlock": loaders["floatySpaceBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FloatySpaceBlock(e, o); return true;
      case "glassBlock": loaders["glassBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new GlassBlock(e, o); return true;
      case "goldenBlock": loaders["goldenBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new GoldenBlock(e, o); return true;
      case "fireBall": loaders["fireBall"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FireBall(e, o); return true;
      case "risingLava": loaders["risingLava"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new RisingLava(e, o); return true;
      case "sandwichLava": loaders["sandwichLava"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new SandwichLava(e, o); return true;
      case "killbox": loaders["killbox"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Killbox(e, o); return true;
      case "fakeHeart": loaders["fakeHeart"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FakeHeart(e, o); return true;
      case "finalBoss": loaders["finalBoss"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FinalBoss(e, o); return true;
      case "finalBossMovingBlock": loaders["finalBossMovingBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FinalBossMovingBlock(e, o); return true;
      case "dashBlock": loaders["dashBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new DashBlock(e, o, new EntityID(d.Name,e.ID)); return true;
      case "invisibleBarrier": loaders["invisibleBarrier"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new InvisibleBarrier(e, o); return true;
      case "exitBlock": loaders["exitBlock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ExitBlock(e, o); return true;
      case "coverupWall": loaders["coverupWall"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new CoverupWall(e, o); return true;
      case "crumbleWallOnRumble": loaders["crumbleWallOnRumble"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new CrumbleWallOnRumble(e, o, new EntityID(d.Name,e.ID)); return true;
      case "tentacles": loaders["tentacles"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ReflectionTentacles(e, o); return true;
      case "playerSeeker": loaders["playerSeeker"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new PlayerSeeker(e, o); return true;
      case "chaserBarrier": loaders["chaserBarrier"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ChaserBarrier(e, o); return true;
      case "introCrusher": loaders["introCrusher"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new IntroCrusher(e, o); return true;
      case "bridge": loaders["bridge"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Bridge(e, o); return true;
      case "bridgeFixed": loaders["bridgeFixed"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new BridgeFixed(e, o); return true;
      case "bird": loaders["bird"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new BirdNPC(e, o); return true;
      case "introCar": loaders["introCar"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new IntroCar(e, o); return true;
      case "memorial": loaders["memorial"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Memorial(e, o); return true;
      case "wire": loaders["wire"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Wire(e, o); return true;
      case "cobweb": loaders["cobweb"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Cobweb(e, o); return true;
      case "hahaha": loaders["hahaha"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Hahaha(e, o); return true;
      case "bonfire": loaders["bonfire"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Bonfire(e, o); return true;
      case "colorSwitch": loaders["colorSwitch"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ClutterSwitch(e, o); return true;
      case "resortmirror": loaders["resortmirror"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ResortMirror(e, o); return true;
      case "towerviewer": loaders["towerviewer"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Lookout(e, o); return true;
      case "picoconsole": loaders["picoconsole"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new PicoConsole(e, o); return true;
      case "wavedashmachine": loaders["wavedashmachine"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new WaveDashTutorialMachine(e, o); return true;
      case "oshirodoor": loaders["oshirodoor"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new MrOshiroDoor(e, o); return true;
      case "templeMirrorPortal": loaders["templeMirrorPortal"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new TempleMirrorPortal(e, o); return true;
      case "reflectionHeartStatue": loaders["reflectionHeartStatue"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ReflectionHeartStatue(e, o); return true;
      case "resortRoofEnding": loaders["resortRoofEnding"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ResortRoofEnding(e, o); return true;
      case "gondola": loaders["gondola"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Gondola(e, o); return true;
      case "birdForsakenCityGem": loaders["birdForsakenCityGem"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ForsakenCitySatellite(e, o); return true;
      case "whiteblock": loaders["whiteblock"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new WhiteBlock(e, o); return true;
      case "plateau": loaders["plateau"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Plateau(e, o); return true;
      case "soundSource": loaders["soundSource"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new SoundSourceEntity(e, o); return true;
      case "templeMirror": loaders["templeMirror"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new TempleMirror(e, o); return true;
      case "templeEye": loaders["templeEye"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new TempleEye(e, o); return true;
      case "clutterCabinet": loaders["clutterCabinet"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ClutterCabinet(e, o); return true;
      case "floatingDebris": loaders["floatingDebris"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FloatingDebris(e, o); return true;
      case "foregroundDebris": loaders["foregroundDebris"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ForegroundDebris(e, o); return true;
      case "moonCreature": loaders["moonCreature"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new MoonCreature(e, o); return true;
      case "lightbeam": loaders["lightbeam"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new LightBeam(e, o); return true;
      case "door": loaders["door"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Door(e, o); return true;
      case "trapdoor": loaders["trapdoor"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Trapdoor(e, o); return true;
      case "resortLantern": loaders["resortLantern"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new ResortLantern(e, o); return true;
      case "water": loaders["water"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Water(e, o); return true;
      case "waterfall": loaders["waterfall"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new WaterFall(e, o); return true;
      case "bigWaterfall": loaders["bigWaterfall"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new BigWaterfall(e, o); return true;
      case "clothesline": loaders["clothesline"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new Clothesline(e, o); return true;
      case "cliffflag": loaders["cliffflag"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new CliffFlags(e, o); return true;
      case "cliffside_flag": loaders["cliffside_flag"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new CliffsideWindFlag(e, o); return true;
      case "flutterbird": loaders["flutterbird"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new FlutterBird(e, o); return true;
      case "SoundTest3d": loaders["SoundTest3d"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new _3dSoundTest(e, o); return true;
      case "SummitBackgroundManager": loaders["SummitBackgroundManager"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new AscendManager(e, o); return true;
      case "summitGemManager": loaders["summitGemManager"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new SummitGemManager(e, o); return true;
      case "heartGemDoor": loaders["heartGemDoor"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new HeartGemDoor(e, o); return true;
      case "summitcheckpoint": loaders["summitcheckpoint"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new SummitCheckpoint(e, o); return true;
      case "summitcloud": loaders["summitcloud"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new SummitCloud(e, o); return true;
      case "coreMessage": loaders["coreMessage"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new CoreMessage(e, o); return true;
      case "playbackTutorial": loaders["playbackTutorial"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new PlayerPlayback(e, o); return true;
      case "playbackBillboard": loaders["playbackBillboard"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new PlaybackBillboard(e, o); return true;
      case "cutsceneNode": loaders["cutsceneNode"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new CutsceneNode(e, o); return true;
      case "kevins_pc": loaders["kevins_pc"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new KevinsPC(e, o); return true;
      case "templeGate": loaders["templeGate"]=static (Level l, LevelData d, Vector2 o, EntityData e)=>new TempleGate(e,o, d.Name); return true;
    }
    return false;
  }
  static Level l___;
}