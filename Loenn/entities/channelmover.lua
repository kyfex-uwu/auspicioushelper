local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channelmover = {}

channelmover.name = "auspicioushelper/ChannelMover"
channelmover.depth = 2000
channelmover.nodeLimits = {1,1}
channelmover.nodeLineRenderType = "line"

channelmover.placements = {
  {
    name = "Channel Mover",
    data = {
      width = 8,
      height = 8,
      channel = "",
      move_time=1.8,
      asymmetry=1.0,
      safe=false
    }
  }
}
channelmover.fieldInformation = {
  channel = {
    fieldType="string"
  }
}
function channelmover.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function channelmover.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

return channelmover