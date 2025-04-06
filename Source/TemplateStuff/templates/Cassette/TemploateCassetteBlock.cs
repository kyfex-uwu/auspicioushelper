


using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Celeste.Editor;
using Celeste.Mod.Entities;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/TemplateCassetteBlock")]
public class TemplateCassetteBlock:TemplateDisappearer, IMaterialObject, IChannelUser, ITemplateChild{
  
  public string channel{get;set;}
  enum State {
    gone, trying, there
  }
  State there = State.there;
  public List<Entity> todraw=new List<Entity>();
  public bool doBoost;
  public bool doRaise;
  public override Vector2 virtLoc =>Position+Vector2.UnitY*hoffset;
  float hoffset; 
  public TemplateCassetteBlock(EntityData d, Vector2 offset):this(d,offset,d.Int("depthoffset",0)){}
  public TemplateCassetteBlock(EntityData d, Vector2 offset, int depthoffset)
  :base(d.Attr("template",""),d.Position+offset,depthoffset){
    channel = d.Attr("channel","");
    prop = prop&~Propagation.Inside;
    doBoost = d.Bool("do_boost",false);
    doRaise = d.Bool("do_raise",false);
  }
  public override void Added(Scene scene){
    base.Added(scene);
    ChannelState.watch(this);
    setChVal(ChannelState.readChannel(channel));
    if(CassetteMaterialLayer.layers.TryGetValue(channel,out var layer)){
      AddAllChildren(todraw);
      layer.dump(todraw);
    }
  }
  public void tryManifest(){
    Player p = Scene?.Tracker.GetEntity<Player>();
    if(there!=State.trying) return;
    bool inLayer = CassetteMaterialLayer.layers.TryGetValue(channel,out var layer);
    if(getParentCol() && p!=null && hasInside(p)){
      p.Position.Y-=4;
      bool inside = hasInside(p);
      p.Position.Y+=4;
      bool flag = false;
      if(!inside){
        setCollidability(true);
        for(int i=0; i<4; i++){
          if (!p.CollideCheck<Solid>(p.Position - Vector2.UnitY * i)){
            p.Position -= Vector2.UnitY * i;
            flag = true;
            break;
          }
        }
        if(!flag)setCollidability(false);
      }
      if(inLayer){
        layer.addTrying(this);
      }
      if(!flag) return;
    }
    if(inLayer)layer.removeTrying(this);
    there = State.there;
    prop|=Propagation.Inside;
    setVisCol(true,true);
  }
  float bumpTarget = 1;
  IEnumerator bumpUp(){
    float at = 0;
    while(at>-bumpTarget){
      at=Calc.Approach(at,-bumpTarget,Engine.DeltaTime*120);
      hoffset = at;
      ownLiftspeed = Vector2.UnitY*-60;
      childRelposTo(virtLoc,gatheredLiftspeed);
      yield return null;
    }
    yield return 0.2f;
    while(at<0){
      at=Calc.Approach(at,0,Engine.DeltaTime*30);
      hoffset = at;
      ownLiftspeed = Vector2.UnitY*30;
      childRelposTo(virtLoc,gatheredLiftspeed);
      yield return null;
    }
  }
  public void setChVal(int val){
    if(val==0){
      if(there == State.there){
        setVisCol(false,false);
      }
      there = State.gone;
      prop&=~Propagation.Inside;
    } else {
      there = State.trying;
      tryManifest();
      if(doBoost)Add(new Coroutine(bumpUp()));
    }
  }
  public void renderMaterial(IMaterialLayer l, SpriteBatch s, Camera c){
    if(there == State.there) return;
    SpriteBatch origsb = Draw.SpriteBatch;
    Draw.SpriteBatch = s;
    foreach(Entity e in todraw) if(e.Scene != null && e.Depth<=l.depth)e.Render();
    Draw.SpriteBatch = origsb;
  }
  public override void Update(){
    base.Update();
    if(there == State.trying) tryManifest();
  }
}