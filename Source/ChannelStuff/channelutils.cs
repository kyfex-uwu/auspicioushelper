


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Celeste.Mod.auspicioushelper;
using Monocle;

namespace Celeste.Mods.auspicioushelper;
public static class ChannelState{
  public static Dictionary<string, int> channelStates = new Dictionary<string, int>();
  public static Dictionary<string, List<IChannelUser>> watching = new Dictionary<string, List<IChannelUser>>();
  enum Ops{
    none, not, lnot, xor, and, or, add, sub, mult, div, mrecip, mod, safemod, 
    min, max, ge, le, gt, lt, eq, ne,rshift, lshift, shiftr, shiftl
  }
  struct Modifier{
    int y;
    Ops op;
    Regex prefixSuffix = new Regex("^\\s*(-|[^-\\d]+)([-\\d]*)\\s*");
    public Modifier(string s, out bool success){
      Match m = prefixSuffix.Match(s);
      int.TryParse(m.Groups[2].ToString(),out y);
      success = true;
      switch(m.Groups[1].ToString()){
        case "~":op=Ops.not; break;
        case "!":op=Ops.lnot; break;
        case "^":op=Ops.xor; break;
        case "&":op=Ops.and; break;
        case "|":op=Ops.or; break;
        case "+":op=Ops.add; break;
        case "-":op=Ops.sub; break;
        case "*":op=Ops.mult; break;
        case "/":op=Ops.div; break;
        case "recip": case "d": case "x/":op=Ops.mrecip; break;
        case "%":op=Ops.mod; break;
        case "%s": case "r": op=Ops.safemod; break;
        case "min":op=Ops.min; break;
        case "max":op=Ops.max; break;
        case ">":op=Ops.gt; break;
        case "<":op=Ops.lt; break;
        case ">=":op=Ops.ge; break;
        case "<=":op=Ops.le; break;
        case "==":op=Ops.eq; break;
        case "!=":op=Ops.ne; break;
        case "<<":op=Ops.lshift; break;
        case ">>":op=Ops.rshift; break;
        case "x<<":op=Ops.shiftl; break;
        case "x>>":op=Ops.shiftr; break;
        default: success = false; break;
      }
      if(!success && s.Length>0){
        DebugConsole.Write($"Improper modifier {s} - parsed as op {m} and val {y}");
      }
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
        case Ops.lshift: return x<<y;
        case Ops.rshift: return x>>y;
        case Ops.shiftl: return y<<x;
        case Ops.shiftr: return y>>x;
        default: return x;
      }
    }
  }
  class ModifierDesc{
    public string outname;
    List<Modifier> ops = new List<Modifier>();
    public ModifierDesc(string nch){
      outname = nch;
      int idx=0;
      for(;idx<nch.Length;idx++) if(nch[idx]=='[')break;
      string stuff = nch.Substring(idx+1, nch.Length-idx-2);
      foreach(string sub in stuff.Split(",")){
        var m = new Modifier(sub, out var success);
        if(success)ops.Add(m);
      }
    }
    public int apply(int val){
      foreach(var op in ops) val = op.apply(val);
      return val;
    }
  }
  static Dictionary<string, List<ModifierDesc>> modifiers = new Dictionary<string, List<ModifierDesc>>();
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
      foreach(var m in ms){
        SetChannelRaw(m.outname,m.apply(state));
      }
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
    Dictionary<string, List<ModifierDesc>> nlist = new Dictionary<string, List<ModifierDesc>>();
    foreach(var pair in modifiers){
      List<ModifierDesc> keep = new List<ModifierDesc>();
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
      List<ModifierDesc> mods =null;
      if(!modifiers.TryGetValue(clean,out mods)){
        modifiers.Add(clean, mods = new List<ModifierDesc>());
      }
      ModifierDesc mod = new ModifierDesc(ch);
      mods.Add(mod);
      return channelStates[ch] = mod.apply(readChannel(clean));
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