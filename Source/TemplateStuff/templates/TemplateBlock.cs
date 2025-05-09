



using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/TemplateBlock")]
class TemplateBlock:TemplateDisappearer{
  public TemplateBlock(EntityData d, Vector2 offset):this(d,offset,d.Int("depthoffset",0)){}
  bool uvis;
  bool ucol;
  bool uact;
  bool candash;
  bool persistent;
  
  public TemplateBlock(EntityData d, Vector2 offset, int depthoffset)
  :base(d.Attr("template",""),d.Position+offset,depthoffset,getOwnID(d)){
    uvis = d.Bool("visible",true);
    ucol = d.Bool("collidable",true);
    uact = d.Bool("active",true);
    candash = !d.Bool("only_redbubble_or_summit_launch",false);
    persistent = d.Bool("persistent",false);
    if(d.Bool("canbreak",true)){
      OnDashCollide = (Player p, Vector2 dir)=>{
        if (!candash && p.StateMachine.State != 5 && p.StateMachine.State != 10){
          return DashCollisionResults.NormalCollision;
        }
        setCollidability(false);
        destroy(true);
        if(persistent) auspicioushelperModule.Session.brokenTempaltes.Add(fullpath);
        return DashCollisionResults.Rebound;
      };
    }
  }
  public override void addTo(Scene scene){
    if(persistent && auspicioushelperModule.Session.brokenTempaltes.Contains(fullpath)){
      RemoveSelf();
    } else {
      base.addTo(scene);
      setVisColAct(uvis,ucol,uact);
    }
  }
}