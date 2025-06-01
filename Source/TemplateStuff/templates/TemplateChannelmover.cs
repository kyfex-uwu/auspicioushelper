

using Celeste.Mod.Entities;
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/TemplateChannelmover")]
public class TemplateChannelmover:Template, IChannelUser{
  Vector2 movevec;
  float relspd;
  float asym;
  float dir;
  float prog;
  public string channel {get;set;}
  public override Vector2 virtLoc => Position+prog*movevec;
  public TemplateChannelmover(EntityData d, Vector2 offset):this(d,offset,d.Int("depthoffset",0)){}
  public TemplateChannelmover(EntityData d, Vector2 offset, int depthoffset)
  :base(d,d.Position+offset,depthoffset){
    movevec = d.Nodes[0]-d.Position;
    channel = d.Attr("channel","");
    relspd = 1/d.Float("move_time",1);
    asym = d.Float("asymmetry",1f);
  }
  public void setChVal(int val){
    dir = (val&1)==1?1:-1*asym;
  }
  public override void addTo(Scene scene){
    ChannelState.watch(this);
    dir = (ChannelState.readChannel(channel) &1)==1?1:-1*asym;
    prog = dir == 1?1:0;
    base.addTo(scene);
  }
  public override void Update(){
    base.Update();
    float lprog = prog;
    prog = System.Math.Clamp(prog+dir*relspd*Engine.DeltaTime,0,1);
    if(lprog != prog){
      ownLiftspeed = dir*relspd*movevec;
      childRelposSafe();
    } else {
      ownLiftspeed = Vector2.Zero;
    }
  }
}