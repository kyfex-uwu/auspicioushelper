local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TrackedCassette"
entity.depth = 2000
entity.nodeLimits = {2,2}
entity.nodeLineRenderType = "line"

entity.placements = {
  {
    name = "main",
    data = {
      dont_unlock_bside=false,
      use_raw_message=true,
      --always_show=false,
      message="",
      identifier="",
      flag = "",
      line_width=900
    }
  }
}
entity.texture = "collectables/cassette/idle00"
entity.color = {225/255, 186/255, 255/255}

return entity