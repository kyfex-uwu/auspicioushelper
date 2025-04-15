


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
  public static EWrap makeWrapper(EntityData e){
    Types etype;
    if(!parseMap.TryGetValue(e.Name, out etype)){
      bool bad=true;
      try {
        if(Level.EntityLoaders.TryGetValue(e.Name, out var loader)){
          Entity t = loader(null, null, Vector2.Zero, e);
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
        }
      } catch(Exception ex){
        DebugConsole.Write(e.Name+" failed to be parsed for reason "+ex.ToString());
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
      case Types.platformbasic:
        if(e is Platform p){
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
    loaders["dreamBlock"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new DreamBlock(e,offset);
    parseMap["jumpThru"] = Types.platformbasic;
    loaders["jumpThru"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new JumpthruPlatform(e,offset);
    parseMap["glider"] = Types.unwrapped;
    loaders["glider"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Glider(e,offset);
    parseMap["seekerBarrier"] = Types.platformbasic;
    loaders["seekerBarrier"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new SeekerBarrier(e,offset);
    
    parseMap["spikesUp"] = Types.removeSMbasic;
    loaders["spikesUp"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Spikes(e,offset,Spikes.Directions.Up);
    parseMap["spikesDown"] = Types.removeSMbasic;
    loaders["spikesDown"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Spikes(e,offset,Spikes.Directions.Down);
    parseMap["spikesLeft"] = Types.removeSMbasic;
    loaders["spikesLeft"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Spikes(e,offset,Spikes.Directions.Left);
    parseMap["spikesRight"] = Types.removeSMbasic;
    loaders["spikesRight"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Spikes(e,offset,Spikes.Directions.Right);
    parseMap["triggerSpikesUp"] = Types.removeSMbasic;
    loaders["triggerSpikesUp"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new TriggerSpikes(e,offset,TriggerSpikes.Directions.Up);
    parseMap["triggerSpikesDown"] = Types.removeSMbasic;
    loaders["triggerSpikesDown"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new TriggerSpikes(e,offset,TriggerSpikes.Directions.Down);
    parseMap["triggerSpikesLeft"] = Types.removeSMbasic;
    loaders["triggerSpikesLeft"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new TriggerSpikes(e,offset,TriggerSpikes.Directions.Left);
    parseMap["triggerSpikesRight"] = Types.removeSMbasic;
    loaders["triggerSpikesRight"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new TriggerSpikes(e,offset,TriggerSpikes.Directions.Right);

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
}