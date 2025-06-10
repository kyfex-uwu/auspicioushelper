local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

local entity = {}

entity.name = "auspicioushelper/TemplateMoveblock"
entity.depth = -13000
entity.nodeLimits = {0,100}
entity.nodeLineRenderType = "line"

local directions = {"down","up","left","right"}

entity.placements = {
  {
    name = "Template Moveblock",
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
      movesfx = "event:/game/04_cliffside/arrowblock_move",
      arrow_texture = "small",
      decal_depth = -10001,
      max_leniency=4,
      
      _loenn_display_template = true,
    }
  }
}
entity.fieldInformation = {
  direction = {
    options = directions,
    editable=false
  },
  movesfx = { options = {"event:/game/04_cliffside/arrowblock_move"} },
  respawn_timer = {minimumValue=0},
  max_leniency = {fieldType="integer"},
}

function entity.rectangle(room, entity)
    return utils.rectangle(entity.x-8, entity.y-8, 16, 16)
end
function entity.nodeTexture(room, entity, node)
  local base = "objects/auspicioushelper/templates/movearrows/"
  local addr = entity.arrow_texture
  return "objects/auspicioushelper/templates/movearrows/small00"
end
entity.draw = aelperLib.get_entity_draw("tmovr")

return entity