local mods = require("mods")

local trigger = {}

trigger.name = "auspicioushelper/ChannelPlayerTrigger"
trigger.triggerText = function(room, self)
    if self.op == "set" then return string.format("set channel \"%s\" to %s", self.channel, self.value) end
    if self.op == "max" or self.op == "min" then
        return string.format("%s(channel \"%s\", %s)", self.op, self.channel, self.value)
    end
    
    return string.format("channel \"%s\" %s %s", self.channel, self.op, self.value)
end
local actions = {"jump","dash","enter","leave"}
local ops = {"xor", "and", "or", "set", "max", "min", "add"}
trigger.placements = {
    name = "main",
    data = {
      channel = "",
      value = 1,
      op = "set",
      action = "dash",
      only_once = false
    }
}
trigger.fieldInformation = {
  op = {
    options=ops
  },
  action = {
    options = actions
  },
  value = {
    fieldType="integer"
  }
}
return trigger