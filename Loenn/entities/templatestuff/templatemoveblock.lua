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
      arrow_texture = "objects/auspicioushelper/templates/movearrows/small",
      decal_depth = -10001,
      max_leniency=4
    }
  }
}
entity.fieldInformation = {
  direction = {
    options = directions
  }
}

function entity.rectangle(room, entity)
    return utils.rectangle(entity.x-8, entity.y-8, 16, 16)
end
function entity.draw(room, entity, viewport)
    aelperLib.draw_template_sprites(entity.template, entity.x, entity.y, room)
    drawableSprite.fromTexture("loenn/auspicioushelper/template/tmovr", {
        x=entity.x,
        y=entity.y,
    }):draw()
end

return entity