
using System.Collections.Generic;
using System.Linq;
using Celeleste.Mods.auspicioushelper;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public static class MaterialPipe {
  public static List<MaterialLayer> layers = new List<MaterialLayer>();
  public static bool dirty;
  public static GraphicsDevice gd;
  public static void setup(){
    dirty=true;
  }
  public static void registerPipe(){
    On.Celeste.GameplayRenderer.Render += GameplayRender;
  }
  public static void deregisterPipe(){
    On.Celeste.GameplayRenderer.Render-= GameplayRender;
  }

  public static void GameplayRender(On.Celeste.GameplayRenderer.orig_Render orig, GameplayRenderer self, Scene scene){
    orig(self, scene); // Lmao this (ok friend we are calling the original function, don't panic don't panic don't panic)
    Engine.Instance.GraphicsDevice.Clear(Color.Transparent);

    SpriteBatch sb = Draw.SpriteBatch;
    gd = Engine.Instance.GraphicsDevice;
    RenderTarget2D t = GameplayBuffers.Gameplay;
    if(dirty) layers.Sort((a, b) => a.depth.CompareTo(b.depth));
    dirty=false;

    double curdepth = float.PositiveInfinity;
    bool sorted = true;
    foreach(Entity e in scene.Entities.entities){
      if(e.Visible && (e is IMaterialObject en)){
        en.registerMaterials();
      }
      if(curdepth<e.actualDepth) sorted=false;
      curdepth=e.actualDepth;
    }
    //strawberries change their depth for the sole reason of making my life harder...;
    if(!sorted){
      //DebugConsole.Write("ENTITYLIST NOT SORTED (resorting, kms)");
      //oh god whyyy are they not using a tree for this like this is literally the most tree-ish 
      //problem ever. My head hurts my head hurts my head hurts my head hurts my head hurts
      //yes let's just resort the entire list every time we change a single depth or add any entities
      //truely a handsome and stylish methodology
      scene.Entities.entities.Sort(EntityList.CompareDepth);
    }
    gd.SamplerStates[1] = SamplerState.PointClamp;
    gd.SamplerStates[2] = SamplerState.PointClamp;
    foreach(MaterialLayer l in layers){
      if(l.independent){
        if(l.checkdo())l.render(self.Camera,sb);
        else l.diddraw = false;
      }
    }
    
    float ndepth = layers.Count>0?layers[0].depth:float.NegativeInfinity;
    int ldx = 0;

    begin(self.Camera, sb, t);
    foreach(Entity e in scene.Entities.entities){
      if(e.Visible && !e.TagCheck(Tags.HUD) && e.actualDepth>=ndepth){
        e.Render();
      } else {
        while(ndepth>e.actualDepth){
          rlayer(self.Camera, sb, t, layers[ldx]);
          ldx++;
          ndepth = layers.Count>ldx?layers[ldx].depth:float.NegativeInfinity;
        }
        if(e.Visible && !e.TagCheck(Tags.HUD))e.Render();
      }
    }
    while(ldx<layers.Count){
      rlayer(self.Camera, sb, t, layers[ldx++]);
    }
    //sb.Draw(Draw.Pixel.Texture.Texture_Safe, new Rectangle((int)self.Camera.Position.X,(int)self.Camera.Position.Y,10,10),Draw.Pixel.ClipRect,Color.White);
    sb.End();
  }

  public static void begin(Camera c, SpriteBatch sb, RenderTarget2D t){
    gd.SetRenderTarget(t);
    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, c.Matrix);
  }
  public static void rlayer(Camera c, SpriteBatch sb, RenderTarget2D t, MaterialLayer l){
    if(l.checkdo()){
      if(l.independent){
        //DebugConsole.Write("pasting prerendered layer");
        sb.Draw(l.outtex, Vector2.Zero+c.Position,Color.White);
      } else {
        sb.End();
        DebugConsole.Write("Generating pastable layer");
        l.render(c,sb,t);
        begin(c, sb, t);
        sb.Draw(l.outtex, Vector2.Zero,Color.White);
      }
    }
  }
  public static void addLayer(MaterialLayer l){
    dirty = true;
    if(layers.Contains(l)) return; //we do not allow that, no sir
    layers.Add(l);
  }
  public static void removeLayer(MaterialLayer l){
    layers.Remove(l);
  }
  public static Rectangle obtainInvertedRectangle(Camera c){
    Matrix m = Matrix.Invert(c.Matrix);
    Vector2 c1 = Vector2.Transform(Vector2.Zero,m);
    Vector2 c2 = Vector2.Transform(new Vector2(320,180),m);
    return new Rectangle(
      (int)c1.X, (int)c1.Y, (int)(c2.X-c1.X), (int)(c2.Y-c2.Y)
    );
  }
}