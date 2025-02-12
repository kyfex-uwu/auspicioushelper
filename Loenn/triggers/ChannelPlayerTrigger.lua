local mods = require("mods")

local trigger = {}

trigger.name = "auspicioushelper/ChannelPlayerTrigger"
trigger.triggerText = "Channel PT"
local actions = {"jump","dash"}
local ops = {"xor", "and", "or", "set", "max", "min", "add"}
trigger.placements = {
    name = "Channel Player Trigger",
    data = {
      channel = "",
      value = 1,
      op = "set",
      action = "dash"
    }
}
trigger.fieldInformation = {
  channel = {
    fieldType="string"
  },
  op = {
    options=ops
  },
  action = {
    options = actions
  },value = {
    fieldType="integer"
  }
}
return trigger