


using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;
public static class MarkedRoomParser{

  public static Dictionary<string, templateFiller> templates = new Dictionary<string, templateFiller>();
  public static string sigstr = "AHtroom";
  public static void parseLeveldata(LevelData l, string prefix){
    var rects = new StaticCollisiontree();
    var handleDict = new Dictionary<int, string>();
    foreach(EntityData d in l.Entities){
      if(d.Name == "auspicioushelper/templateFiller"){
        templateFiller t = new templateFiller(d, Vector2.Zero); //we are in frame of room <3
        string id = prefix+t.name;
        if(templates.ContainsKey(id)){
          DebugConsole.Write("Multiple templates with the same identifier "+id);
          continue;
        }
        int handle = rects.add(new FloatRect(t));
        handleDict.Add(handle, id);
        templates.Add(id,t);
        DebugConsole.Write(id);
        t.setTiles(l.Solids,l.Bg);
      }
    }

  }
  public static void parseMapdata(MapData m){
    templates.Clear();
    foreach(LevelData l in m.Levels){
      if(l.Name.StartsWith(sigstr+"-")||l.Name == sigstr){
        DebugConsole.Write("Parsing "+l.Name);
        string prefix = l.Name == sigstr?"":l.Name.Substring(sigstr.Length+1)+"/";
        parseLeveldata(l, prefix);

      }
    }
  }
}