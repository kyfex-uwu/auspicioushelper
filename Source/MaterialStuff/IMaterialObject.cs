


using Celeste.Mod.auspicioushelper;
using Monocle;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.auspicioushelper;

public interface IMaterialObject{
  public void registerMaterials(){}
  public void renderMaterial(IMaterialLayer l, SpriteBatch sb, Camera c);
}

public class MaterialLayerInfo{
  public bool enabled;
  public bool independent;
  public bool diddraw;
  public float depth;
  public MaterialLayerInfo(bool independent, float depth){
    this.independent=independent; this.depth=depth;
  }
}
public interface IMaterialLayerSimple:IMaterialLayer{
  MaterialLayerInfo info {get;}
  bool IMaterialLayer.enabled {get=>info.enabled; set=>info.enabled=value;}
  bool IMaterialLayer.independent{get=>info.independent;}
  bool IMaterialLayer.diddraw {get=>info.diddraw; set=>info.diddraw=value;}
  float IMaterialLayer.depth {get=>info.depth;}
}