local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channelclearcontroller = {}

channelclearcontroller.name = "auspicioushelper/ChannelClearController"
channelclearcontroller.depth = 2000

channelclearcontroller.placements = {
  {
    name = "Channel Clear Controller",
    data = {
      channel = "",
      value = 0,
      clear_all = false
    }
  }
}
channelclearcontroller.fieldInformation = {
  channel = {
    fieldType="string"
  },
  value = {
    fieldType="integer"
  }
}
function channelclearcontroller.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSpriteStruct.fromTexture("util/rect", nil)
  sprite:useRelativeQuad(0, 0, 8,8) 
  sprite.color = color 
  return sprite
end

function channelclearcontroller.rectangle(room, entity)
  return utils.rectangle(entity.x-4, entity.y-4, 8,8)
end

return channelclearcontroller