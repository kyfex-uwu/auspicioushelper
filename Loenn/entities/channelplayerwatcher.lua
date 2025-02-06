local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channelplayerwatcher = {}

channelplayerwatcher.name = "auspicioushelper/ChannelPlayerWatcher"
channelplayerwatcher.depth = 2000

local ops = {"xor", "and", "or", "set", "max", "min", "add"}
local actions = {"dash"}

channelplayerwatcher.placements = {
  {
    name = "Channel Player Watcher",
    data = {
      channel = "",
      value = 1,
      op = "set",
      action = "dash"
    }
  }
}
channelplayerwatcher.fieldInformation = {
  channel = {
    fieldType="string"
  },
  op = {
    options=ops
  },
  action = {
    options = actions
  }
}
function channelplayerwatcher.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSprite.fromTexture("util/rect", nil)
  sprite.useRelativeQuad(-4, -4, 8,8) 
  sprite.color = color 
  return sprite
end

function channelplayerwatcher.rectangle(room, entity)
  return utils.rectangle(entity.x-4, entity.y-4, 8,8)
end

return channelplayerwatcher