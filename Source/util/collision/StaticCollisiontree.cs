


using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper;

//well it's not actually a tree now but whenever I want to actually accelerate it I have the interface atleast
public class StaticCollisiontree{
  List<FloatRect> rects = new List<FloatRect>();
  bool built = false;
  public StaticCollisiontree(){

  }
  public int add(FloatRect r){
    int index = rects.Count;
    rects.Add(r);
    return index;
  }
  public void build(){}
  public List<int> collidePointAll(Vector2 x){
    if(!built){
      List<int> l = new List<int>();
      for(int i=0; i<rects.Count; i++){
        if(rects[i].CollidePoint(x)) l.Add(i);
      }
      return l;
    }
    return null; //not yet implemented
  }
}