

using System;
using System.Collections.Generic;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.auspicioushelper;

public class auspicioushelperModuleSettings : EverestModuleSettings {
  private bool _usingDebugConsole = false;
  public bool UseDebugConsole {
    get=>_usingDebugConsole;
    set{
      _usingDebugConsole = value;
      if(value) DebugConsole.Open();
      else DebugConsole.Close();
    }}
}