local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channelswitch = {}

channelswitch.name = "auspicioushelper/ChannelSwitch"
channelswitch.depth = 2000

channelswitch.placements = {
  {
    name = "Channel Switch",
    data = {
      channel = "",
      on_only=false,
      off_only=false,
      player_toggle=true,
      throwable_toggle=false,
      seeker_toggle=false,
      on_value=1,
      off_value=0,
      cooldown=1.0
    }
  }
}
channelswitch.fieldInformation = {
  on_value = {
    fieldType="integer"
  },
  off_value = {
    fieldType="integer"
  }
}
channelswitch.texture = "objects/coreFlipSwitch/o1"


return channelswitch