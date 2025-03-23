local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/DieWater"
entity.depth = 2000

entity.placements = {
  {
    name = "Die water",
    data = {
      width = 8,
      height=8
    }
  }
}
function entity.sprite(room, entity)
  color = {1, 1, 1, 0.3}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end


return entity