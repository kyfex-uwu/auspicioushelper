


using System;
using System.Collections.Generic;
using System.Text;
using Celeste.Mods.auspicioushelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.auspicioushelper;

public class ChannelMathController:Entity{
  byte[] op;
  int[] reg;
  enum Op:byte{
    noop, loadZero, loadImmediateByte, loadImmediateInt, loadChannel, storeChannel, copy,
    startAccInit0, startAccInit1, startAccInitImm, startAccInitReg, startAccNoInit, startAcc, finishAcc,
    mult, div, mod, add, sub, lshift, rshift, and, or, xor, land, lor, 
    multI, divI, modI, addI, subI, lshiftI, rshiftI, andI, orI, xorI, landI, lorI,
    eq,ne,le,ge,less,greater, eqI,neI,leI,geI,lessI,greaterI, not, lnot,
    //max, min, maxI, minI, select, jnz,
  }
  public ChannelMathController(EntityData d, Vector2 offset):base(new Vector2(0,0)){
    op=Convert.FromBase64String(d.Attr("op",""));

  }
  public void run8bitsimple(){


    int iptr = 0;
    int ridx = 0;
    int len = 0;
    string channel = "";
    while(iptr<op.Length){
      switch((Op)op[iptr++]){
        case Op.loadZero: reg[op[iptr++]]=0; break;
        case Op.loadImmediateByte: reg[op[iptr++]]=op[iptr++];break;
        case Op.loadImmediateInt: reg[op[iptr++]]=BitConverter.ToInt32(op,iptr);iptr+=4;break;
        case Op.loadChannel:
          ridx=op[iptr++];
          len=op[iptr++];
          channel = BitConverter.ToString(op, iptr, len);
          reg[ridx]=ChannelState.readChannel(channel);
          iptr+=len;
          break;
        case Op.storeChannel:
          ridx=op[iptr++];
          len=op[iptr++];
          channel = BitConverter.ToString(op, iptr, len);
          ChannelState.SetChannel(channel, reg[ridx]);
          iptr+=len;
          break;
        case Op.copy:
          reg[iptr++]=reg[iptr++];break;
        case Op.startAccInit0:
          ridx=op[iptr++]; reg[ridx]=0;
          goto case Op.startAcc;
        case Op.startAccInit1:
          ridx=op[iptr++]; reg[ridx]=1;
          goto case Op.startAcc;
        case Op.startAccInitReg:
          ridx=op[iptr++]; reg[ridx]=reg[op[iptr++]];
          goto case Op.startAcc;
        case Op.startAccInitImm:
          ridx=op[iptr++]; reg[ridx]=op[iptr++];
          goto case Op.startAcc;
        case Op.startAccNoInit:
          ridx=op[iptr++];
          goto case Op.startAcc;
        case Op.startAcc:
          while((Op)op[iptr]!=Op.finishAcc){
            switch((Op) op[iptr++]){
              case Op.mult: reg[ridx]*=reg[op[iptr]];break;
              case Op.div: reg[ridx]/=reg[op[iptr]];break;
              case Op.mod: reg[ridx]%=reg[op[iptr]];break;
              case Op.add: reg[ridx]+=reg[op[iptr]];break;
              case Op.sub: reg[ridx]-=reg[op[iptr]];break;
              case Op.lshift: reg[ridx]<<=reg[op[iptr]];break;
              case Op.rshift: reg[ridx]>>=reg[op[iptr]];break;
              case Op.and: reg[ridx]&=reg[op[iptr]];break;
              case Op.or: reg[ridx]|=reg[op[iptr]];break;
              case Op.xor: reg[ridx]^=reg[op[iptr]];break;
              case Op.land: reg[ridx]=reg[ridx]!=0?reg[op[iptr]]:0;break;
              case Op.lor: reg[ridx]=reg[ridx]==0?reg[op[iptr]]:reg[ridx];break;

              case Op.multI: reg[ridx]*=op[iptr];break;
              case Op.divI: reg[ridx]/=op[iptr];break;
              case Op.modI: reg[ridx]%=op[iptr];break;
              case Op.addI: reg[ridx]+=op[iptr];break;
              case Op.subI: reg[ridx]-=op[iptr];break;
              case Op.lshiftI: reg[ridx]<<=op[iptr];break;
              case Op.rshiftI: reg[ridx]>>=op[iptr];break;
              case Op.andI: reg[ridx]&=op[iptr];break;
              case Op.orI: reg[ridx]|=op[iptr];break;
              case Op.xorI: reg[ridx]^=op[iptr];break;
              case Op.landI: reg[ridx]=reg[ridx]!=0?op[iptr]:0;break;
              case Op.lorI: reg[ridx]=reg[ridx]==0?op[iptr]:reg[ridx];break;
              default:break;
            }
            iptr++;
          }break;
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
        default: break;
      }
    }
  }
}