local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateCassetteManagerSimple"
entity.depth = -100000

entity.placements = {
  {
    name = "Template Cassette Manager (simple)",
    data = {
      channel_1="",
      channel_2="",
      channel_3="",
      channel_4="",
      simple_style = false,
    }
  }
}

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-5,entity.y-5,10,10)
end
entity.fillColor = {0.3,0,0.6,0.5}
entity.borderColor = {0.5,0,1,1}




return entity