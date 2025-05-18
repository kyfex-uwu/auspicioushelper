

using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class TemplateGluable:Template{
  string lookingFor;
  Vector2 lpos;
  Entity gluedto;
  int smearamount;
  Vector2[] pastLiftspeed;
  bool averageSmear;
  public override Vector2 gatheredLiftspeed => ownLiftspeed;
  void evalLiftspeed(bool precess = true){
    float mX=0;
    float mY=0;
    if(!averageSmear)foreach(Vector2 v in pastLiftspeed){
      if(MathF.Abs(v.X)>MathF.Abs(mX)) mX=v.X;
      if(MathF.Abs(v.Y)>MathF.Abs(mY)) mY=v.Y;
    } else foreach(Vector2 v in pastLiftspeed){
      mX+=v.X/smearamount; mY+=v.Y/smearamount;
    }
    ownLiftspeed = new Vector2(mX,mY);
    if(!precess) return; 
    for(int i=smearamount-1; i>=1; i--){
      pastLiftspeed[i]=pastLiftspeed[i-1];
    }
    pastLiftspeed[0]=Vector2.Zero;
  }
  public TemplateGluable(EntityData d, Vector2 offset):this(d,offset,d.Int("depthoffset",0)){}
  public TemplateGluable(EntityData d, Vector2 offset, int depthoffset)
  :base(d,d.Position+offset,depthoffset){
    lookingFor = d.Attr("glue_to");
    smearamount = d.Int("liftspeed_smear",4);
    pastLiftspeed = new Vector2[smearamount];
    averageSmear = d.Bool("smear_average",false);
  }
  public void make(Entity e){
    Depth = e.Depth-1;
    lpos = e.Position;
    Position = e.Position;
    base.addTo(Scene);
  }
  public override void addTo(Scene scene){
    gluedto = FoundEntity.find(lookingFor)?.Entity;
    if(gluedto != null) make(gluedto);
  }
  public override void Awake(Scene scene){
    base.Awake(scene);
    if(gluedto == null) addTo(Scene);
  }
  public override void Update(){
    base.Update();
    if(gluedto.Scene == null){
      gluedto = null;
      destroy(true);
      return;
    } 
    if(gluedto.Position!=lpos){
      var move = gluedto.Position-lpos;
      pastLiftspeed[0]+=move/Math.Max(Engine.DeltaTime,0.005f);
      evalLiftspeed();
      relposTo(Position,ownLiftspeed);
      lpos = gluedto.Position;
    }
    evalLiftspeed(true);
  }
}