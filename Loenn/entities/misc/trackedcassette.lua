local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TrackedCassette"
entity.depth = 2000
entity.nodeLimits = {2,2}
entity.nodeLineRenderType = "line"

entity.placements = {
  {
    name = "Tracked Cassette",
    data = {
      dont_unlock_bside=false,
      use_raw_message=true,
      always_show=false,
      message="",
      identifier="",
      flag = "",
      line_width=900
    }
  }
}
entity.fieldInformation = {
  fly_value = {
    fieldType="integer"
  },
  appear_chvalue = {
    fieldType="integer"
  }
}
function entity.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(0, 0, 8, 8) 
  sprite.color = color 
  return sprite
end

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, 8, 8)
end

return entity