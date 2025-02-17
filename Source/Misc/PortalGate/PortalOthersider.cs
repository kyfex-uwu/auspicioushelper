


using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class PortalOthersider:Actor{
  public PortalIntersectInfoH info;
  

  public PortalOthersider(Vector2 pos, PortalIntersectInfoH h):base(pos){
    Actor copying = h.a;
    Depth=copying.Depth;
    Collider chit = copying.Collider;
    Collider = new Hitbox(chit.Width,chit.Height,chit.Position.X,chit.Position.Y);
    info=h;
  }
  public override void Render(){
    base.Render();
    Vector2 unrendpos = Position;
    Center = info.getOthersiderPos();
    if(info.end)return;
    var delta = -info.a.Position+Position;
    float facing=1;
    if(info.a is Player p){
      PlayerHair h = p.Get<PlayerHair>();
      facing=(float)p.Facing;
      var oldpos = p.Position;
      p.Position+=delta;
      for(int i=0; i<h.Sprite.HairCount; i++){
        h.Nodes[i]+=delta;
      }
      p.Render();
      for(int i=0; i<h.Sprite.HairCount; i++){
        h.Nodes[i]-=delta;
      }
      p.Position=oldpos;
      //return;
    }

    foreach(Component c in info.a.Components){
      if(c is Sprite s){
        var oldpos = s.RenderPosition;
        s.RenderPosition = oldpos+delta;
        s.Scale.X*=facing;
        s.Render();
        s.Scale.X*=facing;
        s.RenderPosition = oldpos;
      }
    }
    Position=unrendpos;
  }
  public override void Update(){
    base.Update();
    //DebugConsole.Write(info.getOthersiderPos().ToString());
    Center = info.getOthersiderPos();
    if(info.end) RemoveSelf();
  }
}