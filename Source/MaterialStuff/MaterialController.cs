

using Monocle;
using Celeste.Mod.Entities;
using System.Collections.Generic;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/MaterialController")]
[Tracked]
internal class MaterialController:Entity{
  static Dictionary<string, IMaterialLayer> loadedMats = new Dictionary<string, IMaterialLayer>();
  public MaterialController(EntityData e,Vector2 v):base(new Vector2(0,0)){
    string path=e.Attr("path","");
    string identifier=e.Attr("identifier");
    if(string.IsNullOrWhiteSpace(identifier)) identifier = path;
    bool reload = e.Bool("reload",false);
    if(path.Length == 0)return;
    IMaterialLayer l = null;
    if(reload && loadedMats.TryGetValue(identifier, out l)){
      if(l.enabled) MaterialPipe.removeLayer(l);
      loadedMats.Remove(identifier);
    }
    DebugConsole.Write($"Loading material shader from {path} as {identifier}");
    if(!loadedMats.ContainsKey(identifier)){
      if(identifier == "auspicioushelper/ChannelMatsEN"){
        l = loadedMats[identifier] = (ChannelBaseEntity.layerA = new ChannelMaterialsA());
      } else {
        l = UserLayer.make(e);
        if(l!=null){
          loadedMats[identifier]=l;
        }
      }
    }
    if(loadedMats.TryGetValue(identifier, out var layer))MaterialPipe.addLayer(layer);
  }
  public override void Added(Scene scene){
    RemoveSelf();
  }
}