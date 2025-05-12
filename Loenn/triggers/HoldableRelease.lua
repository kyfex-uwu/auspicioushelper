

local mods = require("mods")

local trigger = {}

trigger.name = "auspicioushelper/HoldableReleaseTrigger"
trigger.triggerText = "Release Hold"
trigger.placements = {
    name = "Release Holdable Trigger",
    data = {
        force_throw=false,
        only_once=false
    }
}

return trigger