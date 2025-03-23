


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
    actor,
    basic,
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
            parseMap[e.Name] = Types.platformbasic;
            DebugConsole.Write(e.Name +" registered as platformbasic");
            bad=false;
          }else if(t is Actor){
            parseMap[e.Name] = Types.actor;
            DebugConsole.Write(e.Name +" registered as actor");
            bad=false;
          }else{
            parseMap[e.Name] = Types.basic;
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
  public static Entity create(EWrap d, Level l, LevelData ld, Vector2 simoffset, Template t){
    if(d.t == Types.unable) return null;
    
    loaders.TryGetValue(d.d.Name, out var loader);
    if(loader == null && !Level.EntityLoaders.TryGetValue(d.d.Name, out loader)) return null;
    Entity e = loader(l,ld,simoffset,d.d);
    switch(d.t){
      case Types.platformbasic:
        if(e is Platform p){
          return new Wrappers.BasicPlatform(p,t,simoffset+d.d.Position-t.Position);
        }else{
          DebugConsole.Write("Wrongly classified!!! "+d.d.Name);
          return null;
        }
      case Types.actor:
        return e;
      case Types.basic:
        if(e!=null){
          return new Wrappers.BasicEnt(e,t,simoffset+d.d.Position-t.Position);
        }
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
    parseMap["glider"] = Types.actor;
    loaders["glider"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Glider(e,offset);
    parseMap["seekerBarrier"] = Types.platformbasic;
    loaders["seekerBarrier"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new SeekerBarrier(e,offset);
  }
}