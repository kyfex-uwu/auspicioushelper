local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local beamblocker = {}

beamblocker.name = "auspicioushelper/BeamBlocker"
beamblocker.depth = 2000

beamblocker.placements = {
  {
    name = "Beam Blocker",
    data = {
      width = 8,
      height = 8
    }
  }
}
function beamblocker.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSpriteStruct.fromTexture("util/rect", nil)
  sprite:useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function beamblocker.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

return beamblocker