
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public static class currentUpdate{
  public static int num;
  public static void updateHook(On.Monocle.Scene.orig_Update update, Scene self){
    num+=1; //doesn't matter if this overflows or anything <3
    update(self);
  }
  public static HookManager hooks = new HookManager(()=>{
    On.Monocle.Scene.Update+=updateHook;
  }, ()=>{
    On.Monocle.Scene.Update-=updateHook;
  }).enable();
}