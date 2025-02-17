


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
    this.pmul = p.flipped?new Vector2(-1,1):new Vector2(1,1);
    this.p=p;
    this.a=a;
    ce = end;
    facesign = Math.Sign(a.CenterX-p.getpos(end).X)>=0;
  }
  public void addOthersider(){
    a.Scene.Add(m=new PortalOthersider(a.Position, this));
    m.Center=getOthersiderPos();
  }
  public Vector2 getOthersiderPos(){
    return pmul*(a.Center-p.getpos(ce))+p.getpos(!ce);
  }
  public void swap(){
    ce = !ce;
    //DebugConsole.Write("swap "+a.Position.ToString()+" "+m.Position.ToString());
    Vector2 temp = a.Center;
    a.Center=m.Center;
    m.Center=temp;
    a.MoveHExact(0);
    Vector2 DelSpeed = p.getspeed(ce)-p.getspeed(!ce);
    //DebugConsole.Write("swapend "+a.Position.ToString()+" "+PortalGateH.collideLim[a]);
    DebugConsole.Write(p.getspeed(true).ToString()+" "+p.getspeed(false).ToString());
    if(a is Player pl){
      pl.Speed += DelSpeed;
    } else if(a is Glider g){
      g.Speed += DelSpeed;
    }
    facesign = Math.Sign(a.CenterX-p.getpos(ce).X)>=0;
  }
  public bool finish(){
    float center = a.CenterX;
    bool nsign = Math.Sign(a.CenterX - p.getpos(ce).X)>=0;
    m.Center=getOthersiderPos();
    if(facesign != nsign)swap();
    end = (Math.Sign((facesign?a.Left:a.Right)-p.getpos(ce).X)>=0)==facesign;
    return end;
  }
  public bool applyDummyPush(Vector2 amount){
    bool hblock = a.MoveHExact((int)amount.X);
    
    return true;
  }

}