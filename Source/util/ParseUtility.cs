


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
      rgba= (rgba<<shift)+mask;
    }
    return new Color(
      (float)((rgba>>(shift*3))&mask)*mult, 
      (float)((rgba>>(shift*2))&mask)*mult, 
      (float)((rgba>>shift)&mask)*mult, 
      (float)(rgba&mask)*mult);
  }

  public static int bsearchLast(float[] arr, float val){
    int left = 0; 
    int right = arr.Length;
    while(right-left>1){
      int middle = (left+right)/2;
      if(arr[middle]>val){
        right = middle;
      } else {
        left = middle;
      }
    }
    return left;
  }
  public static int bsearchFirst(float[] arr, float val){
    int left = -1; 
    int right = arr.Length-1;
    while(right-left>1){
      int middle = (left+right+1)/2;
      if(arr[middle]>=val){
        right = middle;
      } else {
        left = middle;
      }
    }
    return right;
  }
  public static float remap(float t, float low, float high){
    t=t-low;
    high = high-low;
    return t/high;
  }
}