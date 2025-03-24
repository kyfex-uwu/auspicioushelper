



using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class CassetteGateSolid:Solid, IMaterialObject{


  public CassetteGateSolid(Vector2 p, float w, float h):base(p,w,h,true){
    Depth = -9000;
    SurfaceSoundIndex = 32;
  }
  public override void Render(){
    base.Render();
    Draw.Rect(this.Collider,Color.White);
  }
  public void registerMaterials(){
    CassetteGate.visuals.planDraw(this);
  }
  public void renderMaterial(MaterialLayer l, SpriteBatch sb, Camera c){
    sb.Draw(Draw.Pixel.Texture.Texture_Safe, new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height), Draw.Pixel.ClipRect, new Color(1,0,0,255));
  }
}