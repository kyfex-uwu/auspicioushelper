


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;
public class MipGrid{
  public class Layer{
    ulong[] d;
    public int width;
    public int height;
    public Layer(List<ulong> data, int width){
      d=data.ToArray();
      this.width=width;
      height = data.Count/width;
      if(data.Count!=width*height) throw new Exception("mystery!");
    }
    public ulong getBlock(int x, int y){
      if(x<0 || y<0 || x>=width || y>=height) return 0;
      return d[x+y*width];
    }
    public ulong getBlockFast(int x, int y){
      return d[x+y*width];
    }
  }
  List<Layer> layers;
  int width;
  int height;
  //ok but we hardcode these everywhere so like don't
  const int blockw = 8;
  const int blockh = 8;
  int highestlevel;
  int gridw;
  int gridh;
  Vector2 cellshape;
  Vector2 tlc;
  Grid g;
  const ulong FULL = 0xffff_ffff_ffff_ffffUL;
  public MipGrid(Grid grid){
    VirtualMap<bool> map = grid.Data;
    List<ulong> r0 = new();
    for(int yb=0; yb<map.Rows; yb+=blockh){
      for(int xb=0; xb<map.Columns; xb+=blockw){
        ulong block = 0;
        int xstop = Math.Min(xb+blockw,map.Columns);
        int ystop = Math.Min(yb+blockh,map.Rows);
        for(int y=yb; y<ystop; y++){
          for(int x=xb; x<xstop; x++){
            if(map[x,y])block |= 1UL<<(x+y*8);
          }
        }
        r0.Add(block);
      }
    }
    layers = [new Layer(r0,(map.Columns+blockw-1)/blockw)];
    buildMips();
  }
  void buildMips(int level=1){
    Layer b = layers[level-1];
    if(layers.Count != level) throw new Exception("only build mips on new grid");
    List<ulong> r = new();
    for(int yb=0; yb<b.height; yb+=blockh){
      for(int xb=0; xb<b.width; xb+=blockw){
        ulong block = 0;
        int xstop = Math.Min(xb+blockw,b.width);
        int ystop = Math.Min(yb+blockh,b.height);
        for(int y=yb; y<ystop; y++){
          for(int x=xb; x<xstop; x++){
            if(b.getBlockFast(x,y)!=0)block |= 1UL<<(x+y*8);
          }
        }
      }
    }
    Layer nl;
    layers.Add(nl = new Layer(r, b.width/blockw));
    highestlevel = level;
    if(Math.Max(nl.width/blockw,nl.height/blockh)>1) buildMips(level+1);
  }
  ulong makeRectMask(int blockx, int blocky, Vector2 otlc, Vector2 obrc, int level){
    int levelDiv = 1<<(level*3);
    Vector2 coordoffset = new Vector2(blockx*blockw,blocky*blockh);
    Vector2 rtlc = (otlc/levelDiv-coordoffset).Floor();
    Vector2 rbrc = (obrc/levelDiv-coordoffset).Ceiling();
    if(rbrc.X<0 || rbrc.Y<0 || rtlc.X>=blockw || rtlc.Y>=blockh) return 0;
    int x1 = Math.Clamp((int) rtlc.X,0,blockw-1);
    int y1 = Math.Clamp((int) rtlc.Y,0,blockh-1);
    int x2 = Math.Clamp((int) rbrc.X,0,blockw-1);
    int y2 = Math.Clamp((int) rbrc.Y,0,blockh-1);
    if(x2<x1 || y2<y1) return 0;
    byte row = (byte)(((1<<(x2-x1+1))-1)<<x1);
    int rows = y2-y1+1;
    var mask = rows==8? FULL: ((1UL << (rows*8))-1) << (y1*8);
    return mask & (0x0101_0101_0101_0101UL*row);
  }
  bool collideFrLevel(int x, int y, Vector2 otlc, Vector2 obrc, int level){
    ulong dat = layers[level].getBlock(x,y);
    ulong mask = makeRectMask(x,y,otlc,obrc,level);
    if(dat == 0 || mask == 0) return false;
    if(mask == FULL) return true;
    if(dat == FULL && level == 0) return true;
    ulong hit = dat&mask;
    if(level == 0) return hit!=0;
    while(hit != 0){
      int index = System.Numerics.BitOperations.TrailingZeroCount(hit);
      if(collideFrLevel(x*blockw+index%8,y*blockh+index/8,otlc,obrc,level-1)) return true;
      hit &= hit-1;
    }
    return false;
  }
  bool collideFr(FloatRect f){
    tlc = g.AbsolutePosition.Round();
    int mld = (int)Math.Ceiling(Math.Max(f.x/cellshape.X,f.y/cellshape.Y));
    int level = 0;
    while(level<highestlevel && (1<<(3*(level+1)))<=mld)level++;
    int levelDiv = 1<<(3*level);
    Vector2 rtlc = Vector2.Max(((f.tlc-tlc)/cellshape).Floor(), new Vector2(0,0));
    Vector2 rbrc = Vector2.Min(((f.brc-tlc)/cellshape).Ceiling(), new Vector2(width,height));
    if(rbrc.X<0 || rbrc.Y<0 || rtlc.X>width || rtlc.Y>height) return false;
    int xstop = Math.Min((int)Math.Ceiling(rbrc.X/levelDiv),layers[level].width);
    int ystop = Math.Min((int)Math.Ceiling(rbrc.Y/levelDiv),layers[level].height);
    for(int x=(int)Math.Floor(rtlc.X/levelDiv); x<xstop; x++){
      for(int y=(int)Math.Floor(rtlc.Y/levelDiv); y<ystop; y++){
        if(collideFrLevel(x,y,rtlc,rbrc,level)) return true;
      }
    }
    return false;
  }
}