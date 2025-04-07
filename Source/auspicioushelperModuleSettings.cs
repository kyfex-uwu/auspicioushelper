

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
  private bool _tryQuietShader = false;
  public bool UseQuietShader{
    get=>_tryQuietShader;
    set{
      _tryQuietShader = value;
    }
  }
  private bool _hideHelperMaps = false;
  public bool hideHelperMaps {
    get=>_hideHelperMaps;
    set{
      _hideHelperMaps = value;
      if(value)MapHider.hideListed();
      else MapHider.revealListed();
    }
  }

  private string _hideRules = "0:\"\\/t$\"";
  [SettingMaxLength(600)]
  public string hideRules {
    get=>_hideRules;
    set{
      _hideRules = value;
      MapHider.uncache();
    }
  } 
}