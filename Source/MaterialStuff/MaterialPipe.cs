
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
  public static Camera camera;

  public static void GameplayRender(On.Celeste.GameplayRenderer.orig_Render orig, GameplayRenderer self, Scene scene){
    orderFlipped = false;
    if(transroutine!=null) transroutine.Update();
    if(layers.Count==0){
      orig(self, scene);
      return;
    }
    camera = self.Camera;
    gd = Engine.Instance.GraphicsDevice;
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
        if(l.checkdo()){
          l.render(self.Camera,sb);
          l.diddraw = true;
        }
        else l.diddraw = false;
      }
    }
    orig(self, scene);
    gd.SamplerStates[1]=SamplerState.LinearClamp;
    gd.SamplerStates[2]=SamplerState.LinearClamp;
    sb.End();
  }

  public static void continueDefault(){
    gd.SetRenderTarget(GameplayBuffers.Gameplay);
    Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, camera.Matrix);
  }
  public static void rlayer(Camera c, SpriteBatch sb, RenderTarget2D t, IMaterialLayer l){
    if(l.checkdo()){
      float alpha = 
      if(l.independent){
        sb.Draw(l.outtex, Vector2.Zero+c.Position,Color.White*alpha*l.alpha);
      } else {
        
      }
    }
  }
  public static float GetTransitionAlpha(IMaterialLayer l){
    return transroutine == null? 1:(
      leaving.Contains(l) || entering.Contains(l)? l.transalpha(leaving.Contains(l),camAt):1
    );
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