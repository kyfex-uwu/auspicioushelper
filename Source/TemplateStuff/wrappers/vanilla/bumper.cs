


using Celeste.Mod.auspicioushelper.Wrappers;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper.Wrappers;

public class Bumperw : Bumper, ISimpleEnt {
  public Template parent { get; set; }
  public void relposTo(Vector2 pos, Vector2 ls) {
    rpp = pos;
  }
  
  Vector2 rpp;
  Vector2 toffset;
  public Bumperw(EntityData e, Vector2 o):base(e,o){}
  public override void Update() {
    anchor = rpp+toffset;
    base.Update();
  }
  public void setOffset(Vector2 ppos) {
    toffset=Position-ppos;
    rpp = ppos;
  }
  public void parentChangeStat(int vis, int col, int act){
    if(vis!=0) Visible = vis>0;
    if(col!=0) Collidable = col>0;
    if(act!=0) Active = act>0;
  }
}