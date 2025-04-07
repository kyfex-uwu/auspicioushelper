



using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Celeste.Mod.auspicioushelper;

public static class MapHider{
  
  // public static void ADLoadHook(On.Celeste.AreaData.orig_Load orig){
  //   DebugConsole.Write("Loading maps for whatever reason");
  //   orig();
  // }

  // public static HookManager hooks = new HookManager(()=>{
  //   On.Celeste.AreaData.Load+=ADLoadHook;
  // },void ()=>{
  //   On.Celeste.AreaData.Load-=ADLoadHook;
  // }).enable();
  static Dictionary<string,ModAsset> hidden = new Dictionary<string, ModAsset>();
  static HashSet<string> whitelisted = new HashSet<string>();
  static List<Tuple<string,Regex>> rules=null;
  static bool check(string assetstr){
    foreach(var rule in rules){
      if(rule.Item2.Match(assetstr).Success){
        DebugConsole.Write($"Hiding {assetstr} due to rule {rule.Item1}");
        return true;
      }
    }
    return false;
  }
  public static void hideListed(){
    //DebugConsole.Write("here");
    if(rules == null){
      rules = new List<Tuple<string,Regex>>();
      try{
        foreach(var pair in Util.kvparseflat(auspicioushelperModule.Settings.hideRules)){
          string str = Util.stripEnclosure(pair.Value);
          DebugConsole.Write($"Registered hiding rule {pair.Key} as {str}");
          rules.Add(new Tuple<string, Regex>(pair.Key,new Regex(str)));
        }
      } catch(Exception ex){
        DebugConsole.Write("Your map hiding rules are bad "+ex.ToString());
      }
    }
    List<string> toremove = new List<string>();
    foreach(var pair in Everest.Content.Map){
      if(pair.Value.Type == typeof(AssetTypeMap)){
        if(whitelisted.Contains(pair.Key)) continue;
        if(check(pair.Key)){
          toremove.Add(pair.Key);
        } else whitelisted.Add(pair.Key);
      }
    }
    foreach(var x in toremove){
      if(Everest.Content.Map.TryGetValue(x,out var y))hidden[x]=y;
      //DebugConsole.Write($"hiding {x}");
      Everest.Content.Map.Remove(x);
    }
    if(toremove.Count != 0) AssetReloadHelper.ReloadAllMaps();
  }
  public static void revealListed(){
    foreach(var pair in hidden){
      Everest.Content.Map[pair.Key] = pair.Value;
    }
    hidden.Clear();
    AssetReloadHelper.ReloadAllMaps();
  }
  public static void uncache(){
    whitelisted.Clear();
    rules = null;
  }
}