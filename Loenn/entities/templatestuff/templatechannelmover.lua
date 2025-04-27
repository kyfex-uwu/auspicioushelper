local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateChannelmover"
entity.depth = 2000
entity.nodeLimits = {1,100}
entity.nodeLineRenderType = "line"

entity.placements = {
  {
    name = "Template Channelmover",
    data = {
      template = "",
      depthoffset=5,
      channel = "",
      move_time=1.8,
      asymmetry=1.0
    }
  }
}

entity.texture = "loenn/auspicioushelper/template/tchan"

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-6, entity.y-6, 12, 12)
end
--entity.fillColor = {1,0.3,0.3}

return entity