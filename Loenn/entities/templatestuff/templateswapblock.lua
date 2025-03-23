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
function entity.sprite(room, entity)
  color = {1, 1, 1, 0.3}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-2, entity.y-2, 4, 4)
end

return entity