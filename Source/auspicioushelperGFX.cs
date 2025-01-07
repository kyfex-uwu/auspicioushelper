
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Celeste.Mod.auspicioushelper;
public static class auspicioushelperGFX {
  public static SpriteBank spriteBank {get; set;}
  public static Dictionary<string, Effect> effects = new Dictionary<string, Effect>();

  public static IGraphicsDeviceService graphicsDeviceService;

  //from ShaderHelper
  public static Effect LoadEffect(string path){
    //probably not a great method of doing this whatsoever
    //-well I don't even know what this is doing <3
    if (graphicsDeviceService == null)  
      graphicsDeviceService = Engine.Instance.Content.ServiceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
    ModAsset asset = Everest.Content.Get("Effects/auspicioushelper/StaticEffect.cso",true);
    if (asset == null){
      DebugConsole.Write("Failed to fetch shader at "+path);
      return null;
    }
    try{
      Effect returnV = new Effect(graphicsDeviceService.GraphicsDevice, asset.Data);
      return returnV;
    }catch(Exception err){
      DebugConsole.Write("Failed to load shader "+path+" with exception "+ err.ToString());
    }
    return null;
  }
  public static void loadContent(){
    spriteBank = new SpriteBank(GFX.Game, "Graphics/auspicioushelper/Sprites.xml");
    MaterialPipe.addLayer(ChannelBaseEntity.layerA = new ChannelMaterialsA());
    MaterialPipe.registerPipe();
  }
}