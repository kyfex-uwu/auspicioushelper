local mods = require("mods")

local trigger = {}

trigger.name = "auspicioushelper/ChannelPlayerTrigger"
trigger.triggerText = "Channel Player Trigger"
local actions = {"jump","dash"}
local ops = {"xor", "and", "or", "set", "max", "min", "add"}
trigger.placements = {
    name = "Channel PT",
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
  }
}
return trigger