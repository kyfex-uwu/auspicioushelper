local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channelJelly = {}
channelJelly.name = "auspicioushelper/ChannelJelly"
channelJelly.depth = 2000

channelJelly.placements = {
  {
    name = "Channel Jelly",
    data = {
      state0 = "normal",
      state1 = "normal",
      channel = 0
    }
  }
}
local jellyTypes = {"normal","platform", "fallable", "falling", "withplatform"}
channelJelly.fieldInformation = {
  state0 ={
    options = jellyTypes
  },
  state1 = {
    options = jellyTypes
  },
  channel = {
    fieldType="integer"
  }
}

channelJelly.texture = "objects/auspicioushelper/channeljelly/idle0"

function channelJelly.rectangle(room, entity)
  return utils.rectangle(entity.x-8, entity.y-8, 16, 16)
end

return channelJelly
