local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateFallingblock"
entity.depth = 2000

local directions = {"down","up","left","right"}

entity.placements = {
  {
    name = "Template Falling Block",
    data = {
      template = "",
      depthoffset=5,
      direction="down",
      reverseChannel="",
      triggerChannel="",
      gravity = 500,
      max_speed = 130,
      impact_sfx = "event:/game/general/fallblock_impact",
      shake_sfx = "event:/game/general/fallblock_shake"
    }
  }
}
entity.fieldInformation = {
  direction = {
    options = directions
  }
}

entity.texture = "loenn/auspicioushelper/template/tfall"

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-6, entity.y-6, 12, 12)
end
--entity.fillColor = {1,0.3,0.3}

return entity