


using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mods.auspicioushelper;
using System;
using Celeste.Mod.Entities;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/ChannelSprite")]
public class ChannelSprite:ChannelBaseEntity{
  public int num;
  public Sprite sprite;
  public enum edgeTypes{
    loop, clamp, hide,
  }
  edgeTypes ty;
  public ChannelSprite(EntityData d, Vector2 offset):base(d.Position+offset){
    channel=d.Attr("channel","");
    Add(sprite=GFX.SpriteBank.Create(d.Attr("xml_spritename")));
    if(d.Bool("attached",false)){
      StaticMover staticMover = new StaticMover() {
          SolidChecker = solid => solid.CollideRect(new Rectangle((int) X, (int) Y - 1, 8,8)),
          OnMove = move
      };
      Add(staticMover);
    }
    num = d.Int("cases",1);
    ty=d.Attr("edge_type","") switch {
      "loop"=>edgeTypes.loop,
      "clamp"=>edgeTypes.clamp,
      _=>edgeTypes.hide,
    };
  }
  public override void setChVal(int val){
    if(val<0 || val>=num){
      switch(ty){
        case edgeTypes.loop: val=val%num; break;
        case edgeTypes.clamp: val=Math.Clamp(val,0,num-1);break;
        default:
          sprite.Visible=false;
          return;
      }
    }
    sprite.Visible=true;
    sprite.Play("case"+val.ToString());
  }
  public override void Added(Scene scene){
    setChVal(ChannelState.readChannel(channel));
    ChannelState.watch(this);
  }
  public void move(Vector2 amount){
    DebugConsole.Write(amount.ToString());
  }
}