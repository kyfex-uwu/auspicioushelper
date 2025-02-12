
using System;
using Celeste;
using Monocle;


namespace Celeste.Mods.auspicioushelper;
[Tracked]
public class JumpListener:Component{
  public Action OnJump;
  public int flags;
  public JumpListener(Action jumpcallback, int flags=15):base(true,false){
    if(!setup)setupHooks();
    OnJump=jumpcallback;
  }
  public static void setupHooks(){
    On.Celeste.Player.SuperWallJump+=SuperWallHook;
    On.Celeste.Player.SuperJump+=SuperHook;
    On.Celeste.Player.WallJump+=WallHook;
    On.Celeste.Player.Jump+=JumpHook;
    setup=true;
  }
  public static void releaseHooks(){
    On.Celeste.Player.SuperWallJump-=SuperWallHook;
    On.Celeste.Player.SuperJump-=SuperHook;
    On.Celeste.Player.WallJump-=WallHook;
    On.Celeste.Player.Jump-=JumpHook;
    setup=false;
  }
  public static void alertJumpListeners(int type){
    foreach(JumpListener l in Engine.Scene.Tracker.GetComponents<JumpListener>()){
      if((l.flags & type)!=0 && l.OnJump!= null) l.OnJump();
    }
  }
  public static void JumpHook(On.Celeste.Player.orig_Jump orig, Player p, bool vfx, bool sfx){
    orig(p, vfx, sfx);
    alertJumpListeners(1);
  }
  public static void WallHook(On.Celeste.Player.orig_WallJump orig, Player p, int dir){
    orig(p,dir);
    alertJumpListeners(2);
  }
  public static void SuperHook(On.Celeste.Player.orig_SuperJump orig, Player p){
    orig(p);
    alertJumpListeners(4);
  }
  public static void SuperWallHook(On.Celeste.Player.orig_SuperWallJump orig, Player self, int dir){
    orig(self, dir);
    alertJumpListeners(8);
  }
  public static bool setup = false;
}
