local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local spiralbossbeam = {}

spiralbossbeam.name = "auspicioushelper/SpiralBossBeam"
spiralbossbeam.depth = 2000
spiralbossbeam.nodeLimits = {0,1}
spiralbossbeam.nodeLineRenderType = "line"

spiralbossbeam.placements = {
  {
    name = "Spiral Boss Beam",
    data = {
      start_angle = 0.,
      speed = 1.
    }
  }
}
function spiralbossbeam.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSpriteStruct.fromTexture("util/rect", nil)
  sprite:useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function spiralbossbeam.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

return spiralbossbeam