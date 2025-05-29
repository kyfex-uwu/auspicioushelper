



using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper.Wrappers;

public interface ISimpleEnt:ITemplateChild{
  Template.Propagation ITemplateChild.prop => Template.Propagation.Shake;
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

public interface ISimpleWrapper:ITemplateChild{
  Entity wrapped {get;}
  Vector2 toffset {get;set;}
  void ITemplateChild.AddAllChildren(List<Entity> list) {
    list.Add(wrapped);
  }
  void ITemplateChild.addTo(Scene s) {
    DebugConsole.Write($"adding from wrapper: {wrapped}");
    s.Add(wrapped);
  }
  void ITemplateChild.destroy(bool particles) {
    wrapped.RemoveSelf();
  }
  void ITemplateChild.parentChangeStat(int vis, int col, int act) {
    if(vis!=0) wrapped.Visible = vis>0;
    if(col!=0) wrapped.Collidable = col>0;
    if(act!=0) wrapped.Active = col>0;
  }
  void ITemplateChild.setOffset(Vector2 ppos) {
    toffset = wrapped.Position-ppos;
  }
}