


using System.Collections.Generic;
using Celeste.Mod.Entities;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/TemplateCassetteBlock")]
public class TemplateCassetteBlock:TemplateDisappearer, IMaterialObject, IChannelUser, ITemplateChild{
  
  public string channel{get;set;}
  public bool there = true;
  public List<Entity> todraw;
  public TemplateCassetteBlock(EntityData d, Vector2 offset):this(d,offset,d.Int("depthoffset",0)){}
  public TemplateCassetteBlock(EntityData d, Vector2 offset, int depthoffset)
  :base(d.Attr("template",""),d.Position+offset,depthoffset){
    channel = d.Attr("channel","");
  }
  public override void Added(Scene scene){
    base.Added(scene);
    ChannelState.watch(this);
    there = ChannelState.readChannel(channel) !=0;
    setChVal(there?1:0);
    if(CassetteMaterialLayer.layers.TryGetValue(channel,out var layer)){
      var l = new List<Entity>();
      AddAllChildren(l);
      layer.dump(l);
    }
  }
  public void setChVal(int val){
    there = val != 0;
    setVisCol(there,there);
  }
  public void renderMaterial(IMaterialLayer l, SpriteBatch s, Camera c){
    if(there) return;
    SpriteBatch origsb = Draw.SpriteBatch;
    Draw.SpriteBatch = s;
    foreach(Entity e in todraw) e.Render();
    Draw.SpriteBatch = origsb;
  }
}