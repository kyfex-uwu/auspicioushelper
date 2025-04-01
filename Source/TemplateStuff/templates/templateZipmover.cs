


using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/TemplateZipmover")]
public class TemplateZipmover:Template{
    public enum Themes{
        Normal,
        Moon
    }
  private SoundSource sfx = new SoundSource();
  Themes theme = Themes.Normal;
  Vector2 virtLoc;
  SplineAccessor spos;
  EntityData dat;
  Vector2 offset;
  public enum ReturnType{
    loop,
    none,
    normal,
  }
  ReturnType rtype;
  public enum ActivationType{
    ride,
    dash,
    rideAutomatic,
    dashAutomatic,
  }
  ActivationType atype;
  public TemplateZipmover(EntityData d, Vector2 offset):this(d,offset,d.Int("depthoffset",0)){}
  public TemplateZipmover(EntityData d, Vector2 offset, int depthoffset)
  :base(d.Attr("template",""),d.Position+offset,depthoffset){
    Add(new Coroutine(FancySequence()));
    virtLoc = Position;
    Add(sfx);
    dat=d;
    this.offset=offset;
    rtype = d.Attr("return_type","normal") switch {
      "loop"=>ReturnType.loop,
      "none"=>ReturnType.none,
      _=>ReturnType.normal
    };
    atype = d.Attr("activation_type","ride") switch {
      "ride"=>ActivationType.ride,
      "dash"=>ActivationType.dash,
      "rideAutomatic"=>ActivationType.rideAutomatic,
      "dashAutomatic"=>ActivationType.dashAutomatic,
      _=>ActivationType.ride,
    };
  }
  public override void Added(Scene scene){
    base.Added(scene);
    Spline spline=null;
    if(!string.IsNullOrEmpty(dat.Attr("spline"))){
      Spline.splines.TryGetValue(dat.Attr("spline"), out spline);
      if(spline == null){
        spline = SplineEntity.constructImpl(dat.Position,dat.Nodes,offset,dat.Attr("spline"),dat.Bool("lastNodeIsKnot",true));
      }
    }
    if(spline == null){
      DebugConsole.Write("using fallback spline");
      LinearSpline l =new LinearSpline();
      spline = l;
      l.fromNodes(SplineEntity.entityInfoToNodes(dat.Position,dat.Nodes,offset,dat.Bool("lastNodeIsKnot",true)));
    }
    spos = new SplineAccessor(spline, Vector2.Zero);
  }
  public override void Update(){
    base.Update();
  }
  Vector2 ownLiftspeed;

  /*private IEnumerator Sequence(){
    while(true){
      if(!hasRiders<Player>()){
        yield return null; continue;
      }
      sfx.Play((theme == Themes.Normal) ? "event:/game/01_forsaken_city/zip_mover" : "event:/new_content/game/10_farewell/zip_mover");
      yield return 0.1f;
      float at = 0;
      //DebugConsole.Write(virtLoc.ToString());
      while(at<1f){
        yield return null;
        at = Calc.Approach(at, 1f, 2f * Engine.DeltaTime);
        progress = Ease.SineIn(at);
        Vector2 old = virtLoc;
        virtLoc = Position+spos.getPos(progress);
        ownLiftspeed = Math.Sign(Engine.DeltaTime)*(virtLoc-old)/Engine.DeltaTime;
        childRelposTo(virtLoc,ownLiftspeed);
      }
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      SceneAs<Level>().Shake();
      yield return 0.5f;
      float at2 = 0;
      while (at2 < 1f){
        yield return null;
        at2 = Calc.Approach(at2, 1f, 0.5f * Engine.DeltaTime);
        progress = 1f - Ease.SineIn(at2);
        Vector2 old = virtLoc;
        virtLoc = Position+spos.getPos(progress);
        ownLiftspeed = Math.Sign(Engine.DeltaTime)*(virtLoc-old)/Engine.DeltaTime;
        childRelposTo(virtLoc,ownLiftspeed);
      }
      yield return 0.5f;
    }
  }*/
  private IEnumerator FancySequence(){
    bool done; float at;
    waiting:
      yield return null;
      if(atype == ActivationType.ride || atype==ActivationType.rideAutomatic){
        if(hasRiders<Player>()) goto going;
      } else if(atype == ActivationType.dash || atype==ActivationType.dashAutomatic){
        throw new NotImplementedException();
      }
      goto waiting;

    going:
      sfx.Play((theme == Themes.Normal) ? "event:/game/01_forsaken_city/zip_mover" : "event:/new_content/game/10_farewell/zip_mover");
      yield return 0.1f;
      at=0;
      done = false;
      while(!done){
        yield return null;
        at+=Engine.DeltaTime*2;
        Vector2 old = virtLoc;
        done = spos.towardsNext((float)(2*(Math.PI/2)*Math.Sin(at*Math.PI/2)*Engine.DeltaTime));
        //DebugConsole.Write($"{at}, {spos.t}");
        virtLoc = Position+spos.pos;
        ownLiftspeed = Math.Sign(Engine.DeltaTime)*(virtLoc-old)/Engine.DeltaTime;
        childRelposTo(virtLoc, ownLiftspeed);
      }
      
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      SceneAs<Level>().Shake();
      yield return 0.25f;

      if((atype == ActivationType.rideAutomatic || atype==ActivationType.dashAutomatic) && 
      (rtype == ReturnType.loop || spos.t<spos.numsegs-1)){
        sfx.Stop();
        goto going;
      } else{
        if(spos.t==spos.numsegs-1 && rtype == ReturnType.normal)goto returning;
        sfx.Stop();
        if(spos.t<spos.numsegs-1 || rtype == ReturnType.loop)goto waiting;
        else yield break;
      }

    returning:
      yield return 0.25f;
      sfx.Stop();
      sfx.Play((theme == Themes.Normal) ? "event:/game/01_forsaken_city/zip_mover" : "event:/new_content/game/10_farewell/zip_mover");
      sfx.instance.setTimelinePosition(1000);
      done = false;
      at=0;
      while(!done){
        yield return null;
        at+=Engine.DeltaTime/2;
        Vector2 old = virtLoc;
        done = spos.towardsNext((float)(-(Math.PI/2)*Math.Sin(at*Math.PI/2)*Engine.DeltaTime/2));
        virtLoc = Position+spos.pos;
        ownLiftspeed = Math.Sign(Engine.DeltaTime)*(virtLoc-old)/Engine.DeltaTime;
        childRelposTo(virtLoc, ownLiftspeed);
      }
      if(spos.t>0) goto returning;
      yield return 0.5f;
      goto waiting;
  }
  public override void relposTo(Vector2 loc,Vector2 liftspeed){
    Position = loc+toffset;
    virtLoc = Position+spos.pos;
    childRelposTo(virtLoc,liftspeed+ownLiftspeed);
  }
}