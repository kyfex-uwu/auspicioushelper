


using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Editor;
using Celeste.Mod.Entities;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/TemplateCassetteManager")]
public class TemplateCassetteManager:Entity{
  Dictionary<string, string> material = null;
  static Dictionary<string,string> nameTrans = new Dictionary<string, string>{
    {"small","event:/game/general/cassette_block_switch_1"},
    {"big","event:/game/general/cassette_block_switch_2"},
  };
  class timingDesc{
    public List<Tuple<string,int>> ch = new List<Tuple<string, int>>();
    public List<string> sound = new List<string>();
    public void fromDict(Dictionary<string,string> d){
      foreach(var pair in d){
        switch(pair.Key){
          case "channel": case "ch": case "c": case "channels":
            foreach(var chpair in Util.kvparseflat(Util.stripEnclosure(pair.Value))){
              ch.Add(new Tuple<string, int>(chpair.Key,int.Parse(chpair.Value)));
            } 
            break;
          case "sound": case "s":
            if(nameTrans.TryGetValue(pair.Value, out var sname)){
              sound.Add(sname);
            } else sound.Add(pair.Value); 
            break;
        }
      }
    }
  }
  List<Tuple<float,timingDesc>> timings = new List<Tuple<float, timingDesc>>();
  float beatsPerMeasure;
  float beatsPerSecond;
  float lastBeatLoc = 0;
  int curIndex = 0;
  public TemplateCassetteManager(EntityData d, Vector2 offset):base(d.Position+offset){
    material = Util.kvparseflat(d.Attr("materials",""));
    var timingDict = Util.kvparseflat(d.Attr("timings",""));
    beatsPerMeasure = d.Float("beatsPerMeasure",4);
    beatsPerSecond = d.Float("beatsPerMinute",90)/60;
    lastBeatLoc = (d.Float("offset",0)*beatsPerSecond)%beatsPerMeasure;
    Dictionary<float, timingDesc> added = new Dictionary<float, timingDesc>();
    if(timingDict != null && timingDict.Count>0)foreach(var pair in timingDict){
      float timing = float.Parse(pair.Key);
      if(timing>beatsPerMeasure) continue;
      timingDesc desc = null;
      if(!added.TryGetValue(timing, out desc)){
        desc = new timingDesc();
        timings.Add(new Tuple<float, timingDesc>(timing,desc));
        added.Add(timing,desc);
      }
      desc.fromDict(Util.kvparseflat(Util.stripEnclosure(pair.Value)));
    }
    timings.Sort((a, b) => a.Item1.CompareTo(b.Item1));
    for(int i=0; i<timings.Count; i++){
      if(timings[i].Item1>lastBeatLoc){
        curIndex = i; break;
      }
    }
    inimaterials();
  }
  public void inimaterials(){
    if(material != null) foreach(var pair in material){
      var format = CassetteMaterialLayer.CassetteMaterialFormat.fromDict(Util.kvparseflat(Util.stripEnclosure(pair.Value)));
      CassetteMaterialLayer other;
      if(CassetteMaterialLayer.layers.TryGetValue(pair.Key, out other)){
        if(other.GetHashCode() != format.gethash()){
          MaterialPipe.removeLayer(other);
          other = null;
        }
      }
      if(other == null) other = new CassetteMaterialLayer(format, pair.Key);
      CassetteMaterialLayer.layers[pair.Key]=other;
      MaterialPipe.addLayer(other);
    }
  }
  public static void unfrickMats(Scene s){
    List<CassetteMaterialLayer> toremove = new List<CassetteMaterialLayer>();
    foreach(var l in MaterialPipe.layers){
      if(l is CassetteMaterialLayer la)toremove.Add(la);
    }
    foreach(var la in toremove){
      MaterialPipe.removeLayer(la);
    }
    foreach(Entity e in Engine.Instance.scene.Entities){
      if(e is TemplateCassetteManager m) m.inimaterials();
    }
    foreach(Entity e in Engine.Instance.scene.Entities){
      if(e is TemplateCassetteBlock c){
        if(CassetteMaterialLayer.layers.TryGetValue(c.channel,out var layer)){
          var l = new List<Entity>();
          c.AddAllChildren(l);
          layer.dump(l);
        }
      }
    }
  }
  public override void Update(){
    base.Update();

    float cbeatloc = lastBeatLoc+Engine.DeltaTime*beatsPerSecond;
    if(timings.Count>0)while(timings[curIndex].Item1<=cbeatloc){
      var t = timings[curIndex].Item2;
      foreach(string sound in t.sound){
        Audio.Play(sound);
      }
      foreach(var pair in t.ch){
        ChannelState.SetChannel(pair.Item1,pair.Item2);
      }
      curIndex++;
      if(curIndex == timings.Count){
        curIndex = 0;
        cbeatloc -= beatsPerMeasure;
      }
    }
    lastBeatLoc = cbeatloc;
  }
}