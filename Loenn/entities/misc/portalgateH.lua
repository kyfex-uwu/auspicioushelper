local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/PortalGateH"
entity.depth = 2000
entity.nodeLimits = {1,1}
entity.nodeLineRenderType = "line"
entity.nodeVisibility = "always"

entity.placements = {
  {
    name = "Portal Gate (Horizontal)",
    data = {
      height = 8,
      right_facing_f0=false,
      right_facing_f1=true,
      color_hex="#FFFA",
      attached=false
    }
  }
}

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-2, entity.y, 4, entity.height)
end

return entity