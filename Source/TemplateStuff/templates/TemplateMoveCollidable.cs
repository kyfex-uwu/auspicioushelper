


// using System.Collections.Generic;
// using System.Linq;
// using Microsoft.Xna.Framework;
// using Monocle;

// namespace Celeste.Mod.auspicioushelper;

// public class TemplateMoveCollidable:TemplateDisappearer{ 
//   public override Vector2 gatheredLiftspeed => ownLiftspeed;
//   Vector2 movementCounter;
//   public TemplateMoveCollidable(EntityData data, Vector2 pos, int depthoffset, string idp):base(data,pos,depthoffset){
//     nodes = new(data.Nodes);
//     movementCounter = Vector2.Zero;
//   }
//   List<Solid> solids;
//   List<Vector2> nodes;
//   public override void addTo(Scene scene){
//     base.addTo(scene);
//     solids = GetChildren<Solid>();
//   }
//   public override void relposTo(Vector2 loc, Vector2 liftspeed) {}
//   class QueryBounds {
//     public List<FloatRect> rects=new();
//     public List<SolidTiles> grids=new();
//   }
//   QueryBounds getQinfo(){
//     QueryBounds res  =new();
//     foreach(Solid s in solids){
//       if(!s.Collidable) continue;
//       if(s.Collider is Hitbox h) res.hitboxSolids
//     }
//   }
//   int QueryH(int amount, QueryBounds q){

//     return 0;
//   }
//   int QueryV
// }