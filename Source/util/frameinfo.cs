
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public static class currentUpdate{
  public static int num;
  public static bool isSetup= false;
  public static void updateHook(On.Monocle.Scene.orig_Update update, Scene self){
    num+=1; //doesn't matter if this overflows or anything <3
    update(self);
  }
  public static void setup(){
    if(isSetup) return;
    isSetup = true;
    On.Monocle.Scene.Update+=updateHook;
  }
  public static void unsetup(){
    if(!isSetup) return;
    isSetup=false;
    On.Monocle.Scene.Update-=updateHook;
  }
}