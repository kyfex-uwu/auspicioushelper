



using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class FoundEntity:Component{
  int id;
  EntityData d;
  public FoundEntity(int target, EntityData d):base(false,false){
    id=target; this.d=d;
  }
  public void finalize(Entity e){
    if(e==null){
      DebugConsole.Write($"Failed to find the entity {d.Name} with id {id} - (maybe this entity adds itself non-standardly?)");
      return;
    }
    DebugConsole.Write($"Found the entity {d.Name} with id {id}");
    e.Add(this);
  }
}