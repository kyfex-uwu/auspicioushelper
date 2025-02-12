local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channelmathcontroller = {}

channelmathcontroller.name = "auspicioushelper/ChannelMathController"
channelmathcontroller.depth = 2000

channelmathcontroller.placements = {
  {
    name = "Channel Math Controller",
    data = {
      compiled_operations = "",
      run_immediately = false,
      every_frame = false,
      debug = false
    }
  }
}
function channelmathcontroller.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(-4, -4, 8,8) 
  sprite.color = color 
  return sprite
end

function channelmathcontroller.rectangle(room, entity)
  return utils.rectangle(entity.x-4, entity.y-4, 8,8)
end

return channelmathcontroller