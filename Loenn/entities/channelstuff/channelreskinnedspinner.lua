local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/ChannelReskinnedSpinner"
entity.depth = 2000
entity.nodeLimits = {0,1}
entity.nodeLineRenderType = "line"

entity.placements = {
  {
    name = "Channel Reskinned Spinner",
    data = {
      mini=false
    }
  }
}
function entity.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(0, 0, 8, 8) 
  sprite.color = color 
  return sprite
end

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, 8, 8)
end

return entity