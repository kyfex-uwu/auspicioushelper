local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateCassetteBlock"
entity.depth = -13000

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

entity.texture = "loenn/auspicioushelper/template/tcass"

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-6, entity.y-6, 12, 12)
end
--entity.fillColor = {0.5,0,1}




return entity