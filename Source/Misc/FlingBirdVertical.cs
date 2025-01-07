
using System.Collections;
using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeleste.Mods.auspicioushelper;
[CustomEntity("auspicioushelper/VerticalFlingBird")]
public class VerticalFlingBird:FlingBird{
  public static bool hooked = false;
  public VerticalFlingBird(EntityData data, Vector2 offset):base(data, offset){
    if(!hooked){
      hooked = true;
      On.Celeste.FlingBird.LeaveRoutine += LeaveRoutineHook;
    }
  }
  private static IEnumerator LeaveRoutineHook(On.Celeste.FlingBird.orig_LeaveRoutine orig, FlingBird self){
    if(self is VerticalFlingBird){
      self.sprite.Scale.X = 1f;
      self.sprite.Play("fly");
      Vector2 vector = new Vector2(self.X, (self.Scene as Level).Bounds.Top - 32);
      yield return self.MoveOnCurve(self.Position, (self.Position + vector) * 0.5f - Vector2.UnitX * 12f, vector);
      self.RemoveSelf();
    }else{
      yield return orig(self);
    }
  }
}