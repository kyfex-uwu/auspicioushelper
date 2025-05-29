


using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper.Wrappers;

public class CWire:Wire, ISimpleEnt{
  public Template parent {get;set;}
  Vector2 fromOffset;
  Vector2 toOffset;
  public CWire(EntityData d, Vector2 o):base(d,o){
  }
  public void setOffset(Vector2 ppos){
    fromOffset = Curve.Begin-ppos;
    toOffset = Curve.End-ppos;
  }
  public void relposTo(Vector2 loc, Vector2 ls){
    Curve.Begin = fromOffset+loc;
    Curve.End = toOffset+loc;
  }
}
