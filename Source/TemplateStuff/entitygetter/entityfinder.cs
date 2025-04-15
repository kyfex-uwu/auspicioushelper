


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Celeste.Mod.Entities;
using Iced.Intel;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/EntityMarkingFlag")]
public class EntityMarkingFlag:Entity{
  static FoundEntity finding = null;
  static Entity last = null;
  public EntityMarkingFlag(EntityData d):base(Vector2.Zero){}
  public override void Added(Scene s){base.Added(s);RemoveSelf();}

  static Dictionary<int, List<List<int>>> flagged = new();
  public static void watch(string sig){
    try{
      List<int> looking = sig.Split("/").Select(s=>int.Parse(s.Trim())).ToList();
      if(!flagged.TryGetValue(looking[0], out var list)){
        if(looking.Count == 1) flagged.Add(looking[0], null);
        else flagged.Add(looking[0], list=new List<List<int>>());
      }
      if(looking.Count>1){
        if(list == null) flagged[looking[0]] = list = new List<List<int>>();
        looking.RemoveAt(0);
        list.Add(looking);
      }
    } catch(Exception ex){
      DebugConsole.Write($"Your ID path could not be parsed: {sig} causes error \n{ex}\n"+
      "Please remember to format your path to match: \\d+(/\\d+)*");
    }
  }

  public static void StartingLoad(EntityData d){
    DebugConsole.Write($"Start Ld {d.ID} {d.Name}");
    finding = new FoundEntity(d.ID,d);
    last = null;
  }
  public static void EndingLoad(EntityData d){
    DebugConsole.Write($"End Ld {d?.ID} {d?.Name}");
    if(finding!=null) finding.finalize(last);
    finding = null;
    last = null;
  }
  public static void LLILHook(ILContext ctx){
    var c = new ILCursor(ctx);
    if(!c.TryGotoNext(MoveType.Before, instr=>instr.MatchCall<Level>("LoadCustomEntity"))){
      goto bad;
    }
    c.EmitLdloc(17);
    c.EmitDelegate(StartingLoad);
    Type et = typeof(List<EntityData>.Enumerator);
    if(!c.TryGotoNext(MoveType.Before, instr=>instr.MatchLdloca(16),instr=>instr.MatchCall(et,"MoveNext"))){
      goto bad;
    }
    c.Index++;
    c.EmitLdloc(17);
    c.EmitDelegate(EndingLoad);
    // for(int i=-10; i<30; i++){
    //   try{
    //     if(i==0) DebugConsole.Write("===========");
    //     DebugConsole.Write(c.Instrs[c.Index+i].ToString());
    //   }catch(Exception ex){
    //     DebugConsole.Write("cannot");
    //   }
    // }
    return;
    bad:
      DebugConsole.Write("Failed to add hook to entity finder");
  }
  static ILHook llhook;
  public static void AddHook(On.Monocle.Scene.orig_Add_Entity orig, Scene self, Entity e){
    if(self is Level l){
      if(e!=null)DebugConsole.Write("Add "+ e?.ToString());
      if(finding!=null)last = e;
    }
    orig(self, e);
  } 
  public static HookManager hooks = new HookManager(()=>{
    MethodInfo oll = typeof(Level).GetMethod("orig_LoadLevel",BindingFlags.Public |BindingFlags.Instance);
    llhook = new ILHook(oll, LLILHook);
    On.Monocle.Scene.Add_Entity+=AddHook;
    DebugConsole.Write("enabling");
  },void ()=>{
    llhook?.Dispose();
    On.Monocle.Scene.Add_Entity-=AddHook;
    DebugConsole.Write("disabling");
  }, auspicioushelperModule.OnEnterMap);
}