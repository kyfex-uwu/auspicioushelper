local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

local entity = {}

entity.name = "auspicioushelper/TemplateZipmover"
entity.depth = -13000
entity.nodeLimits = {1,-1}
entity.nodeLineRenderType = "line"

local rtypes = {"loop","none", "normal"}
local atypes = {"ride","rideAutomatic","dash","dashAutomatic"}

entity.placements = {
  {
    name = "Template Zipmover",
    data = {
      template = "",
      depthoffset=5,
      return_type = "normal",
      activation_type = "ride",
      channel = "",
      propegateRiding = false,
      lastNodeIsKnot = true
    }
  }
}
entity.fieldInformation = {
  return_type ={
    options = rtypes,
  },
  activation_type={
    options = atypes
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
    drawableSprite.fromTexture("loenn/auspicioushelper/template/tzip", {
        x=entity.x,
        y=entity.y,
    }):draw()
end

return entity