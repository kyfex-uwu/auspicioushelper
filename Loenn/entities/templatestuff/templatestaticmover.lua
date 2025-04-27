local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateStaticmover"
entity.depth = 2000

entity.placements = {
  {
    name = "Template Staticmover",
    data = {
      template = "",
      depthoffset=5,
      channel = "",
      liftspeed_smear = 4,
      smear_average = false
    }
  }
}

entity.texture = "loenn/auspicioushelper/template/tstat"

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-6, entity.y-6, 12, 12)
end
--entity.fillColor = {1,0.3,0.3}

return entity