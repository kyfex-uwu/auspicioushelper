local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/EntityMarkingFlag"
entity.depth = 2000

entity.placements = {
  {
    name = "Entity ID Marker",
    data = {
      path = "0"
    }
  }
}

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-2, entity.y-2, 4, 4)
end
entity.fillColor = {0.3,1,0.4}




return entity