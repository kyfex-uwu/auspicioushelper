local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/ChannelSprite"
entity.depth = 2000

edge_types = {"loop","clamp","hide"}
entity.placements = {
  {
    name = "Channel Sprite",
    data = {
      channel = "",
      attached = false,
      edge_type = "loop",
      xml_spritename = "auspicioushelper_example1",
      cases=3,
      offsetX=0,
      offsetY=0,
      depth=2
    }
  }
}
entity.fieldInformation = {
  edge_type = {
    options = edge_types
  },
  cases = {
    fieldType="integer"
  },
  offsetX = {
    fieldType="integer"
  },
  offsetY = {
    fieldType="integer"
  },
  depth = {
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