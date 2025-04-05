


using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/TemplateCassetteManager")]
public class TemplateCassetteManager:Entity{
  Dictionary<string, string> material = null;
  public TemplateCassetteManager(EntityData d, Vector2 offset):base(d.Position+offset){
    material = Util.kvparseflat(d.Attr("materials",""));
    var timing = d.Attr("timings","");
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
}