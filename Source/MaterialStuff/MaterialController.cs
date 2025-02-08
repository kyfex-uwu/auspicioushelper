

using Monocle;
using Celeste.Mod.Entities;
using System.Collections.Generic;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/MaterialController")]
[Tracked]
public class MaterialController:Entity{
  static Dictionary<string, MaterialLayer> loadedMats = new Dictionary<string, MaterialLayer>();
  public MaterialController(EntityData e,Vector2 v):base(new Vector2(0,0)){
    string path=e.Attr("path","");
    bool reload = e.Bool("reload",false);
    if(path.Length == 0)return;
    if(reload && loadedMats.TryGetValue(path, out var l)){
      if(l.enabled) MaterialPipe.removeLayer(l);
      loadedMats.Remove(path);
    }
    if(!loadedMats.ContainsKey(path)){
      if(path == "auspicioushelper/ChannelMatsEN"){
        loadedMats[path]= (ChannelBaseEntity.layerA = new ChannelMaterialsA());
      } else {
        return;
      }
    }
    MaterialPipe.addLayer(loadedMats[path]);
  }
  public override void Added(Scene scene){
    RemoveSelf();
  }
}