local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

local entity = {}

entity.name = "auspicioushelper/TemplateChannelmover"
entity.depth = -13000
entity.nodeLimits = {1,-1}
entity.nodeLineRenderType = "line"

entity.placements = {
  {
    name = "Template Channelmover",
    data = {
      template = "",
      depthoffset=5,
      channel = "",
      move_time=1.8,
      asymmetry=1.0
    }
  }
}

function entity.selection(room, entity)
    local nodes = {}
    for _,node in ipairs(entity.nodes) do
        table.insert(nodes, utils.rectangle(node.x-8, node.y-8, 16, 16))
    end
    
    return utils.rectangle(entity.x-8, entity.y-8, 16, 16), nodes
end
function entity.draw(room, entity, viewport)
    aelperLib.draw_template_sprites(entity.template, entity.x, entity.y, room)
    drawableSprite.fromTexture(aelperLib.getIcon("loenn/auspicioushelper/template/tchan"), {
        x=entity.x,
        y=entity.y,
    }):draw()
end

return entity