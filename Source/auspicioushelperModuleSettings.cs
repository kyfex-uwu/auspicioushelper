

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
  // private Dictionary<int, string> hideRulesDict {get; set;} = new();
  // [SettingIgnore]
  // public HideruleMenu hideruleMenu {get;set;} = new();
  // [SettingSubMenu]
  // public class HideruleMenu{
  //   [SettingIgnore]
  //   public bool Dummy {get;set;}
  //   public Dictionary<int,TextMenu.Button> items = new();
  //   public void CreateDummyEntry(TextMenuExt.SubMenu menu, bool ingame){
  //     Dictionary<int, string> rules = auspicioushelperModule.Settings.hideRulesDict;
  //     foreach((int i, string s) in rules){
  //       TextMenu.Button entry = new(i.ToString(), )
  //       entry.Change()
  //     }
  //   }
  // }
}