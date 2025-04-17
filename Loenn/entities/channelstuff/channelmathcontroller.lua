local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/ChannelMathController"
entity.depth = 2000

entity.placements = {
  {
    name = "Channel Math Controller",
    data = {
      compiled_operations = "",
      run_immediately = false,
      custom_polling_rate = "",
      debug = false
    }
  }
}
function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-5,entity.y-5,10,10)
end
entity.fillColor = {0.6,0.8,0,0.5}
entity.borderColor = {0.8,1,0,1}

return entity