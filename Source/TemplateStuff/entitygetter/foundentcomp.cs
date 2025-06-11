



using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    DebugConsole.Write($"Found the entity {d.Name} with id {id} - position {e.Position}");
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
  public static object reflectGet(Entity e, List<string> path, List<int> args, int startidx = 2, bool toSet=false){
    MemberInfo lastInfo = null;
    object previousObj = null;
    object o = e;
    int j=toSet?1:0;
    for(int i=startidx; i<path.Count; i++){
      if(o == null) return 0;
      Type type = o.GetType();
      if(path[i] == "__index__"){
        if(o is IList list) o = list[args[j]];
        else if(o is IEnumerable enumer){
          int k=0;
          o=null;
          foreach(object c in enumer){
            if(k++ == args[j]){
              o=c; break;
            }
          }
        }
        j++;continue;
      }
      FieldInfo field = type.GetField(path[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if(field!=null) {
        lastInfo = field;
        previousObj = o;
        o=field.GetValue(o); continue;
      }
      PropertyInfo prop = type.GetProperty(path[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if(prop != null){
        //DebugConsole.Write($"Getting prop {path[i]} {o.ToString()}")
        lastInfo = prop;
        previousObj = o;
        o = prop.GetValue(o); continue;
      }
      MethodInfo method = type.GetMethod(path[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
      if (method != null) {
        //if we still have more strings, maybe print a warning?
          return method;
      }

      DebugConsole.Write($"The reflection process on entity {e?.ToString()} failed at index {i} looking for {path[i]} on {o?.ToString()}");
      return 0;
    }

    if (toSet&&lastInfo!=null) {
      try {
        if (lastInfo is PropertyInfo propInfo) {
          propInfo.SetValue(previousObj, args[0]);
        } else if (lastInfo is FieldInfo fieldInfo) {
          fieldInfo.SetValue(previousObj, args[0]);
        }
      } catch (Exception ignored) {
        return 0;
      }
      return 1;
    }
    return o;
  }
  
  public object reflectGet(List<string> path, List<int> args, bool toSet=false){
    return reflectGet(Entity,path,args, toSet: toSet);
  }
  public static void clear(Scene refill = null){
    found.Clear();
    if(refill!=null){
      foreach(FoundEntity f in refill.Tracker.GetComponents<FoundEntity>()){
        found[f.ident] = f;
      }
    }
  }
  public static object sreflectGet(List<string> path, List<int> args){
    if(!found.TryGetValue(path[1], out var f)){
      DebugConsole.Write($"Entity with attached identifier {path[1]} not found");
      return 0;
    }
    return f.reflectGet(path,args);
  }
  public static object sreflectSet(List<string> path, List<int> args){
    if(!found.TryGetValue(path[1], out var f)){
      DebugConsole.Write($"Entity with attached identifier {path[1]} not found");
      return 0;
    }
    return f.reflectGet(path,args, toSet: true);
  }
  public static object sreflectCall(List<string> path, List<int> args){
    if(!found.TryGetValue(path[1], out var f)){
      DebugConsole.Write($"Entity with attached identifier {path[1]} not found");
      return 0;
    }
    object o = f.reflectGet(path,args);
    if(o is MethodInfo m){
      try{
        return m.Invoke(f.Entity,null);
      } catch(Exception ex){
        DebugConsole.Write($"Method invocation on {path[1]} failed:\n {ex}");
      }
    }
    return null;
  }
  public static FoundEntity find(string ident){
    return found.TryGetValue(ident, out var f)?f:null;
  }
}