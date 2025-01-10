local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channelswitch = {}

channelswitch.name = "auspicioushelper/ChannelSwitch"
channelswitch.depth = 2000

channelswitch.placements = {
  {
    name = "Channel Switch",
    data = {
      channel = 0,
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
  channel = {
    fieldType="integer"
  }
}
function channelswitch.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSpriteStruct.fromTexture("util/rect", nil)
  sprite:useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function channelswitch.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, 16, 16)
end

return channelswitch