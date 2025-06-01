


using System.IO;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper.Wrappers;

public class JumpThruW:JumpthruPlatform{
  public JumpThruW(EntityData d, Vector2 offset):base(d,offset){
  }
  public override void MoveHExact(int move) {
    if (!Collidable) goto end;
    foreach (Actor entity in base.Scene.Tracker.GetEntities<Actor>()){
      if(!entity.IsRiding(this)) continue;
      if(entity.TreatNaive) entity.NaiveMove(Vector2.UnitX*move);
      else entity.MoveHExact(move);
      entity.LiftSpeed=LiftSpeed;
    }
    end:
    base.X += move;
    MoveStaticMovers(Vector2.UnitX * move);
  }
}