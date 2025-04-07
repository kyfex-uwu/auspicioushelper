




using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper.Wrappers;

// public class BasicEnt:Entity, ITemplateChild{
//   public Template parent{get;set;}
//   public Template.Propagation prop {get;} = Template.Propagation.Shake;
//   Entity e;
//   Vector2 toffset;
//   public BasicEnt(Entity e, Template t, Vector2 offset):base(t.Position){
//     e.Depth += t.depthoffset;
//     this.e=e;
//     parent = t;
//     toffset = offset;
//   }
//   public void relposTo(Vector2 loc, Vector2 liftspeed){
//     e.Position = loc+toffset;
//   }
//   public void addTo(Scene scene){
//     scene.Add(this);
//     scene.Add(e);
//   }
// }

public class BasicMultient:Entity, ITemplateChild{
  public Template parent{get;set;}
  public Template.Propagation prop {get;} = Template.Propagation.Shake;
  public struct EntEnt{ //entity entry, say it faster
    public Vector2 offset;
    public Entity e;
    public EntEnt(Entity e, Vector2 o){
      offset=o; this.e=e;
    }
  }
  List<EntEnt> ents = new List<EntEnt>();
  public BasicMultient(Template t):base(t.virtLoc){
    parent = t;
    Depth = -9000+t.depthoffset;
  }
  public void add(Entity e, Vector2 offset){
    ents.Add(new EntEnt(e,offset));
    if(e.Scene == null && this.Scene != null) Scene.Add(e);
  }
  public void relposTo(Vector2 loc, Vector2 liftspeed){
    foreach(var en in ents){
      en.e.Position=(en.e is Decal)?(loc+en.offset).Round():loc+en.offset;
    }
  }
  public void sceneadd(Scene scene){
    this.Scene = scene;
    scene.Add(this);
    foreach(EntEnt en in ents){
      scene.Add(en.e);
    }
  }
  public void AddAllChildren(List<Entity> l){
    foreach(EntEnt ent in ents)l.Add(ent.e);
  }
  public void parentChangeStat(int vis, int col){
    foreach(EntEnt ent in ents){
      if(vis!=0) ent.e.Visible = vis>0; 
      if(col!=0){
        ent.e.Collidable = col>0; 
        foreach(Component c in ent.e.Components){
          if(c is PlayerCollider cl) cl.Active=col>0;
        }
      }
    }
  }
}