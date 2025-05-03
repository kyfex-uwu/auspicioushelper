


using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;

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
  public static Dictionary<string,string> kvparseflat(string str){
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

}