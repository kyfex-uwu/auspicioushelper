


using System;
using MonoMod.ModInterop;

namespace Celeste.Mod.auspicioushelper.iop;

[ModImportName("auspicioushelper.templates")]
public static class TemplateIop{
  public static class EntityParseTypes{
    public const int unable = 0; //will not include this entity in templates
    public const int platformbasic = 1; //basic platform; use moveV/moveH when moving
    public const int unwrapped = 2; //use this entity directly; do not put into tree
    public const int basic = 3; //basic entity; movement done via position change
    public const int removeSMbasic = 4; //basic but will remove all staticmovers on construction (use for conventionally attached items)
  }
  public static Action<string, int, Level.EntityLoader> clarify;
}

[ModExportName("auspicioushelper.templates")]
public static class TemplateIopExp{
  static EntityParser.Types getType(int t){
    return t switch{
      0=>EntityParser.Types.unable,
      1=>EntityParser.Types.platformbasic,
      2=>EntityParser.Types.unwrapped,
      3=>EntityParser.Types.basic,
      4=>EntityParser.Types.removeSMbasic,
      _=>EntityParser.Types.unable,
    };
  }
  public static void clarify(string name, int type, Level.EntityLoader loader){
    EntityParser.clarify(name, getType(type), loader);
  }
}