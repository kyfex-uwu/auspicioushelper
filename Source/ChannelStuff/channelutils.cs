


using System;
using System.Collections.Generic;
using Celeste.Editor;
using Celeste.Mod.auspicioushelper;
using FMOD;
using Monocle;

namespace Celeste.Mods.auspicioushelper;
public static class ChannelState{
  public static Dictionary<string, int> channelStates = new Dictionary<string, int>();
  public static Dictionary<string, List<IChannelUser>> watching = new Dictionary<string, List<IChannelUser>>();
  enum Ops{
    none, not, lnot, xor, and, or, add, sub, mult, div, mrecip, mod, safemod, min, max, ge, le, gt, lt, eq, ne
  }
  struct modifier{
    int y;
    Ops op;
    public modifier(string s){

    }
    public int apply(int x){
      switch(op){
        case Ops.not: return ~x;
        case Ops.lnot: return x==0?1:0;
        case Ops.xor: return x^y;
        case Ops.and: return x&y;
        case Ops.or: return x|y;
        case Ops.add: return x+y;
        case Ops.sub: return x-y;
        case Ops.mult: return x*y;
        case Ops.div: return x/y;
        case Ops.mrecip: return y/x;
        case Ops.mod: return x%y;
        case Ops.safemod: return ((x%y)+y)%y;
        case Ops.min: return Math.Min(x,y);
        case Ops.max: return Math.Max(x,y);
        case Ops.ge: return x>=y?1:0;
        case Ops.le: return x<=y?1:0;
        case Ops.gt: return x>y?1:0;
        case Ops.lt: return x<y?1:0;
        case Ops.eq: return x==y?1:0;
        case Ops.ne: return x!=y?1:0;
        default: return x;
      }
    }
  }
  struct modifierDesc{
    public string outname;
    List<modifier> ops = new List<modifier>();
    public int apply(int val){}
    public modifierDesc(string nch){
      outname = nch;
      int idx=0;
      for(;idx<nch.Length;idx++) if(nch[idx]=='[')break;
      string stuff = nch.Substring(idx+1; nch.Length-idx-2);
      foreach(string sub in stuff.Split(",")){
        var m = new modifier(sub, out var success);
        if(success)ops.Add(m);
        else if(sub!="")DebugConsole.Write($"Improper modifier operation {sub}");
      }
    }
  }
  static Dictionary<string, List<modifierDesc>> modifiers = new Dictionary<string, List<modifierDesc>>();
  public static int readChannel(string ch){
    if(channelStates.TryGetValue(ch, out var v)) return v;
    else return addModifier(ch);
  }
  static void SetChannelRaw(string ch, int state){
    if(readChannel(ch) == state) return;
    channelStates[ch] = state;
    if (watching.TryGetValue(ch, out var list)) {
      foreach(IChannelUser b in list){
        b.setChVal(state);
      }
    }
  }
  public static void SetChannel(string ch, int state){
    int idx=0;
    for(;idx<ch.Length;idx++) if(ch[idx]=='[')break;
    string clean = ch.Substring(0,idx);
    SetChannelRaw(clean,state);
    if(modifiers.TryGetValue(clean, out var ms)){
      foreach(var m in ms) SetChannelRaw(m.outname,m.apply(state));
    }
  }
  public static void unwatchNow(IChannelUser b){
    if (watching.TryGetValue(b.channel, out var list)) {
      list.Remove(b);
    }
  }
  public static int watch(IChannelUser b){
    //DebugConsole.Write("watching new thing");
    if (!watching.TryGetValue(b.channel, out var list)) {
      list = new List<IChannelUser>();
      watching[b.channel] = list;
    }
    list.Add(b);
    return readChannel(b.channel);
  }
  static void clearModifiers(HashSet<string> except = null){
    Dictionary<string, List<modifierDesc>> nlist = new Dictionary<string, List<modifierDesc>>();
    foreach(var pair in modifiers){
      List<modifierDesc> keep = new List<modifierDesc>();
      foreach(var mod in pair.Value){
        if(except == null || !except.Contains(mod.outname))channelStates.Remove(mod.outname);
        else keep.Add(mod);
      }
      if(keep.Count>0) nlist[pair.Key] = keep;
    }
    modifiers = nlist;
  }
  public static void unwatchAll(){
    watching.Clear();
    clearModifiers();
  }
  static int addModifier(string ch){
    int idx=0;
    for(;idx<ch.Length;idx++) if(ch[idx]=='[')break;
    string clean = ch.Substring(0,idx);

    if(clean!=ch && ch[ch.Length-1] == ']'){
      List<modifierDesc> mods =null;
      if(!modifiers.TryGetValue(clean,out mods)){
        modifiers.Add(clean, mods = new List<modifierDesc>());
      }
      modifierDesc mod = new modifierDesc(ch);
      mods.Add(mod);
      return mod.apply(readChannel(clean));
    } else {
      channelStates.Add(ch,0);
      return 0;
    }
  }
  public static void unwatchTemporary(){
    clearModifiers();
    List<string> toRemove = new List<string>();
    foreach(var pair in watching){
      var newlist = new List<IChannelUser>();
      foreach(IChannelUser e in pair.Value){
        if(e is Entity en && en.TagCheck(Tags.Persistent)){
          newlist.Add(e);
        }
      }
      if(newlist.Count>0){
        watching[pair.Key] = newlist;
        addModifier(pair.Key);
      }
      else toRemove.Add(pair.Key);
    }
    foreach(var ch in toRemove) watching.Remove(ch);
  }
}