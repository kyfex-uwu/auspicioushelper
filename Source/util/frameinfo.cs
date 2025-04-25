
using Monocle;

namespace Celeste.Mod.auspicioushelper;

internal static class currentUpdate{
  internal static int num;
  internal static void updateHook(On.Monocle.Scene.orig_Update update, Scene self){
    num+=1; //doesn't matter if this overflows or anything <3
    update(self);
  }
  internal static HookManager hooks = new HookManager(()=>{
    On.Monocle.Scene.Update+=updateHook;
  }, ()=>{
    On.Monocle.Scene.Update-=updateHook;
  }).enable();
}