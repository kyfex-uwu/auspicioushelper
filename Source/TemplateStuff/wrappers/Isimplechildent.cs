



using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper.Wrappers;
public interface ISimpleChild:ITemplateChild{
  void ITemplateChild.AddAllChildren(List<Entity> l){
    l.Add((Entity)this);
  }
  void ITemplateChild.addTo(Scene s){
    s.Add((Entity) this);
  }
}