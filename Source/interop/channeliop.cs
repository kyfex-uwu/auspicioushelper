


using System;
using System.Collections.Generic;
using Celeste.Mod.auspicioushelper;
using Monocle;
using MonoMod.ModInterop;

namespace Celeste.Mod.auspicioushelper.iop;

[ModImportName("auspicioushelper.channels")]
public static class ChannelIop{
  public static Func<string,int> readChannel;
  public static Action<string, int> setChannel;
  public static Func<string, Action<int>, Component> getWatcher;
  public static Func<Component, int> watcherValue;
  public static Action<string, Func<List<string>,List<int>,int>> registerIopFunc;
  public static Action<string, Func<List<string>,List<int>,int>>deregisterIopFunc;
}


[ModExportName("auspicioushelper.channels")]
public static class ChannelIopExp{
  public static int readChannel(string ch){
    return ChannelState.readChannel(ch);
  }
  public static void setChannel(string ch, int val){
    ChannelState.SetChannel(ch,val);
  }
  public static Component getWatcher(string ch, Action<int> onChannelChange){
    return new ChannelTracker(ch, onChannelChange);
  }
  public static int watcherValue(Component w){
    if(w is ChannelTracker ct) return ct.value;
    return 0;
  }
  public static void registerIopFunc(string identifier, Func<List<string>,List<int>,int> fn){
    ChannelMathController.registerInterop(identifier,fn);
  }
  public static void deregisterIopFunc(string identifier, Func<List<string>,List<int>,int> fn){
    ChannelMathController.deregisterInterop(identifier, fn);
  }
}

