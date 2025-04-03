local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/templateFiller"
entity.depth = -100000
entity.nodeLimits = {0,1}
entity.nodeLineRenderType = "line"

entity.placements = {
  {
    name = "Template Filler",
    data = {
      width = 8,
      height = 8,
      template_name = ""
    }
  }
}

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end
entity.fillColor = {0.2,0.45,0.6,0.3}
entity.borderColor = {0.5,0.8,1,1}
function entity.nodeRectangle(room,entity,node,nodeIndex)
  return utils.rectangle(node.x-3,node.y-3,6,6)
end
entity.nodeFillColor = {1,1,1,1}




return entity