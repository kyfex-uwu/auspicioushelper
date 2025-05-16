



using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper.Wrappers;

public interface ISimpleEnt:ITemplateChild{
  void ITemplateChild.AddAllChildren(List<Entity> l){
    l.Add((Entity)this);
  }
  void ITemplateChild.addTo(Scene s){
    if(this is Entity e){
      s.Add(e);
      e.Scene = s;
    } else {
      DebugConsole.Write($"{this} implements simpleent wihtout being entity");
    }
  }
  void ITemplateChild.destroy(bool particles){
    ((Entity) this).RemoveSelf();
  }
}