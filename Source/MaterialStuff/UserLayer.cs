


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

internal class UserLayer:BasicMaterialLayer, IMaterialLayer, IFadingLayer, ISettableDepth{
  void setparamvalex(string key, bool t) {
    normalShader.Parameters[key]?.SetValue(t);
    quietShader?.Parameters[key]?.SetValue(t);
  }
  void setparamvalex(string key, float t) {
    normalShader.Parameters[key]?.SetValue(t);
    quietShader?.Parameters[key]?.SetValue(t);
  }
  void setparamvalex(string key, int t) {
    normalShader.Parameters[key]?.SetValue(t);
    quietShader?.Parameters[key]?.SetValue(t);
  }
  void setparamvalex(string key, float[] t){
    normalShader.Parameters[key]?.SetValue(t);
    quietShader?.Parameters[key]?.SetValue(t);
  }
  List<Action> chset = new();
  Action getparamsetter(string key, string channel, float mult){
    return ()=>{
      float val = mult*(float)ChannelState.readChannel(channel);
      DebugConsole.Write($"{key} {val}");
      if(Calc.NextFloat(Calc.Random)<0.1f)setparamvalex(key,val+0.0001f);

      else setparamvalex(key,val);

    };
  }
  static Regex chr = new Regex(@"@(\w*(?:\[[^]]+\])?)((?:[*\/][\d\.]+)?)", RegexOptions.Compiled);
  void setparamval(string key, string val){
    if(string.IsNullOrEmpty(key)) return;
    switch(val.ToLower()){
      case "true": setparamvalex(key, true); break;
      case "false": setparamvalex(key, false); break;
      default: switch(val[0]){
        case '#': setparamvalex(key, Util.toArray(Util.hexToColor(val).ToVector4())); break;
        case '{': case '[': setparamvalex(key, Util.csparseflat(Util.stripEnclosure(val))); break;
        case '@': 
          var match = chr.Match(val);
          if (!match.Success){
            DebugConsole.Write($"Error parsing channel string {val}"); break;
          }
          string ch = match.Groups[1].Value;
          float mult = 1;
          if(!string.IsNullOrWhiteSpace(match.Groups[2].Value)){
            float.TryParse(match.Groups[2].Value.Substring(1), out mult);
            if(match.Groups[2].Value[0] == '/') mult = 1/mult;
          }
          chset.Add(getparamsetter(key, ch, mult));
          break;
        case >='0' and <='9': case '.':
          if(val.Contains('.')){
            float.TryParse(val, out var f);
            setparamvalex(val,f);
          } else {
            int.TryParse(val, out var i);
            setparamvalex(val,i);
          }
          break;
        default:
          DebugConsole.Write($"Don't know how to parse {val}"); break;
      }break;
    }
  }
  internal static UserLayer make(EntityData d){
    var eff = auspicioushelperGFX.LoadExternEffect(d.Attr("path"));
    if(eff != null){
      return new UserLayer(d,eff);
    } else {
      return null;
    }
  }
  bool uselast = false;
  RenderTarget2D prev = null;
  bool _usebg = false;
  public override bool usesbg(){
    return _usebg;
  }
  public UserLayer(EntityData d, Tuple<Effect,Effect> effects):base(
    d.Int("depth",0),effects.Item1,d.Bool("independent"),d.Bool("simple"),d.Bool("always") 
  ){
    quietShader = effects.Item2;
    if(uselast = d.Bool("useprev",false)){
      prev = new RenderTarget2D(Engine.Instance.GraphicsDevice, 320, 180);
    }
    _usebg = d.Bool("usebg");
    try{
      foreach(var pair in Util.kvparseflat(d.Attr("params",""))){
        setparamval(pair.Key,pair.Value);
      }
    } catch(Exception err){
      DebugConsole.Write($"error setting shader params: {err}");
    }
  }
  public override void render(Camera c, SpriteBatch sb, RenderTarget2D back){
    SamplerState origss = null;
    if(uselast){
      origss = MaterialPipe.gd.SamplerStates[4];
      prev = swapOuttex(prev);
      MaterialPipe.gd.Textures[4]=prev;
      MaterialPipe.gd.SamplerStates[4]=SamplerState.PointClamp;
    }
    foreach(Action s in chset) s();
    base.render(c, sb, back);
    if(uselast){
      MaterialPipe.gd.SamplerStates[4]=origss;
    }
  }

  

  public IFadingLayer.FadeTypes fadeTypeIn {get;set;} = IFadingLayer.FadeTypes.Linear;
  public IFadingLayer.FadeTypes fadeTypeOut {get;set;} = IFadingLayer.FadeTypes.Linear;
}