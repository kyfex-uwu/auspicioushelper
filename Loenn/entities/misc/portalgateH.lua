local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/PortalGateH"
entity.depth = 2000
entity.nodeLimits = {1,1}
entity.nodeLineRenderType = "line"

entity.placements = {
  {
    name = "Portal Gate (Horizontal)",
    data = {
      width = 4,
      height = 8,
      right_facing_f0=false,
      right_facing_f1=true,
      color_hex="#FFFA",
      attached=false
    }
  }
}
function entity.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

return entity