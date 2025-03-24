


using System.Collections.Generic;
using System.Linq;
using Celeste;
using Celeste.Mod;
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.Utils;

namespace Celeste.Mods.auspicioushelper;

public static class EntityBinder{
  public static Dictionary<Entity, EntityData> entitydata;
  public static Dictionary<int, Entity> entities;

  public static int? last=null;

  public class EntityBinderRef: Component{
    Entity e;
    EntityData d;
    public EntityBinderRef(EntityData d, Entity e):base(false, false){
      this.e=e;
      this.d=d;
    }
  }
  public static void addHooks(){
    //IL.Celeste.Level.LoadLevel += modifyLoad;
    //Everest.Events.Level.OnLoadEntity += OnLoadEntityHook;
    //On.Monocle.Scene.Add_Entity += OnAddEntityHook;
    //IL.Celeste.Level.orig_load
  }
  public static void modifyLoad(ILContext il){
    ILCursor c = new ILCursor(il);
    foreach (var instruction in c.Instrs) {
      DebugConsole.Write($"Instruction: {instruction.OpCode}, Operand: {instruction.Operand}");
    }

    if(c.TryGotoNext(
      MoveType.After,
      i=>i.MatchStloc(17)
    )){
      c.Emit(OpCodes.Ldloca_S, 17);
      c.EmitDelegate(register);
      DebugConsole.Write("Setup entity binder");
    } else {
      DebugConsole.Write("Failed to setup binder");
    }
  }
  public static void register(EntityData e){
    DebugConsole.Write(e.Name+" "+e.ID.ToString());
  }
  public static bool OnLoadEntityHook(Level l, LevelData ld, Vector2 offset, EntityData dat){
    DebugConsole.Write("here "+dat.Name);
    last = dat.ID;
    return false;
  }
  public static void OnAddEntityHook(On.Monocle.Scene.orig_Add_Entity orig, Scene self, Entity e){
    orig(self,e);
    if(last != null && last is int l){
      DynamicData.For(e).Set("__ID_",last);
    }
  }
}