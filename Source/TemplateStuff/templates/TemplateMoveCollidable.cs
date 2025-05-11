


using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class TemplateMoveCollidable:TemplateDisappearer{
  
  public class CoarseCollisionRast:LinearRaster<Solid>{
    public void Fill(Scene s,FloatRect f, Vector2 step, float maxt, HashSet<Solid> exclude){
      Clear();
      Fill(s.Tracker.GetEntities<Solid>()
        .Where(h=>h.Collidable && (exclude == null || !exclude.Contains(h)))
        .Select(h=>new ACol<Solid>(f.ISweep(h.Collider,-step),(Solid)h)),
        maxt);
    }
    List<Solid> current;
    public void proc(float step){
      prog(step);
    }
  }
  public static CoarseCollisionRast rast = new();

  public TemplateMoveCollidable(string templateStr, Vector2 pos, int depthoffset, string idp):base(templateStr,pos,depthoffset,idp){
  }
  List<Solid> solids;
  List<Vector2> nodes;
  public override void addTo(Scene scene){
    base.addTo(scene);
  }
}