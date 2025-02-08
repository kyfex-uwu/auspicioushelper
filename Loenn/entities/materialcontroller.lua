local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local materialcontroller = {}

materialcontroller.name = "auspicioushelper/MaterialController"
materialcontroller.depth = 2000

materialcontroller.placements = {
  {
    name = "Material Controller",
    data = {
      path="",
      reload=false
    }
  }
}
materialcontroller.fieldInformation = {
  channel = {
    fieldType="string"
  },
  value = {
    fieldType="integer"
  }
}
function materialcontroller.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(-4, -4, 8,8) 
  sprite.color = color 
  return sprite
end

function materialcontroller.rectangle(room, entity)
  return utils.rectangle(entity.x-4, entity.y-4, 8,8)
end

return materialcontroller