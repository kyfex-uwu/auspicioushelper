local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/lavasandwichAligner"
entity.depth = 2000

entity.placements = {
  {
    name = "lavasandwich aligner",
    data = {
    }
  }
}
entity.fieldInformation = {
  fly_value = {
    fieldType="integer"
  },
  appear_chvalue = {
    fieldType="integer"
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
  return utils.rectangle(entity.x-2, entity.y-2, 4, 4)
end

return entity