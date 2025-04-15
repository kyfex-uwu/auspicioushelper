



using System;
using System.Collections.Generic;
using System.Reflection;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[Tracked]
public class FoundEntity:Component{
  int id;
  EntityData d;
  public string ident;
  static Dictionary<string, FoundEntity> found = new();
  public FoundEntity(EntityData d, string identifier):base(false,false){
    id=d.ID; this.d=d; ident = identifier;
  }
  public void finalize(Entity e){
    if(e==null){
      DebugConsole.Write($"Failed to find the entity {d.Name} with id {id} - (maybe this entity adds itself non-standardly?)");
      return;
    }
    DebugConsole.Write($"Found the entity {d.Name} with id {id}");
    found[ident] = this;
    e.Add(this);
  }
  public override void EntityRemoved(Scene scene){
    base.EntityRemoved(scene);
    found.Remove(ident);
  }
  public override void Removed(Entity entity){
    base.Removed(entity);
    found.Remove(ident);
  }
  public int reflectGet(List<string> path, List<int> args){
    object o = Entity;
    for(int i=2; i<path.Count; i++){
      if(o == null) return 0;
      Type type = o.GetType();
      FieldInfo field = type.GetField(path[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if(field!=null){
        o=field.GetValue(o); continue;
      }
      PropertyInfo prop = type.GetProperty(path[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if(prop != null){
        o = prop.GetValue(o); continue;
      }
      DebugConsole.Write($"The reflection process on entity {Entity?.ToString()} failed at index {i} looking for {path[i]}");
      return 0;
    }
    try {
      return Convert.ToInt32(o);
    } catch(Exception){
      return o==null?0:1;
    }
  }
  public static void clear(Scene refill = null){
    found.Clear();
    if(refill!=null){
      foreach(FoundEntity f in refill.Tracker.GetComponents<FoundEntity>()){
        found[f.ident] = f;
      }
    }
  }
  public static int sreflectGet(List<string> path, List<int> args){
    if(!found.TryGetValue(path[1], out var f)){
      DebugConsole.Write($"Entity with attached identifier {path[1]} not found");
      return 0;
    }
    return f.reflectGet(path, args);
  }
}