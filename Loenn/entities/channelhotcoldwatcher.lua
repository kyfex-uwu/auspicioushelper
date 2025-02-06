local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channelhotcoldwatcher = {}

channelhotcoldwatcher.name = "auspicioushelper/ChannelHotColdWatcher"
channelhotcoldwatcher.depth = 2000

channelhotcoldwatcher.placements = {
  {
    name = "Channel Coremode Watcher",
    data = {
      channel = "",
      Hot_value = 0,
      Cold_value = 1
    }
  }
}
channelhotcoldwatcher.fieldInformation = {
  channel = {
    fieldType="string"
  },
  Hot_value = {
    fieldType="integer"
  },
  Cold_value = {
    fieldType="integer"
  }
}
function channelhotcoldwatcher.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(-4, -4, 8,8) 
  sprite.color = color 
  return sprite
end

function channelhotcoldwatcher.rectangle(room, entity)
  return utils.rectangle(entity.x-4, entity.y-4, 8,8)
end

return channelhotcoldwatcher