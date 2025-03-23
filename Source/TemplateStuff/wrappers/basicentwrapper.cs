




using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper.Wrappers;

public class BasicEnt:Entity, ITemplateChild{
  public Template parent{get;set;}
  public Template.Propagation prop {get;} = Template.Propagation.Shake;
  Entity e;
  Vector2 toffset;
  public BasicEnt(Entity e, Template t, Vector2 offset):base(t.Position){
    e.Depth += t.depthoffset;
    this.e=e;
    parent = t;
    toffset = offset;
  }
  public void relposTo(Vector2 loc, Vector2 liftspeed){
    e.Position = loc+toffset;
  }
  public void addTo(Scene scene){
    scene.Add(this);
    scene.Add(e);
  }
}