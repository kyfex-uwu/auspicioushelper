local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/templateFiller"
entity.depth = -10000
entity.nodeLimits = {0,1}

entity.placements = {
  {
    name = "Template Filler",
    data = {
      width = 8,
      height = 8,
      template_name = ""
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

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

return entity