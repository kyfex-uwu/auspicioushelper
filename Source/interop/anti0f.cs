


using System;
using MonoMod.ModInterop;

namespace Celeste.Mod.auspicioushelper.iop;

[ModExportName("auspicioushelper.anti0f")]
public static class Anti0fIopExp{
  /**
  Anti0f zones normally work by overriding the default speed-based movement in Player.orig_update.
  Some states end up skipping this type of movement (such as dream dashes). If you do not want the
  player to move from the default update step, add a NormalSkipCond.

  Similarly, some states use Player.NaiveMove for their movement (such as dreamdash). Add a 
  naiveRunCond to tell the Anti0f zones to intercept naive movement through them when in your state.

  Exitconds tell the anti0f to stop moving the player at that position when true. For example, we
  want to exit if the player has gained a dash from a refill crystal. To add custom conditions for
  stopping, add a naive or normal skipCond. Note that if your skipCond is only meant to happen in
  a certain state, you must ensure you are in that state. (i.e. any exitcond returing true will
  result in the player halting their movement).

  You should remove your functions when your mod unloads (don't be rude!). Check in 
  util/import/CommunalHelper for an example of making anti0f zones work with the dream tunnel state.
  */
  public static void AddNormalSkipCond(Func<Player, bool> fn) =>Anti0fZone.skipNormal.Add(fn);
  public static void AddNormalExitCond(Func<Player, bool> fn) =>Anti0fZone.exitNormal.Add(fn);
  public static void AddNaiveRunCond(Func<Player, bool> fn) =>Anti0fZone.runNaive.Add(fn);
  public static void AddNaiveExitCond(Func<Player, bool> fn) =>Anti0fZone.exitNaive.Add(fn);
  public static void RemoveNormalSkipCond(Func<Player, bool> fn) =>Anti0fZone.skipNormal.Remove(fn);
  public static void RemoveNormalExitCond(Func<Player, bool> fn) =>Anti0fZone.exitNormal.Remove(fn);
  public static void RemoveNaiveRunCond(Func<Player, bool> fn) =>Anti0fZone.runNaive.Remove(fn);
  public static void RemoveNaiveExitCond(Func<Player, bool> fn) =>Anti0fZone.exitNaive.Remove(fn);
}

//paste this into your mod to use the functions above
[ModImportName("auspicioushelper.anti0f")]
public static class Anti0fIop{
  public static Action<Func<Player, bool>> AddNormalSkipCond;
  public static Action<Func<Player, bool>> AddNormalExitCond;
  public static Action<Func<Player, bool>> AddNaiveRunCond;
  public static Action<Func<Player, bool>> AddNaiveExitCond;
  public static Action<Func<Player, bool>> RemoveNormalSkipCond;
  public static Action<Func<Player, bool>> RemoveNormalExitCond;
  public static Action<Func<Player, bool>> RemoveNaiveRunCond;
  public static Action<Func<Player, bool>> RemoveNaiveExitCond;
}
