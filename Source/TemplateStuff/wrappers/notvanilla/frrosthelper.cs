

using Microsoft.Xna.Framework;
using Mono.Cecil.Mdb;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.auspicioushelper.Wrappers;

public static class FrostHelperStuff{
  public class Staticbumperwrapper:Entity, ISimpleWrapper{
    public Template parent {get;set;}
    public Entity wrapped {get;}
    public Vector2 toffset {get;set;}
    Vector2 twoffset = Vector2.Zero;
    DynamicData d;
    public Staticbumperwrapper(Entity e, EntityData dat){
      Tween tw = e.Get<Tween>();
      d=new DynamicData(e);
      wrapped = e;
      if(tw == null) return;
      Vector2 delta = dat.Nodes[0]-dat.Position;
      tw.OnUpdate = (Tween t)=>{
        if(d.Get<bool>("goBack")){
          twoffset = Vector2.Lerp(delta,Vector2.Zero,t.Eased);
        } else {
          twoffset = Vector2.Lerp(Vector2.Zero,delta,t.Eased);
        }
        d.Set("anchor",parent.virtLoc+toffset+twoffset);
      };
    }
    public void relposTo(Vector2 loc, Vector2 liftspeed){
      d.Set("anchor",loc+toffset+twoffset);
    }
    public static HookManager clarify = new(()=>{
      Level.EntityLoader orig = EntityParser.getLoader("FrostHelper/StaticBumper");
      EntityParser.clarify("FrostHelper/StaticBumper", EntityParser.Types.unwrapped, (Level l, LevelData d, Vector2 o, EntityData e)=>{
        Entity s = orig(l,d,o,e);
        DebugConsole.Write($"Adding wrapped bumper! {s}");
        return new Staticbumperwrapper(s,e);
      });
    },()=>{});
  }
}