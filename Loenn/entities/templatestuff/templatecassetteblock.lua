local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

local entity = {}

entity.name = "auspicioushelper/TemplateCassetteBlock"
entity.depth = -13000

entity.placements = {
  {
    name = "Template Cassette Block",
    data = {
      template = "",
      depthoffset=5,
      channel = "",
      freeze = false
    }
  }
}

function entity.rectangle(room, entity)
    return utils.rectangle(entity.x-8, entity.y-8, 16, 16)
end
function entity.draw(room, entity, viewport)
    aelperLib.draw_template_sprites(entity.template, entity.x, entity.y, room)
    drawableSprite.fromTexture("loenn/auspicioushelper/template/tcass", {
        x=entity.x,
        y=entity.y,
    }):draw()
end

return entity