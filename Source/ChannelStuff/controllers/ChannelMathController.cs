


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Celeste.Mod.Entities;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

[CustomEntity("auspicioushelper/ChannelMathController")]
public class ChannelMathController:Entity{
  byte[] op = null;
  int[] reg;
  enum Op:byte{
    noop, loadZero, loadImmediateByte, loadImmediateInt, loadChannel, storeChannel, copy,
    startAccInit0, startAccInit1, startAccInitImm, startAccInitReg, startAcc, finishAcc,
    mult, div, mod, add, sub, lshift, rshift, and, or, xor, land, lor, max, min, take,
    multI, divI, modI, addI, subI, lshiftI, rshiftI, andI, orI, xorI, landI, lorI, maxI, minI, takeI,
    eq,ne,le,ge,less,greater, eqI,neI,leI,geI,lessI,greaterI, not, lnot,
    jnz, setsptr, setsptrI, loadsptr,
  }
  List<string> usedChannels = new List<string>();
  bool runImmediately;
  bool runEveryFrame;
  bool needsrun=false;
  bool debug=false;
  public ChannelMathController(EntityData d, Vector2 offset):base(new Vector2(0,0)){
    runImmediately = d.Bool("run_immediately",false);
    runEveryFrame = d.Bool("every_frame",false);
    debug = d.Bool("debug",false);
    var bin=Convert.FromBase64String(d.Attr("compiled_operations",""));
    DebugConsole.Write(bin.Length.ToString());
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
    if(runEveryFrame || (!runImmediately && needsrun)){
      run8bitsimple();
      needsrun = false;
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
        case Op.loadImmediateByte: reg[op[iptr++]]=op[iptr++];break;
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
          if(debug) DebugConsole.Write(channel+" "+reg[ridx].ToString());
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
              case Op.take: if(sptr--==0) acc=reg[op[iptr]];break;

              case Op.multI: acc*=op[iptr];break;
              case Op.divI: acc/=op[iptr];break;
              case Op.modI: acc%=op[iptr];break;
              case Op.addI: acc+=op[iptr];break;
              case Op.subI: acc-=op[iptr];break;
              case Op.lshiftI: acc<<=op[iptr];break;
              case Op.rshiftI: acc>>=op[iptr];break;
              case Op.andI: acc&=op[iptr];break;
              case Op.orI: acc|=op[iptr];break;
              case Op.xorI: acc^=op[iptr];break;
              case Op.landI: acc=acc!=0?op[iptr]:0;break;
              case Op.lorI: acc=acc==0?op[iptr]:acc;break;
              case Op.maxI: acc=Math.Max(acc, op[iptr]);break;
              case Op.minI: acc=Math.Min(acc, op[iptr]);break;
              case Op.takeI: if(sptr--==0) acc=op[iptr];break;
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
        case Op.eqI:reg[op[iptr++]]= (reg[op[iptr++]]==op[iptr++])?1:0; break;
        case Op.neI:reg[op[iptr++]]= (reg[op[iptr++]]!=op[iptr++])?1:0; break;
        case Op.leI:reg[op[iptr++]]= (reg[op[iptr++]]<=op[iptr++])?1:0; break;
        case Op.geI:reg[op[iptr++]]= (reg[op[iptr++]]>=op[iptr++])?1:0; break;
        case Op.lessI:reg[op[iptr++]]= (reg[op[iptr++]]<op[iptr++])?1:0; break;
        case Op.greaterI:reg[op[iptr++]]= (reg[op[iptr++]]>op[iptr++])?1:0; break;

        case Op.not: reg[op[iptr++]]=~reg[op[iptr++]]; break;
        case Op.lnot: reg[op[iptr++]]=reg[op[iptr++]]!=0?0:1; break;

        case Op.setsptr: sptr = reg[op[iptr++]]; break;
        case Op.setsptrI: sptr = reg[op[iptr++]]; break;
        case Op.loadsptr: reg[op[iptr++]]=sptr; break; 
        default: break;
      }
    }
  }
}