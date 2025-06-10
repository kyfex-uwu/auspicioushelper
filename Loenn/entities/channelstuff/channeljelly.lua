local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

local channelJelly = {}
channelJelly.name = "auspicioushelper/ChannelJelly"
channelJelly.depth = 2000

channelJelly.placements = {
  {
    name = "Channel Jelly",
    data = {
      state0 = "normal",
      state1 = "normal",
      channel = ""
    }
  }
}
local jellyTypes = {"normal","platform", "fallable", "falling", "withplatform"}
channelJelly.fieldInformation = {
  state0 ={
    options = jellyTypes,
    editable=false,
  },
  state1 = {
    options = jellyTypes,
    editable=false,
  }
}


function channelJelly.sprite(room, entity)
    return {
        drawableSprite.fromTexture("objects/auspicioushelper/channeljelly/idle0", {
            x=entity.x,
            y=entity.y,
            color = aelperLib.channel_color_tint,
        }),
        aelperLib.channel_spriteicon(entity.x, entity.y-10),
    }
end

return channelJelly
