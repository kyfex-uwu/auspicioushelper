



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
  public static bool isHiding = false;
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
    if(auspicioushelperModule.Settings.hideRulesList == null) return;
    if(rules == null){
      rules = new List<Tuple<string,Regex>>();
      int i=-1;
      foreach(var rule in auspicioushelperModule.Settings.hideRulesList){
        i++;
        if(rule == "") continue;
        try{
          rules.Add(new Tuple<string, Regex>(i.ToString(),new Regex(rule, RegexOptions.IgnoreCase)));
          DebugConsole.Write($"Registered hiding rule {i.ToString()} as {rule}");
        }catch(Exception ex){
          DebugConsole.Write($"your rule {i} was bad - error message {ex}");
        }
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
    if (toremove.Count != 0) {
      AssetReloadHelper.ReloadAllMaps();
      isHiding = true;
    }
  }
  public static void revealListed(){
    if(!isHiding || hidden == null || hidden.Count==0)return;
    foreach(var pair in hidden){
      Everest.Content.Map[pair.Key] = pair.Value;
    }
    hidden.Clear();
    isHiding = false;
    AssetReloadHelper.ReloadAllMaps();
  }
  public static void uncache(){
    whitelisted.Clear();
    rules = null;
  }
}