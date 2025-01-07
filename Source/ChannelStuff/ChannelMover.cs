using Celeleste.Mods.auspicioushelper;
using Celeste.Mod.auspicioushelper;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mods.auspicioushelper;
[Tracked]
[CustomEntity("auspicioushelper/ChannelMover")]
public class ChannelMover:Solid, IChannelUser, IMaterialObject{
  public Vector2 p0;
  public Vector2 p1;
  public float width;
  public float height;
  public float relspd;
  public float prog;
  public int channel {get; set;}
  public float dir;
  public ChannelMover(EntityData data, Vector2 offset):base(data.Position,data.Width, data.Height, data.Bool("safe",false)){
    width = data.Width;
    height = data.Height;
    p0 = data.Position+offset;
    p1 = data.Nodes[0]+offset;
    channel = data.Int("channel",0);
    relspd = 1/data.Float("move_time",1);
  }
  public void setChVal(int val){
    dir = (val&1)==1?1:-1;
  }
  public override void Added(Scene scene){
    base.Added(scene);
    ChannelState.watch(this);
    dir = (ChannelState.readChannel(channel) &1)==1?1:-1;
    Position = dir==1?p1:p0;
    prog = dir == 1?1:0;
  }
  public override void Update(){
    base.Update();
    float lprog = prog;
    prog = System.Math.Clamp(prog+dir*relspd*Engine.DeltaTime,0,1);
    if(lprog != prog){
      MoveTo(prog*p1+(1-prog)*p0);
    }
  }
  public override void Render()
  {
    base.Render();
    Draw.Rect(Position, width, height, Color.AliceBlue);
  }
  public void registerMaterials(){
    ChannelBaseEntity.layerA.planDraw(this);
  }
  public void renderMaterial(MaterialLayer l, SpriteBatch sb, Camera c){
    sb.Draw(Draw.Pixel.Texture.Texture_Safe,new Rectangle((int) Position.X, (int) Position.Y,(int) width, (int) height), Draw.Pixel.ClipRect, new Color(1,0,channel%256,255));
  }
}