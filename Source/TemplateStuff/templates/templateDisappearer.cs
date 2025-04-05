



using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class TemplateDisappearer:Template{

  bool selfVis = true;
  bool selfCol = true;
  bool parentVis = true;
  bool parentCol = true;
  public TemplateDisappearer(string templateStr, Vector2 pos, int depthoffset):base(templateStr,pos,depthoffset){
  }
  int permute(bool action, ref bool activator, bool other){
    if(action == activator){
      DebugConsole.Write("THIS SHOULD NOT HAPPEN - CODE IS BORKED");
      return 0;
    }
    activator = action;
    if(other == false) return 0;
    return action?1:-1;
  }
  public override void parentChangeStat(int vis, int col){
    int nvis = 0; int ncol=0;
    if(vis!=0) nvis = permute(vis>0, ref parentVis, selfVis);
    if(col!=0) ncol = permute(col>0, ref parentCol, selfCol);
    if(nvis!=0 || ncol!=0){
      base.parentChangeStat(nvis,ncol);
    }
  }
  public virtual void setCollidability(bool n){
    if(n == selfCol) return;
    int ncol = permute(n, ref selfCol, parentCol);
    if(ncol != 0) base.parentChangeStat(0,ncol);
  }
  public virtual void setVisibility(bool n){
    if(n == selfVis) return;
    int nvis = permute(n, ref selfVis, parentVis);
    if(nvis != 0) base.parentChangeStat(nvis,0);
  }
  public virtual void setVisCol(bool vis, bool col){
    int nvis = 0;
    if(vis != selfVis) nvis = permute(vis, ref selfVis, parentVis);
    int ncol = 0;
    if(col != selfCol) ncol = permute(col, ref selfCol, parentCol);
    //DebugConsole.Write($" set {vis} {col} {selfVis}");
    if(nvis!=0 || ncol!=0) base.parentChangeStat(nvis,ncol);
  }
}