


using Celeste.Mod.auspicioushelper;
using Monocle;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.auspicioushelper;

public interface IMaterialObject{
  public void registerMaterials(){}
  public void renderMaterial(IMaterialLayer l, SpriteBatch sb, Camera c);
}

