


using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class PortalIntersectInfoH{
  public Actor a;
  public PortalOthersider m;
  public PortalGateH p;
  bool ce; //true if real thing is on node side
  bool facesign = false;
  Vector2 pmul;
  public bool end;
  public PortalIntersectInfoH(bool end, PortalGateH p, Actor a){
    this.p=p;
    this.a=a;
    ce = end;
    pmul = p.flipped?new Vector2(-1,1):new Vector2(1,1);
    facesign = Math.Sign(a.CenterX-p.getpos(end).X)>=0;
  }
  public PortalOthersider addOthersider(){
    a.Scene.Add(m=new PortalOthersider(a.Position, this));
    m.Center=getOthersiderPos();
    m.Scene = a.Scene;
    return m;
  }
  public Vector2 getOthersiderPos(){
    return pmul*(a.Center-p.getpos(ce))+p.getpos(!ce);
  }
  public Vector2 calcspeed(Vector2 speed, bool newend){
    Vector2 rel = speed-p.getspeed(!newend);
    if(p.flipped) rel.X*=-1;
    rel+=p.getspeed(newend);
    return rel;
  }
  public void swap(){
    ce = !ce;
    //DebugConsole.Write("swap "+a.Position.ToString()+" "+m.Position.ToString());
    Vector2 temp = a.Center;
    a.Center=m.Center;
    m.Center=temp;
    PortalGateH.evalEnt(a);
    PortalGateH.collideLim[m]=p.getSidedCollidelim(!ce);

    if(a is Player pl){
      pl.Speed = calcspeed(pl.Speed,ce);
    } else if(a is Glider g){
      g.Speed += calcspeed(g.Speed,ce);
    }
    facesign = Math.Sign(a.CenterX-p.getpos(ce).X)>=0;
  }
  public bool finish(){
    float center = a.CenterX;
    bool nsign = Math.Sign(a.CenterX - p.getpos(ce).X)>=0;
    m.Center=getOthersiderPos();
    if(facesign != nsign)swap();
    end = (Math.Sign((facesign?a.Left:a.Right)-p.getpos(ce).X)>=0)==facesign;
    if(end)DebugConsole.Write("ended");
    return end;
  }
  public bool applyDummyPush(Vector2 amount){
    if(!m.propegateMove) return false;
    bool hblock = amount.X==0?false:a.MoveHExact((int)amount.X);
    
    return true;
  }

}