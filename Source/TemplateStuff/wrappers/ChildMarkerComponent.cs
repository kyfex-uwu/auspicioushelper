



using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class ChildMarkerComponent:Component{

  public Template parent;
  public ChildMarkerComponent(Template parent):base(true, false){
    this.parent = parent;
  }
}