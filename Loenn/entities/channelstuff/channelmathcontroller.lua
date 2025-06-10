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
entity.fieldInformation = {
    compiled_operations = {
        options = {"https://cloudsbelow.neocities.org/celestestuff/mathcompiler"}
    }
}

entity.texture = "loenn/auspicioushelper/controllers/math"

return entity