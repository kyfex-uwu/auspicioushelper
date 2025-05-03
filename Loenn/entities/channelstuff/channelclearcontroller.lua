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
      clear_all = false,
      clear_prefix=""
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
channelclearcontroller.texture = "loenn/auspicioushelper/controllers/clear"
function channelclearcontroller.rectangle(room, entity)
  return utils.rectangle(entity.x-4, entity.y-4, 8,8)
end

return channelclearcontroller