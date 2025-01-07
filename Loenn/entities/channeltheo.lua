local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channeltheo = {}

channeltheo.name = "auspicioushelper/ChannelTheo"
channeltheo.depth = 2000

channeltheo.placements = {
  {
    name = "Channel Theo",
    data = {
      channel = 0,
      switch_thrown_momentum = false,
      swap_thrown_positions = false,
      swap_thrown_positions_nodie = false
    }
  }
}
channeltheo.fieldInformation = {
  channel = {
    fieldType="integer"
  }
}
function channeltheo.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSpriteStruct.fromTexture("util/rect", nil)
  sprite:useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function channeltheo.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, 16, 16)
end

return channeltheo