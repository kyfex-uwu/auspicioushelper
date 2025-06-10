local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

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
  op = {
    options=ops,
    editabe=false,
  },
  action = {
    options = actions,
    editabe=false,
  }
}
channelplayerwatcher.texture = "loenn/auspicioushelper/controllers/playerwatcher"

return channelplayerwatcher