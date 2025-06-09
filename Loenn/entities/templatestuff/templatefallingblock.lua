local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

local entity = {}

entity.name = "auspicioushelper/TemplateFallingblock"
entity.depth = -13000

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

function entity.rectangle(room, entity)
    return utils.rectangle(entity.x-8, entity.y-8, 16, 16)
end
function entity.draw(room, entity, viewport)
    aelperLib.draw_template_sprites(entity.template, entity.x, entity.y, room)
    drawableSprite.fromTexture(aelperLib.getIcon("loenn/auspicioushelper/template/tfall"), {
        x=entity.x,
        y=entity.y,
    }):draw()
end

return entity