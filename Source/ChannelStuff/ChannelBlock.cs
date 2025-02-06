using System;
using Celeleste.Mods.auspicioushelper;
using Celeste.Mod.Entities;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
namespace Celeste.Mod.auspicioushelper;

[Tracked]
[CustomEntity("auspicioushelper/ChannelBlock")]
public class ChannelBlock:ChannelBaseEntity, IMaterialObject {
  public bool inverted;
  public float width;
  public float height;
  public bool safe;
  public bool alwayspresent;
  enum SolidState {
    gone,
    trying,
    there,
  }
  SolidState curstate;
  Solid solid;
  public ChannelBlock(EntityData data, Vector2 offset):base(data.Position+offset){
    Depth=-9000;
    channel = data.Attr("channel","");
    inverted = data.Bool("inverted",false);
    safe = data.Bool("safe",false);
    width = data.Width;
    height = data.Height;
    alwayspresent = data.Bool("alwayspresent",false);
    curstate = (ChannelState.readChannel(channel)&1) != (inverted?1:0)?SolidState.there:SolidState.gone;
  }
  public override void Added(Scene scene){
    base.Added(scene);
    scene.Add(solid = new Solid(Position, width, height, safe));
    solid.Collidable = curstate == SolidState.there;
  }
  public override void setChVal(int val){
    curstate = (val&1) != (inverted?1:0)?SolidState.there:SolidState.gone;
    solid.Collidable = curstate == SolidState.there;
  }
  public override void Render(){
    if(curstate == SolidState.there){
      Draw.Rect(Position, width, height, inverted? Color.Blue:Color.Red);
    } else {
      Draw.HollowRect(Position, width, height, Color.Red);
    }
    // just old testing stuff <3
    //Draw.SpriteBatch.Draw(ChannelBaseEntity.layerA.bgtex, new Rectangle((int)Position.X, (int)Position.Y, (int)width, (int)height), Color.White);
    //Draw.SpriteBatch.Draw(ChannelBaseEntity.layerA.outtex, new Rectangle((int)Position.X, (int)Position.Y-(int)height-2, (int)width, (int)height), Color.White);
    //Draw.SpriteBatch.Draw(ChannelBaseEntity.layerA.mattex, new Rectangle((int)Position.X, (int)Position.Y+(int)height+2, (int)width, (int)height), Color.White);
  }

  public void registerMaterials(){
    layerA.planDraw(this);
  }
  public void renderMaterial(MaterialLayer l, SpriteBatch sb, Camera c){
    if(curstate == SolidState.there){
      sb.Draw(Draw.Pixel.Texture.Texture_Safe, new Rectangle((int)Position.X, (int)Position.Y, (int)width, (int)height), Draw.Pixel.ClipRect, new Color(1,0,0,255));
    }
  }
}