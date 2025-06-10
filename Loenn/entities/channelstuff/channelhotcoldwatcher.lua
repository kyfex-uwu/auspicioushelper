local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

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
  Hot_value = {
    fieldType="integer"
  },
  Cold_value = {
    fieldType="integer"
  }
}

function channelhotcoldwatcher.sprite(room, entity)
    return {
        drawableSprite.fromTexture("objects/coreFlipSwitch/switch01", {
            x=entity.x,
            y=entity.y,
            color = aelperLib.channel_color_tint,
        }),
        aelperLib.channel_spriteicon(entity.x, entity.y),
    }
end

return channelhotcoldwatcher