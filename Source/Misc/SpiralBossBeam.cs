

using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class SpiralBossBeam:Entity {
  Sprite beamSprite;
  public static ParticleType P_Dissipate = new ParticleType{
      Color = Calc.HexToColor("e60022"),
      Size = 1f,
      FadeMode = ParticleType.FadeModes.Late,
      SpeedMin = 15f,
      SpeedMax = 30f,
      DirectionRange = (float)Math.PI / 3f,
      LifeMin = 0.3f,
      LifeMax = 0.6f
  };
  public SpiralBossBeam(EntityData data, Vector2 offset):base(data.Position+offset){
    Add(beamSprite = GFX.SpriteBank.Create("badeline_beam"));
  }
}