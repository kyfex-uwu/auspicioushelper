local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local vflingbird = {}

vflingbird.name = "auspicioushelper/VerticalFlingBird"
vflingbird.depth = 2000
vflingbird.nodeLimits = {0,100}
vflingbird.nodeLineRenderType = "line"

vflingbird.placements = {
  {
    name = "Vertical Fling Bird",
    data = {
      waiting=false
    }
  }
}
vflingbird.fieldInformation = {
  
}
function vflingbird.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSpriteStruct.fromTexture("util/rect", nil)
  sprite:useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function vflingbird.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

return vflingbird