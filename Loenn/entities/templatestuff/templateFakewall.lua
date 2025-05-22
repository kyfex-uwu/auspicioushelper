local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateFakewall"
entity.depth = -13000

entity.placements = {
  {
    name = "Template Fakewall",
    data = {
      template = "",
      depthoffset=5,
      freeze = false,
      dontOnTransitionInto = false,
      disappear_depth = -13000,
      fadespeed = 1
    }
  }
}

entity.texture = "loenn/auspicioushelper/template/tfake"

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-6, entity.y-6, 12, 12)
end
--entity.fillColor = {1,0.3,0.3}

return entity