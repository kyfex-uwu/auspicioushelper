


using System.Globalization;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper;
public static class Util{

  public static Color hexToColor(string hex){
    hex = hex.TrimStart('#');
    uint rgba = uint.Parse(hex, NumberStyles.HexNumber);
    int shift = hex.Length>4?8:4;
    uint mask = hex.Length>4?0xffu:0xfu;
    float mult = hex.Length>4?1f/255f:1f/15f;
    if(hex.Length %4 != 0){
      DebugConsole.Write("Padding Alpha");
      rgba= (rgba<<shift)+mask;
    }
    DebugConsole.Write(rgba.ToString());
    DebugConsole.Write((rgba&mask).ToString()+" "+((float)((rgba>>(shift*3))&mask)*mult).ToString());
    return new Color(
      (float)((rgba>>(shift*3))&mask)*mult, 
      (float)((rgba>>(shift*2))&mask)*mult, 
      (float)((rgba>>shift)&mask)*mult, 
      (float)(rgba&mask)*mult);
  }
}