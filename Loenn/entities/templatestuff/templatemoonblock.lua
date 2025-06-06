local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateMoonblock"
entity.depth = -13000

entity.placements = {
  {
    name = "Template Moonblock",
    data = {
      template = "",
      depthoffset=5,
      drift_frequency=1,
      drift_amplitude=4,
      sink_amount=12,
      sink_speed=1,
      dash_influence=8,
      startphase=0,
      useCustomStartphase=false,
    }
  }
}

entity.texture = "loenn/auspicioushelper/template/tblk"

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-6, entity.y-6, 12, 12)
end
--entity.borderColor = {0.3,1,0.3}

return entity