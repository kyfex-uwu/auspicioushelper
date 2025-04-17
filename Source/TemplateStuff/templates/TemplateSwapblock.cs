


using System;
using System.Collections;
using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/TemplateSwapblock")]
public class TemplateSwapblock:Template{
  EventInstance movesfx;
  float progress {
    get=>spos.t;
    set=>spos.t=value;
  }
  float target=0;
  public override Vector2 virtLoc=>Position+spos.pos;
  SplineAccessor spos;
  EntityData dat;
  Vector2 offset;
  float speed=0;
  const float maxspeed = 360;
  public TemplateSwapblock(EntityData d, Vector2 offset):this(d,offset,d.Int("depthoffset",0)){}
  public TemplateSwapblock(EntityData d, Vector2 offset, int depthoffset)
  :base(d.Attr("template",""),d.Position+offset,depthoffset,d.ID){
    dat=d;
    this.offset=offset;
    //DebugConsole.Write($"{t.childEntities.Count}");
  }
  public override void Update(){
    base.Update();
    if(progress!=target){
      speed = Calc.Approach(speed,maxspeed,maxspeed*Engine.DeltaTime*6);
      Vector2 old = virtLoc;
      bool done = spos.towardsNextDist(speed*Engine.DeltaTime);
      if(done && progress ==0){
        target = target%spos.numsegs;
      }
      ownLiftspeed = (virtLoc-old).SafeNormalize()*speed;
      childRelposTo(virtLoc,gatheredLiftspeed);
    } else {
      speed=0;
      Audio.Stop(movesfx);
      ownLiftspeed = Vector2.Zero;
    }
  }
  public void activate(){
    movesfx = Audio.Play("event:/game/05_mirror_temple/swapblock_move", Center);
    target+=1;
    speed=Math.Max(speed,maxspeed/3);
  }
  public override void addTo(Scene scene){
    Spline spline=null;
    if(!string.IsNullOrEmpty(dat.Attr("spline"))){
      Spline.splines.TryGetValue(dat.Attr("spline"), out spline);
    }
    if(spline == null){
      LinearSpline l =new LinearSpline();
      spline = l;
      l.fromNodesAllRed(SplineEntity.entityInfoToNodes(dat.Position,dat.Nodes,offset,false));
    }
    spos = new SplineAccessor(spline, Vector2.Zero);
    Add(new DashListener((Vector2 dir)=>activate()));
    base.addTo(scene);
  }
  // public override void relposTo(Vector2 loc, Vector2 liftspeed){
  //   Position = loc+toffset;
  //   virtLoc = Position+spos.pos;
  //   childRelposTo(virtLoc,ownLiftspeed+liftspeed);
  // }
}