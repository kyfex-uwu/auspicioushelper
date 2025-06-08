local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local beamblocker = {}

beamblocker.name = "auspicioushelper/BeamBlocker"
beamblocker.depth = 2000

beamblocker.placements = {
  {
    name = "main",
    data = {
      width = 8,
      height = 8
    }
  }
}

function beamblocker.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end
beamblocker.fillColor = {0,0.5,0.5,0.3}
beamblocker.borderColor = {0,0.5,0.5,0.7}

return beamblocker