
using System;
using Monocle;

namespace Celeste.Mod.auspicioushelper;


[Tracked]
public class UpdateHook:Component{
  public Action beforeAction=null;
  public Action afterAction=null;
  public bool updatedThisFrame = false;
  public static float TimeSinceTransMs=0;
  public UpdateHook(Action before=null, Action after=null):base(true,false){
    beforeAction=before;afterAction =after;
    hooks.enable();
  }
  public override void Update() {
    base.Update();
    updatedThisFrame = true;
  }
  internal static int framenum;
  internal static void updateHook(On.Celeste.Level.orig_Update update, Level self){
    foreach(UpdateHook u in self.Tracker.GetComponents<UpdateHook>()){
      u.updatedThisFrame = false;
      if(u.beforeAction!=null)u.beforeAction();
    }
    framenum+=1; //doesn't matter if this overflows or anything <3
    update(self);
    if(TimeSinceTransMs<1000000)TimeSinceTransMs+=Engine.DeltaTime*1000;
    foreach(UpdateHook u in self.Tracker.GetComponents<UpdateHook>()){
      if(u.afterAction!=null)u.afterAction();
    }
  }
  internal static HookManager hooks = new HookManager(()=>{
    On.Celeste.Level.Update+=updateHook;
  }, ()=>{
    On.Celeste.Level.Update-=updateHook;
  }).enable();
}