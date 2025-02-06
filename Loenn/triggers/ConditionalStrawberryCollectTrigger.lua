local mods = require("mods")

local trigger = {}

trigger.name = "auspicioushelper/ConditionalStrawbCollectTrigger"
trigger.triggerText = "Collect Cond Strawb"
trigger.placements = {
    name = "Conditional Strawberry Collection Trigger",
    data = {
        strawberry_id=""
    }
}

return trigger