local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/ConditionalStrawb"
entity.depth = 2000
entity.nodeLimits = {0,1}
entity.nodeLineRenderType = "line"

entity.placements = {
  {
    name = "Conditional Strawberry",
    data = {
      strawberry_id = "",
      appear_on_ch=false,
      appear_roomenter_only=true,
      appear_channel= "",
      appear_chvalue= 0,

      fly_on_ch = false,
      fly_channel="",
      fly_value=1,

      deathless=false,
      winged=false,
      wingedfollower=false,
      persist_on_death=false,
      sprites=""
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