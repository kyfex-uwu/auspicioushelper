local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

local channeltheo = {}

channeltheo.name = "auspicioushelper/ChannelTheo"
channeltheo.depth = 2000

channeltheo.placements = {
  {
    name = "Channel Theo",
    data = {
      channel = "",
      switch_thrown_momentum = false,
      swap_thrown_positions = false,
      swap_thrown_positions_nodie = false,
      player_momentum_weight = 1.0,
      theo_momentum_weight = 0.0
    }
  }
}

function channeltheo.sprite(room, entity)
    return {
        drawableSprite.fromTexture("characters/theoCrystal/idle00", {
            x=entity.x,
            y=entity.y-10,
            color = aelperLib.channel_color_tint,
        }),
        aelperLib.channel_spriteicon(entity.x, entity.y-10),
    }
end

return channeltheo