

local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/Spline"
entity.depth = 2000
entity.nodeLimits = {1,100}
entity.nodeLineRenderType = "line"



entity.placements = {
  {
    name = "spline",
    data = {
      identifier = "",
      spline_type = "basic",
      last_node_knot = false
    }
  }
}
local types = {"linear","basic"}
entity.fieldInformation = {
  spline_type ={
    options = types
  },
}
function entity.sprite(room, entity)
  color = {1, 1, 1, 0.3}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-4, entity.y-4, 8, 8)
end

return entity