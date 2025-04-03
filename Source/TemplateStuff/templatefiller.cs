


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/templateFiller")]
public class templateFiller:Entity{
  
  public string name;
  Rectangle tr;
  // char[,] fgtiles;
  // char[,] bgtiles;
  public VirtualMap<char> fgt;
  public VirtualMap<char> bgt;
  public Vector2 offset;
  public Vector2 origin=>-offset+Position;
  public List<DecalData> decals = new List<DecalData>();
  public List<EntityParser.EWrap> childEntities = new List<EntityParser.EWrap>();
  public templateFiller(EntityData d, Vector2 offset):base(d.Position){
    this.Collider = new Hitbox(d.Width, d.Height);
    name = d.Attr("template_name","");
    tr = new Rectangle((int)d.Position.X/8, (int)d.Position.Y/8, d.Width/8,d.Height/8);
    this.offset = d.Nodes.Count()>0?d.Position-d.Nodes[0]:Vector2.Zero; 
  }
  public override void Awake(Scene scene){
    RemoveSelf();  
  }
  // public AnimatedTiles bgaTiles = null;
  // public TileGrid bgsTiles = null;
  // public AnimatedTiles fgaTiles = null;
  // public TileGrid fgsTiles = null;

  public void setTiles(string fg, string bg){
    Regex regex = new Regex("\\r\\n|\\n\\r|\\n|\\r");
    char[,] fgtiles = new char[tr.Width,tr.Height];
    char[,] bgtiles = new char[tr.Width,tr.Height];
    bool keepfg = false;
    bool keepbg = false;
    string[] fglines= regex.Split(fg);
    string[] bglines= regex.Split(bg);
    for(int i=0; i<tr.Height; i++){
      for(int j=0; j<tr.Width; j++){
        int r = i+tr.Top;
        int c = j+tr.Left;
        if(r<fglines.Length && c<fglines[r].Length){
          fgtiles[j,i]=fglines[r][c];
          keepfg |= fglines[r][c]!='0';
        } else {
          fgtiles[j,i]='0';
        }
        if(r<bglines.Length && c<bglines[r].Length){
          bgtiles[j,i]=bglines[r][c];
          keepbg |= bglines[r][c]!='0';
        } else {
          bgtiles[j,i]='0';
        }
      }
    }

    // Autotiler.Behaviour b = new Autotiler.Behaviour{
    //   EdgesIgnoreOutOfLevel = true,
    //   PaddingIgnoreOutOfLevel = true,
    //   EdgesExtend = false,
    // };
    fgt = keepfg? new VirtualMap<char>(fgtiles):null;
    bgt = keepbg? new VirtualMap<char>(bgtiles):null;
  }
}