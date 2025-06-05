
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Celeste.Editor;
using Celeste.Mod.Helpers;
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using System;

namespace Celeste.Mod.auspicioushelper;




public static class MaterialPipe {
  static List<IMaterialLayer> layers = new List<IMaterialLayer>();
  public static void ClearLayers()=>layers.Clear();
  public static bool dirty;
  public static GraphicsDevice gd;
  static bool orderFlipped;

  public static void GameplayRender(On.Celeste.GameplayRenderer.orig_Render orig, GameplayRenderer self, Scene scene){
    orderFlipped = false;
    if(transroutine!=null) transroutine.Update();
    if(GameplayRenderer.RenderDebug || Engine.Commands.Open || layers.Count==0){
      orig(self, scene); // LOL JK gotya if this ends up screwing up someone's mod you can come to my address and drop a brick on my hand
      //DebugConsole.Write("default rend");
      return;
    }
    Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
    SpriteBatch sb = Draw.SpriteBatch;
    gd = Engine.Instance.GraphicsDevice;
    RenderTarget2D t = GameplayBuffers.Gameplay;
    if(toRemove.Count>0){
      List<IMaterialLayer> nlist = new();
      foreach(var i in layers) if(!toRemove.Contains(i) && i.enabled) nlist.Add(i);
      layers = nlist;
      toRemove.Clear();
    }
    if(dirty) layers.Sort((a, b) => -a.depth.CompareTo(b.depth));
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
      scene.Entities.entities.Sort(EntityList.CompareDepth);
    }
    gd.SamplerStates[1] = SamplerState.PointClamp;
    gd.SamplerStates[2] = SamplerState.PointClamp;
    foreach(IMaterialLayer l in layers){
      if(l.usesbg() && !orderFlipped && Engine.Instance.scene is Level v){
        gd.SetRenderTarget(GameplayBuffers.Level);
        gd.Clear(v.BackgroundColor);
        v.Background.Render(v);
        orderFlipped = true;
        gd.SetRenderTarget(GameplayBuffers.Gameplay);
        bgReorderer.enable();
      }
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
    gd.SamplerStates[1]=SamplerState.LinearClamp;
    gd.SamplerStates[2]=SamplerState.LinearClamp;
    sb.End();
  }

  public static void begin(Camera c, SpriteBatch sb, RenderTarget2D t){
    gd.SetRenderTarget(t);
    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, c.Matrix);
  }
  public static void rlayer(Camera c, SpriteBatch sb, RenderTarget2D t, IMaterialLayer l){
    if(l.checkdo()){
      float alpha = transroutine == null? 1:(
        leaving.Contains(l) || entering.Contains(l)? l.transalpha(leaving.Contains(l),camAt):1
      );
      if(l.independent){
        sb.Draw(l.outtex, Vector2.Zero+c.Position,Color.White*alpha*l.alpha);
      } else {
        sb.End();
        l.render(c,sb,t);
        begin(c, sb, t);
        sb.Draw(l.outtex, Vector2.Zero+c.Position,Color.White*alpha*l.alpha);
      }
    }
  }

  static float camAt;
  static float NextTransitionDuration = 0.65f;
  static HashSet<IMaterialLayer> entering = new();
  static HashSet<IMaterialLayer> leaving = new();
  static Coroutine transroutine = null;
  static void ontrans(On.Celeste.Level.orig_TransitionTo orig, Level self, LevelData next, Vector2 dir){
    camAt = 0;
    entering.Clear();
    foreach(var l in layers) leaving.Add(l);
    NextTransitionDuration = self.NextTransitionDuration;
    orig(self, next, dir);
    transroutine = new Coroutine(transitionRoutine());
  }
  static IEnumerator transitionRoutine(){
    while(camAt<1){
      camAt = Calc.Approach(camAt, 1f, Engine.DeltaTime / NextTransitionDuration);
      yield return null;
    }
    remLeaving();
    transroutine = null;
    yield break;
  }
  public static void addLayer(IMaterialLayer l){
    if(!leaving.Remove(l)) entering.Add(l);
    toRemove.Remove(l);
    l.enabled=true;
    l.onEnable();
    if(layers.Contains(l)) return;
    dirty = true;
    layers.Add(l);
  }
  static HashSet<IMaterialLayer> toRemove = new();
  public static void removeLayer(IMaterialLayer l){
    toRemove.Add(l);
    l.enabled = false;
    l.onRemove();
    leaving.Remove(l);
    entering.Remove(l);
  }
  public static void onDie(){
    foreach(var l in layers) leaving.Add(l);
  }
  public static void remLeaving(){
    foreach(IMaterialLayer l in leaving) removeLayer(l);
    leaving.Clear();
  }
  public static Rectangle obtainInvertedRectangle(Camera c){
    Matrix m = Matrix.Invert(c.Matrix);
    Vector2 c1 = Vector2.Transform(Vector2.Zero,m);
    Vector2 c2 = Vector2.Transform(new Vector2(320,180),m);
    return new Rectangle(
      (int)c1.X, (int)c1.Y, (int)(c2.X-c1.X), (int)(c2.Y-c2.Y)
    );
  }

  static void reorderBg(ILContext ctx){
    ILCursor c = new ILCursor(ctx);
    //DebugConsole.DumpIl(c,0,50);
    if(!c.TryGotoNextBestFit(MoveType.After,
      itr=>itr.MatchLdsfld(typeof(GameplayBuffers),"Level"),
      itr => itr.MatchCall(out _),
      itr=>itr.MatchCallvirt<GraphicsDevice>("SetRenderTarget")
    ))goto bad;
    ILCursor d = c.Clone();
    if(!d.TryGotoNext(MoveType.Before,
      itr=>itr.MatchLdsfld(typeof(GameplayBuffers),"Gameplay")
    )) goto bad;
    Instruction target = d.Next;
    c.EmitDelegate(backdropReorderDetour);
    c.EmitBrtrue(target);
    bad:
    DebugConsole.Write($"Failed to add background reordering hook");
  }
  public static void setup(){
    dirty=true;
    hooks.enable();
  }
  public static void playerCtorHook(On.Celeste.Player.orig_ctor orig, Player p, Vector2 pos, PlayerSpriteMode s){
    orig(p,pos,s);
    remLeaving();
  }
  static HookManager hooks = new HookManager(()=>{
    On.Celeste.GameplayRenderer.Render += GameplayRender;
    On.Celeste.Level.TransitionTo += ontrans;
    On.Celeste.Player.ctor += playerCtorHook;
  }, void ()=>{
    On.Celeste.GameplayRenderer.Render-= GameplayRender;
    On.Celeste.Level.TransitionTo -= ontrans;
    On.Celeste.Player.ctor -= playerCtorHook;
  });

  public static bool backdropReorderDetour(){
    return orderFlipped;
  }
  static HookManager bgReorderer = new HookManager(()=>{
    IL.Celeste.Level.Render += reorderBg;
  }, void ()=>{
    IL.Celeste.Level.Render -= reorderBg;
  }, auspicioushelperModule.OnEnterMap);
}