


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
  static Dictionary<string, Level.EntityLoader> specialcase = new Dictionary<string, Level.EntityLoader>();
  public static EWrap makeWrapper(EntityData e){
    Types etype;
    if(!parseMap.TryGetValue(e.Name, out etype)){
      DebugConsole.Write(e.Name+" template wrapper not yet implemented");
      parseMap[e.Name] = etype = Types.unable;
    }
    if(etype == Types.unable) return null;
    return new EWrap(e,etype);
  }
  public static Entity create(EWrap d, Level l, LevelData ld, Vector2 simoffset, Template t){
    if(d.t == Types.unable) return null;
    specialcase.TryGetValue(d.d.Name, out var loader);
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
        return null;
      default:
        return null;
    }
  }
  static EntityParser(){
    parseMap["dreamBlock"] = Types.platformbasic;
    specialcase["dreamBlock"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new DreamBlock(e,offset);
    parseMap["jumpThru"] = Types.platformbasic;
    specialcase["jumpThru"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new JumpthruPlatform(e,offset);
    parseMap["glider"] = Types.actor;
    specialcase["glider"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new Glider(e,offset);
    parseMap["seekerBarrier"] = Types.platformbasic;
    specialcase["seekerBarrier"] = (Level l, LevelData ld, Vector2 offset, EntityData e)=>(Entity) new SeekerBarrier(e,offset);
  }
}