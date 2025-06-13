
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Xml;

namespace Celeste.Mod.auspicioushelper;
public static class auspicioushelperGFX {
  public static SpriteBank spriteBank {get; set;}
  public static Dictionary<string, Effect> effects = new Dictionary<string, Effect>();

  public static IGraphicsDeviceService graphicsDeviceService;

  //from ShaderHelper
  public static VirtualShader LoadShader(string path)=>LoadExternShader("auspicioushelper/"+path);
  public static VirtualShader LoadExternShader(string path){
    //probably not a great method of doing this whatsoever
    //-well I don't even know what this is doing <3
    if (graphicsDeviceService == null)  
      graphicsDeviceService = Engine.Instance.Content.ServiceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
    ModAsset asset = Everest.Content.Get("Effects/"+path+".cso",true);
    if (asset == null){
      DebugConsole.Write("Failed to fetch shader at Effects/"+path);
      return null;
    }
    try{
      Effect returnV = new Effect(graphicsDeviceService.GraphicsDevice, asset.Data);
      Effect returnQ = null;
      try {
        ModAsset qasset = Everest.Content.Get("Effects/"+path+"_quiet.cso",true);
        if(qasset!=null) returnQ = new Effect(graphicsDeviceService.GraphicsDevice, qasset.Data);
      }catch(Exception err2){
        DebugConsole.Write("Failed to load quiet shader Effects/"+path+"_quiet with exception "+ err2.ToString());
      }
      return new VirtualShader(returnV,returnQ);
    }catch(Exception err){
      DebugConsole.Write("Failed to load shader Effects/"+path+" with exception "+ err.ToString());
    }
    return null;
  }
  public static void loadContent(){
    spriteBank = new SpriteBank(GFX.Game, "Graphics/auspicioushelper/Sprites.xml");
  }
}