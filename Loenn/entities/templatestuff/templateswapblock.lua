local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateSwapblock"
entity.depth = 2000
entity.nodeLimits = {1,100}
entity.nodeLineRenderType = "line"

entity.placements = {
  {
    name = "Template Swapblock",
    data = {
      template = "",
      depthoffset=5
    }
  }
}

entity.texture = "loenn/auspicioushelper/template/tswap"

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-6, entity.y-6, 12, 12)
end
--entity.borderColor = {0.3,1,0.3}

return entity