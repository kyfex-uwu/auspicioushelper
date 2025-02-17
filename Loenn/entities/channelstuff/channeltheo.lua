local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

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
channeltheo.fieldInformation = {
  channel = {
    fieldType="string"
  }
}
function channeltheo.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function channeltheo.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, 16, 16)
end

return channeltheo