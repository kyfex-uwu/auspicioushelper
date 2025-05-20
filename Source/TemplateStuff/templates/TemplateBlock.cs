



using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/TemplateBlock")]
[Tracked]
class TemplateBlock:TemplateDisappearer{
  public TemplateBlock(EntityData d, Vector2 offset):this(d,offset,d.Int("depthoffset",0)){}
  bool uvis;
  bool ucol;
  bool uact;
  bool candash;
  bool persistent;
  string breaksfx;
  public TemplateBlock(EntityData d, Vector2 offset, int depthoffset)
  :base(d,d.Position+offset,depthoffset){
    uvis = d.Bool("visible",true);
    ucol = d.Bool("collidable",true);
    uact = d.Bool("active",true);
    candash = !d.Bool("only_redbubble_or_summit_launch",false);
    persistent = d.Bool("persistent",false);
    breaksfx = d.Attr("breaksfx","event:/game/general/wall_break_stone");
    if(d.Bool("canbreak",true)){
      OnDashCollide = (Player p, Vector2 dir)=>{
        if (!candash && p.StateMachine.State != 5 && p.StateMachine.State != 10){
          return DashCollisionResults.NormalCollision;
        }
        setCollidability(false);
        Audio.Play(breaksfx,Position);
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
  public override void Update() {
    base.Update();
    awaketests.enable();
  }


  public override void Awake(Scene scene) {
    base.Awake(scene);
  }
  static HookManager awaketests = new HookManager(()=>{
    List<Entity> l = Engine.Instance.scene.Tracker.GetEntities<TemplateBlock>().ToList();
    DebugConsole.Write(l.Count.ToString());
    if(l.Count<2) return;
    TemplateBlock t1=l[0] as TemplateBlock;
    TemplateBlock t2=l[1] as TemplateBlock;
    MipGrid m1 = new MipGrid(t1.fgt.Grid);
    MipGrid m2 = new MipGrid(t2.fgt.Grid);
    DebugConsole.Write($"{m1.tlc} {m2.tlc} {m1.collideMipGrid(m2)}");
    //DebugConsole.Write(MipGrid.getBlockstr(m2.layers[0].getArea(-4,-2)));
  },()=>{},auspicioushelperModule.OnReset);
}