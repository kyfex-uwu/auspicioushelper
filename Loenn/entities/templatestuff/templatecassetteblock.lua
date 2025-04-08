local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateCassetteBlock"
entity.depth = 2000

entity.placements = {
  {
    name = "Template Cassette Block",
    data = {
      template = "",
      depthoffset=5,
      channel = "",
      freeze = false
    }
  }
}

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-2, entity.y-2, 4, 4)
end
entity.fillColor = {0.5,0,1}




return entity