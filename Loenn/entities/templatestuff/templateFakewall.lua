local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

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

function entity.rectangle(room, entity)
    return utils.rectangle(entity.x-8, entity.y-8, 16, 16)
end
function entity.draw(room, entity, viewport)
    aelperLib.draw_template_sprites(entity.template, entity.x, entity.y, room)
    drawableSprite.fromTexture("loenn/auspicioushelper/template/tfake", {
        x=entity.x,
        y=entity.y,
    }):draw()
end

return entity