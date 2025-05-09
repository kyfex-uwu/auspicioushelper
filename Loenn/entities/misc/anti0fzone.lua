local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/Anti0fZone"
entity.depth = -11000
entity.nodeLimits = {0,1}

entity.placements = {
  {
    name = "Anti 0f zone",
    data = {
      width = 8,
      height = 8,
      step = 1,
      holdables=false,
      player_colliders = true,
      triggers = false,
      solids = false,
      cover_whole_room=false
    }
  }
}
function entity.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end
entity.fillColor = {1,0.6,0.2,0.2}
entity.borderColor = {0.7,0.4,0.1,0.7}


return entity