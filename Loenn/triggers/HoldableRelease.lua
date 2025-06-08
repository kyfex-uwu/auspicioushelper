

local mods = require("mods")

local trigger = {}

trigger.name = "auspicioushelper/HoldableReleaseTrigger"
trigger.triggerText = "Release Holdable"
trigger.placements = {
    name = "main",
    data = {
        force_throw=false,
        only_once=false
    }
}

return trigger