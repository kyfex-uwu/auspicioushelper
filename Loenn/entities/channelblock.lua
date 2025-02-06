local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channelblock = {}

channelblock.name = "auspicioushelper/ChannelBlock"
channelblock.depth = 2000

channelblock.placements = {
  {
    name = "Channel Block",
    data = {
      width = 8,
      height = 8,
      channel = "",
      inverted = false,
      safe = false,
      alwayspresent = false
    }
  }
}
channelblock.fieldInformation = {
  channel = {
    fieldType="string"
  }
}
function channelblock.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function channelblock.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

return channelblock