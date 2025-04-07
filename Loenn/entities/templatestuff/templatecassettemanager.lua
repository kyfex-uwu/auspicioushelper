local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateCassetteManager"
entity.depth = -100000

entity.placements = {
  {
    name = "Template Cassette Manager",
    data = {
      materials = "",
      timings = "",
      onactivate = "",
      ondeactivate = "",
      channel = "",
      beatsPerMeasure = 4,
      beatsPerMinute = 90,
      offset = 0,
      useChannel = false,
      trysync = true,
      correct = true,
    }
  }
}

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-5,entity.y-5,10,10)
end
entity.fillColor = {0.3,0,0.6,0.5}
entity.borderColor = {0.5,0,1,1}




return entity