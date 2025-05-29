


using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper.Wrappers;

public class RefillW:Refill,ISimpleEnt {
  public Template parent {get;set;}
  public Template.Propagation prop=>Template.Propagation.None;
  public RefillW(EntityData d, Vector2 offset):base(d,offset){
    hooks.enable();
  }
  Vector2 toffset;
  public void setOffset(Vector2 ppos){
    toffset = Position-ppos;
  }
  public void relposTo(Vector2 loc, Vector2 ls){
    Position = toffset+loc;
  }

  bool selfCol = true;
  bool parentCol = true;
  public void parentChangeStat(int vis, int col, int act){
    if(vis!=0)Visible = vis>0;
    if(col!=0){
      parentCol = col>0;
      if(col>0)Collidable = selfCol;
      else{
        selfCol=Collidable;
        Collidable = false;
      }
    }
    if(act!=0) Active = act>0;
  }

  static void respawnHook(On.Celeste.Refill.orig_Respawn orig, Refill self){
    if(self is RefillW rw){
      rw.Collidable = false;
      orig(rw);
      rw.selfCol = true;
      rw.Collidable = rw.parentCol;
    } else orig(self);
  }
  static HookManager hooks = new HookManager(()=>{
    On.Celeste.Refill.Respawn+=respawnHook;
  }, void ()=>{
    On.Celeste.Refill.Respawn-=respawnHook;
  });
}