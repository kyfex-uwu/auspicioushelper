


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Celeste.Editor;
using Celeste.Mod.Entities;
using Celeste.Mod.auspicioushelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/ChannelMathController")]
public class ChannelMathController:Entity{
  byte[] op = null;
  int[] reg;
  enum Op:byte{
    noop, loadZero, loadI, loadImmediateInt, loadChannel, storeChannel, copy,
    startAccInit0, startAccInit1, startAccInitImm, startAccInitReg, startAcc, finishAcc,
    mult, div, mod, add, sub, lshift, rshift, and, or, xor, land, lor, max, min, take,
    multI, divI, modI, addI, subI, lshiftI, rshiftI, andI, orI, xorI, landI, lorI, maxI, minI, takeI,
    eq,ne,le,ge,less,greater, eqI,neI,leI,geI,lessI,greaterI, not, lnot,
    jnz, jz, j, setsptr, setsptrI, loadsptr, iops, iopsi, iopsii, iopss, iopssi, iopssii, iopvsvi
  }
  List<string> usedChannels = new List<string>();
  bool runImmediately;
  bool needsrun=false;
  bool debug=false;
  int period=0;
  int periodTimer=0;
  public ChannelMathController(EntityData d, Vector2 offset):base(new Vector2(0,0)){
    runImmediately = d.Bool("run_immediately",false);
    int p = d.Int("custom_polling_rate",0);
    if(d.Bool("every_frame")) p=1;
    if(p>0){
      period = p;
      periodTimer = 1;
    }
    debug = d.Bool("debug",false);
    var bin=Convert.FromBase64String(d.Attr("compiled_operations",""));
    if(bin.Length<2){
      DebugConsole.Write("Invalid instructions - too short");
      return;
    }
    int version = BitConverter.ToUInt16(bin);
    if(version!=1){
      DebugConsole.Write("Invalid version for mathcontroller");
      return;
    }
    int numUsing = BitConverter.ToUInt16(bin, 2);
    int numReg = BitConverter.ToUInt16(bin,4);
    int opsOffset = BitConverter.ToUInt16(bin,6);
    uint opsLength = BitConverter.ToUInt32(bin,8);
    int coffset = 12;
    for(int i=0; i<numUsing; i++){
      int len = bin[coffset++];
      usedChannels.Add(Encoding.ASCII.GetString(bin,coffset,len));
      coffset+=len;
    }
    reg = new int[numReg];
    op = new byte[opsLength];
    Array.Copy(bin, opsOffset, op, 0, opsLength);
    needsrun = true;
  }
  public override void Added(Scene scene){
    base.Added(scene);
    for(int i=0; i<usedChannels.Count; i++){
      int ridx=i;
      string ch = usedChannels[i];
      reg[i]=ChannelState.readChannel(ch);
      Add(new ChannelTracker(ch,(val)=>changeReg(ridx,val)));
      if(debug) DebugConsole.Write("watching channel "+ch.ToString()+" on register "+i.ToString());
    }
  }
  private void changeReg(int ridx, int nval){
    if(debug) DebugConsole.Write("register "+ridx.ToString()+" set to "+nval.ToString());
    reg[ridx]=nval;
    if(runImmediately) run8bitsimple();
    else needsrun=true;
  }
  public override void Update(){
    base.Update();
    if((period != 0 && --periodTimer<=0) || (period == 0 && !runImmediately && needsrun)){
      run8bitsimple();
      needsrun = false;
      periodTimer = period;
    }
  }
  public void run8bitsimple(){
    int iptr = 0;
    int ridx = 0;
    int len = 0;
    int acc=0;
    int sptr=0;
    string channel = "";
    while(iptr<op.Length){
      switch((Op)op[iptr++]){
        case Op.loadZero: reg[op[iptr++]]=0; break;
        case Op.loadI: reg[op[iptr++]]=(sbyte)op[iptr++];break;
        case Op.loadImmediateInt: reg[op[iptr++]]=BitConverter.ToInt32(op,iptr);iptr+=4;break;
        case Op.loadChannel:
          ridx=op[iptr++];
          len=op[iptr++];
          channel = Encoding.ASCII.GetString(op, iptr, len);
          reg[ridx]=ChannelState.readChannel(channel);
          iptr+=len;
          break;
        case Op.storeChannel:
          ridx=op[iptr++];
          len=op[iptr++];
          channel = Encoding.ASCII.GetString(op, iptr, len);
          if(debug) DebugConsole.Write("setting channel: "+channel+" "+reg[ridx].ToString());
          ChannelState.SetChannel(channel, reg[ridx]);
          iptr+=len;
          break;
        case Op.copy:
          reg[op[iptr++]]=reg[op[iptr++]];break;
        case Op.startAccInit0:
          acc=0;
          goto case Op.startAcc;
        case Op.startAccInit1:
          acc=1;
          goto case Op.startAcc;
        case Op.startAccInitReg:
          acc=reg[op[iptr++]];
          goto case Op.startAcc;
        case Op.startAccInitImm:
          acc=op[iptr++];
          goto case Op.startAcc;
        case Op.startAcc:
          while((Op)op[iptr]!=Op.finishAcc){
            switch((Op) op[iptr++]){
              case Op.mult: acc*=reg[op[iptr]];break;
              case Op.div: acc/=reg[op[iptr]];break;
              case Op.mod: acc%=reg[op[iptr]];break;
              case Op.add: acc+=reg[op[iptr]];break;
              case Op.sub: acc-=reg[op[iptr]];break;
              case Op.lshift: acc<<=reg[op[iptr]];break;
              case Op.rshift: acc>>=reg[op[iptr]];break;
              case Op.and: acc&=reg[op[iptr]];break;
              case Op.or: acc|=reg[op[iptr]];break;
              case Op.xor: acc^=reg[op[iptr]];break;
              case Op.land: acc=acc!=0?reg[op[iptr]]:0;break;
              case Op.lor: acc=acc==0?reg[op[iptr]]:acc;break;
              case Op.max: acc=Math.Max(acc, reg[op[iptr]]);break;
              case Op.min: acc=Math.Min(acc, reg[op[iptr]]);break;
              case Op.take: if(--sptr==0) acc=reg[op[iptr]];break;

              case Op.multI: acc*=(sbyte)op[iptr];break;
              case Op.divI: acc/=(sbyte)op[iptr];break;
              case Op.modI: acc%=(sbyte)op[iptr];break;
              case Op.addI: acc+=(sbyte)op[iptr];break;
              case Op.subI: acc-=(sbyte)op[iptr];break;
              case Op.lshiftI: acc<<=(sbyte)op[iptr];break;
              case Op.rshiftI: acc>>=(sbyte)op[iptr];break;
              case Op.andI: acc&=(int)(sbyte)op[iptr];break;
              case Op.orI: acc|=(int)(sbyte)op[iptr];break;
              case Op.xorI: acc^=(int)(sbyte)op[iptr];break;
              case Op.landI: acc=acc!=0?(sbyte)op[iptr]:0;break;
              case Op.lorI: acc=acc==0?(sbyte)op[iptr]:acc;break;
              case Op.maxI: acc=Math.Max(acc, (sbyte)op[iptr]);break;
              case Op.minI: acc=Math.Min(acc, (sbyte)op[iptr]);break;
              case Op.takeI: if(--sptr==0) acc=(sbyte)op[iptr];break;
              default:break;
            }
            iptr++;
          }iptr++;
          reg[op[iptr++]]=acc;
          break;
        case Op.eq:reg[op[iptr++]]= (reg[op[iptr++]]==reg[op[iptr++]])?1:0; break;
        case Op.ne:reg[op[iptr++]]= (reg[op[iptr++]]!=reg[op[iptr++]])?1:0; break;
        case Op.le:reg[op[iptr++]]= (reg[op[iptr++]]<=reg[op[iptr++]])?1:0; break;
        case Op.ge:reg[op[iptr++]]= (reg[op[iptr++]]>=reg[op[iptr++]])?1:0; break;
        case Op.less:reg[op[iptr++]]= (reg[op[iptr++]]<reg[op[iptr++]])?1:0; break;
        case Op.greater:reg[op[iptr++]]= (reg[op[iptr++]]>reg[op[iptr++]])?1:0; break;
        case Op.eqI:reg[op[iptr++]]= (reg[op[iptr++]]==(sbyte)op[iptr++])?1:0; break;
        case Op.neI:reg[op[iptr++]]= (reg[op[iptr++]]!=(sbyte)op[iptr++])?1:0; break;
        case Op.leI:reg[op[iptr++]]= (reg[op[iptr++]]<=(sbyte)op[iptr++])?1:0; break;
        case Op.geI:reg[op[iptr++]]= (reg[op[iptr++]]>=(sbyte)op[iptr++])?1:0; break;
        case Op.lessI:reg[op[iptr++]]= (reg[op[iptr++]]<(sbyte)op[iptr++])?1:0; break;
        case Op.greaterI:reg[op[iptr++]]= (reg[op[iptr++]]>(sbyte)op[iptr++])?1:0; break;

        case Op.not: reg[op[iptr++]]=~reg[op[iptr++]]; break;
        case Op.lnot: reg[op[iptr++]]=reg[op[iptr++]]!=0?0:1; break;

        case Op.setsptr: sptr = reg[op[iptr++]]; break;
        case Op.setsptrI: sptr = op[iptr++]; break;
        case Op.loadsptr: reg[op[iptr++]]=sptr; break; 

        case Op.iops: reg[op[iptr++]] = interop(1,0,ref iptr); break;
        case Op.iopsi: reg[op[iptr++]] = interop(1,1,ref iptr); break;
        case Op.iopsii: reg[op[iptr++]] = interop(1,2,ref iptr); break;
        case Op.iopss: reg[op[iptr++]] = interop(2,0,ref iptr); break;
        case Op.iopssi: reg[op[iptr++]] = interop(2,1,ref iptr); break;
        case Op.iopssii: reg[op[iptr++]] = interop(2,2,ref iptr); break;
        case Op.iopvsvi: reg[op[iptr++]] = interop(op[iptr++],op[iptr++],ref iptr); break;

        case Op.jnz:
          if(reg[op[iptr++]]!=0) goto case Op.j;
          iptr+=4; break;
        case Op.jz:
          if(reg[op[iptr++]]==0) goto case Op.j;
          iptr+=4; break;
        case Op.j:
          iptr = BitConverter.ToInt32(op,iptr);
          break;
        
        default: break;
      }
    }
  }
  static Dictionary<string, Func<List<string>,List<int>,int>> iopFuncs = new();
  public int interop(int stringCount, int intCount, ref int iptr){
    List<string> strs = new List<string>();
    List<int> ints = new List<int>();
    for(int i=0; i<stringCount; i++){
      int len = op[iptr++];
      strs.Add(Encoding.ASCII.GetString(op, iptr, len));
      iptr+=len;
    }
    for(int i=0; i<intCount; i++){
      ints.Add(reg[op[iptr++]]);
    }
    if(!iopFuncs.TryGetValue(strs[0], out var f)){
      DebugConsole.Write($"Interop function {strs[0]} not yet registered");
      return 0;
    }
    try{
      return f(strs, ints);
    }catch(Exception ex){
      DebugConsole.Write($"Mathcontroller interop {strs[0]} failed {ex}");
      return 0;
    }
  }
  public static void registerInterop(string identifier, Func<List<string>,List<int>,int> function){
    if(!iopFuncs.TryAdd(identifier,function)){
      DebugConsole.Write($"Interop registration collision at {identifier}");
    }
  }
  public static void deregisterInterop(string identifier, Func<List<string>,List<int>,int> function){
    if(!iopFuncs.TryGetValue(identifier, out var f)){
      DebugConsole.Write($"No registered interop function at {identifier}");
      return;
    }
    if(f != function){
      DebugConsole.Write($"Provided function does not match interop function at {identifier}");
      return;
    }
    iopFuncs.Remove(identifier);
  }
  public static int toInt(object o){
    try {
      return Convert.ToInt32(o);
    } catch(Exception){
      return o==null?0:1;
    }
  }
  public static void setupDefaultInterop(){
    registerInterop("print",(List<string> strs, List<int> ints)=>{
      string str = "From mathcontroller: ";
      for(int i=0; i<strs.Count; i++) str+=strs[i]+" ";
      for(int i=0; i<ints.Count; i++) str+=ints[i].ToString()+" ";
      DebugConsole.Write(str);
      return 0;
    });
    registerInterop("hasBerry",(List<string> strs, List<int> ints)=>{
      return SaveData.Instance.CheckStrawberry(new EntityID(strs[1],ints[0]))?1:0;
    });
    registerInterop("getFlag",(List<string> strs, List<int> ints)=>{
      if(Engine.Scene is Level l) return l.Session.GetFlag(strs[1])?1:0;
      return 0;
    });
    registerInterop("setFlag",(List<string> strs, List<int> ints)=>{
      if(Engine.Scene is Level l) l.Session.SetFlag(strs[1], ints[0]!=0);
      return ints[0];
    });
    registerInterop("getCounter",(List<string> strs, List<int> ints)=>{
      if(Engine.Scene is Level l) return l.Session.GetCounter(strs[0]);
      return 0;
    });
    registerInterop("setCounter",(List<string> strs, List<int> ints)=>{
      if(Engine.Scene is Level l) l.Session.SetCounter(strs[1], ints[0]);
      return ints[0];
    });
    registerInterop("getCoreMode",(List<string> strs, List<int> ints)=>{
      if(Engine.Scene is Level l) return l.Session.CoreMode == Session.CoreModes.Cold?1:0;
      return 0;
    });
    registerInterop("setCoreMode",(List<string> strs, List<int> ints)=>{
      if(Engine.Scene is Level l) l.Session.CoreMode = ints[0]==0?Session.CoreModes.Hot:Session.CoreModes.Cold;
      return 0;
    });
    registerInterop("getPlayer",(List<string> strs, List<int> ints)=>{
      Player p = Engine.Scene.Tracker.GetEntity<Player>();
      if(p==null) return 0;
      switch(strs[1]){
        case "speedx": return (int)p.Speed.X;
        case "speedy": return (int)p.Speed.Y;
        case "posx": return (int)p.Position.X;
        case "posy": return (int)p.Position.Y;
        default: return toInt(FoundEntity.reflectGet(p,strs,ints,1));
      }
    });
    registerInterop("killPlayer",(List<string> strs, List<int> ints)=>{
      Player p = Engine.Scene.Tracker.GetEntity<Player>();
      Vector2 dir = ints.Count>=3?new Vector2(0.1f*(float)ints[1],0.1f*(float)ints[2]):Vector2.Zero;
      if(ints[0]!=0) p.Die(dir);
      return (p!=null && ints[0]!=0)?1:0;
    });
    registerInterop("reflectGet",(List<string> strs, List<int> ints)=>{
      return toInt(FoundEntity.sreflectGet(strs, ints));
    });
    registerInterop("reflectCall",(List<string> strs, List<int> ints)=>{
      if(ints.Count>0 && ints[0]==0) return 0;
      return toInt(FoundEntity.sreflectCall(strs,ints));
    });
    registerInterop("reflectSetInt",(List<string> strs, List<int> ints)=>{
      return toInt(FoundEntity.sreflectSet(strs, ints));
    });
    registerInterop("timeSinceTrans",(List<string> strs, List<int> ints)=>{
      return (int)UpdateHook.TimeSinceTransMs;
    });
  }

  static HookManager hooks = new HookManager(setupDefaultInterop,void ()=>{}).enable();
}