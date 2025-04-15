


using System;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public static class DemoButtonUsurper{
  public static bool usurp = false;
  private static void pressedDetour(Func<VirtualInput,bool> orig, VirtualInput self){
    
  }
  public static void loadHooks(){
    //On.Celeste.VirtualIn
  }
}