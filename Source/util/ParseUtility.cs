


using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Celeste.Editor;
using Monocle;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.auspicioushelper;
public static class Util{

  public static Color hexToColor(string hex){
    hex = hex.TrimStart('#');
    uint rgba = uint.Parse(hex, NumberStyles.HexNumber);
    int shift = hex.Length>4?8:4;
    uint mask = hex.Length>4?0xffu:0xfu;
    float mult = hex.Length>4?1f/255f:1f/15f;
    if(hex.Length %4 != 0){
      rgba= (rgba<<shift)+mask;
    }
    return new Color(
      (float)((rgba>>(shift*3))&mask)*mult, 
      (float)((rgba>>(shift*2))&mask)*mult, 
      (float)((rgba>>shift)&mask)*mult, 
      (float)(rgba&mask)*mult);
  }

  public static int bsearchLast(float[] arr, float val){
    int left = 0; 
    int right = arr.Length;
    while(right-left>1){
      int middle = (left+right)/2;
      if(arr[middle]>val){
        right = middle;
      } else {
        left = middle;
      }
    }
    return left;
  }
  public static int bsearchFirst(float[] arr, float val){
    int left = -1; 
    int right = arr.Length-1;
    while(right-left>1){
      int middle = (left+right+1)/2;
      if(arr[middle]>=val){
        right = middle;
      } else {
        left = middle;
      }
    }
    return right;
  }
  public static float remap(float t, float low, float high){
    t=t-low;
    high = high-low;
    return t/high;
  }
  static Dictionary<char,char> escape = new Dictionary<char, char>{
    {'{','}'}, {'[',']'}, {'(',')'},
  };
  public static Dictionary<string,string> kvparseflat(string str, bool strip=false){
    if(strip) str=stripEnclosure(str);
    Stack<char> unescaped = new Stack<char>();
    var o = new Dictionary<string,string>();
    string k="";
    string v="";
    int idx=0;
    bool escapeNext = false;
    parsekey:
      if(idx>=str.Length) return o;
      if(str[idx] == ':'){
        idx++;goto parsevalue;
      }
      else{
        k+=str[idx]; idx++; goto parsekey;
      }

    parsevalue:
      if((idx >= str.Length||str[idx] == ',') && unescaped.Count ==0){
        idx++; goto fent;
      }
      if(idx >= str.Length){
        DebugConsole.Write("PARSE ERROR: "+str);
        return null;
      }
      if(escape.TryGetValue(str[idx], out var esc)){
        unescaped.Push(esc);
        v+=str[idx]; idx++; goto parsevalue;
      }
      if(unescaped.Count>0 && unescaped.Peek()==str[idx]){
        unescaped.Pop(); 
        v+=str[idx]; idx++; goto parsevalue;
      }
      if(str[idx]=='"'){
        v+=str[idx]; idx++; goto parsestring;
      }
      v+=str[idx]; idx++; goto parsevalue;

    parsestring:
      if(idx == str.Length){
        DebugConsole.Write("PARSE ERROR: "+str);
      }
      if(escapeNext){
        escapeNext = false; 
        v+=str[idx]; idx++; goto parsestring;
      }
      if(str[idx] == '"'){
        v+=str[idx]; idx++; goto parsevalue;
      }
      v+=str[idx]; idx++; goto parsestring;

    fent:
      o.Add(k.Trim(),v.Trim());
      k=""; v="";
      goto parsekey;
  }
  public static List<string> listparseflat(string str,bool strip=false,bool stripout=false){
    if(strip) str=stripEnclosure(str);
    Stack<char> unescaped = new Stack<char>();
    var o = new List<string>();
    string v="";
    int idx=0;
    bool escapeNext = false;
    parsevalue:
      if((idx >= str.Length||str[idx] == ',') && unescaped.Count ==0){
        idx++; goto fent;
      }
      if(idx >= str.Length){
        DebugConsole.Write("PARSE ERROR: "+str);
        return null;
      }
      if(escape.TryGetValue(str[idx], out var esc)){
        unescaped.Push(esc);
        v+=str[idx]; idx++; goto parsevalue;
      }
      if(unescaped.Count>0 && unescaped.Peek()==str[idx]){
        unescaped.Pop(); 
        v+=str[idx]; idx++; goto parsevalue;
      }
      if(str[idx]=='"'){
        v+=str[idx]; idx++; goto parsestring;
      }
      v+=str[idx]; idx++; goto parsevalue;

    parsestring:
      if(idx == str.Length){
        DebugConsole.Write("PARSE ERROR: "+str);
      }
      if(escapeNext){
        escapeNext = false; 
        v+=str[idx]; idx++; goto parsestring;
      }
      if(str[idx] == '"'){
        v+=str[idx]; idx++; goto parsevalue;
      }
      v+=str[idx]; idx++; goto parsestring;

    fent:
      if(stripout) o.Add(stripEnclosure(v.Trim()));
      else o.Add(v.Trim());
      v="";
      goto parsevalue;
  }
  public static string stripEnclosure(string str){
    if(str == "") return "";
    if(str[0] == '\"' && str[str.Length-1] == '\"') return str.Substring(1,str.Length-2);
    if(escape.TryGetValue(str[0],out var esc)){
      if(str[str.Length-1]==esc)return str.Substring(1,str.Length-2);
      else {
        DebugConsole.Write("Enclosing characters not symmetric: "+str);
        return str;
      }
    }
    return str;
  }
  public static float[] csparseflat(string str){
    return str.Split(",").Select(s=>{
      float.TryParse(s, out var l);
      return l;
    }).ToArray();
  }
  public static float[] toArray(Vector2 x)=>new float[]{x.X,x.Y};
  public static float[] toArray(Vector3 x)=>new float[]{x.X,x.Y,x.Z};
  public static float[] toArray(Vector4 x)=>new float[]{x.X,x.Y,x.Z,x.W};

  public static string sideBySide(List<string> strs, string seperator = " "){
    List<string[]> sp = strs.Select(s=>s.Split('\n')).ToList();
    List<int> widths = sp.Select(l=>l.Max(s=>s.Length)).ToList();
    int lines = sp.Max(l=>l.Length);
    string res = "";
    for(int i=0; i<lines; i++){
      for(int j=0; j<sp.Count; j++){
        if(i<sp[j].Length){
          res+=sp[j][i]+new string(' ', widths[j]-sp[j][i].Length)+seperator;
        } else {
          res+=new string(' ',widths[j])+seperator;
        }
      }
      res+= '\n';
    }
    return res;
  }
  public static FloatRect levelBounds(Scene s){
    if(s is Level l){
      return new FloatRect(l.Bounds.Left,l.Bounds.Top,l.Bounds.Right-l.Bounds.Left,l.Bounds.Bottom-l.Bounds.Top);
    }
    return FloatRect.empty;
  }
  public static void RemovePred<T>(HashSet<T> set, Func<T,bool> pred){
    List<T> tr = new();
    foreach(T a in set){
      if(pred(a)) tr.Add(a);
    }
    foreach(T a in tr) set.Remove(a);
  }

  public class FunctionList<T1>{
    List<Func<T1,bool>> funcs = new();
    public enum InvocationMode{
      And, AndShortcircut, Or, OrShortcircuit
    }
    InvocationMode mode;
    public FunctionList(InvocationMode mode = InvocationMode.OrShortcircuit){
      this.mode = mode;
    }
    public void Add(Func<T1,bool> n)=>funcs.Add(n);
    public void Remove(Func<T1,bool> n)=>funcs.Remove(n);
    public bool AndShortcircut(T1 p){
      foreach(var f in funcs) if(!f(p)) return false;
      return true;
    }
    public bool And(T1 p){
      bool res = true;
      foreach(var f in funcs) res=res&&f(p);
      return res;
    }
    public bool OrShortcircuit(T1 p){
      foreach(var f in funcs) if(f(p)) return true;
      return false;
    }
    public bool Or(T1 p){
      bool res = false;
      foreach(var f in funcs) res = res||f(p);
      return res; 
    }
    public bool Invoke(T1 p){
      switch(mode){
        case InvocationMode.And: return And(p);
        case InvocationMode.AndShortcircut: return AndShortcircut(p);
        case InvocationMode.Or: return Or(p);
        default: return OrShortcircuit(p);
      }
    }
    public static implicit operator Func<T1, bool>(FunctionList<T1> fl) => fl.Invoke;
  }

  public static float SineInOut(float val)=>-MathF.Cos(MathF.PI*val)/2+0.5f;
  public static float SineInOut(float val, out float derivative){
    derivative = MathF.PI*MathF.Sin(MathF.PI*val)/2;
    return SineInOut(val);
  }
  public static float Approach(float val, float target, float amount, out int sign){
    if(val == target){
      sign = 0; return val;
    }
    if(val>target){
      sign = -MathF.Sign(amount);
      return MathF.Max(target, val-amount);
    }
    sign = MathF.Sign(amount);
    return MathF.Min(target, val+amount);
  }
  public static float Spike(float val)=>MathF.Max(0,1-Math.Abs(2*val-1));
  public static float Spike(float val, out float derivative){
    derivative = val>0.5f?-2:2;
    return Spike(val);
  }
  public static float QuadIn(float val)=>val*val;
  public static float QuadIn(float val, out float derivative){
    derivative = 2*val;
    return QuadIn(val);
  }
}