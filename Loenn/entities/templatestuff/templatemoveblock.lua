local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateMoveblock"
entity.depth = -13000

local directions = {"down","up","left","right"}

entity.placements = {
  {
    name = "Template M*veblock",
    data = {
      template = "",
      depthoffset=5,
      direction="right",
      uncollidable_blocks=false,
      speed=75,
      acceleration=300,
      respawning=true,
      respawn_timer=2,
      Max_stuck=0.15,
      cansteer=false,
      max_leniency=4
    }
  }
}
entity.fieldInformation = {
  direction = {
    options = directions
  }
}

entity.texture = "loenn/auspicioushelper/template/tmovr"

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-6, entity.y-6, 12, 12)
end
--entity.fillColor = {1,0.3,0.3}

return entity