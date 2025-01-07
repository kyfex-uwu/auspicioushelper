local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channelBooster = {}
channelBooster.name = "auspicioushelper/ChannelBooster"
channelBooster.depth = 2000

channelBooster.placements = {
  {
    name = "Channel Booster",
    data = {
      state0 = "normal",
      state1 = "normal",
      channel = 0,
      self_activating = false
    }
  }
}
local boosterTypes = {"normal","reversed","none"}
channelBooster.fieldInformation = {
  state0 ={
    options = boosterTypes
  },
  state1 = {
    options = boosterTypes
  },
  channel = {
    fieldType="integer"
  },
  self_activating = {
    fieldType="boolean"
  }
}

channelBooster.texture = "objects/auspicioushelper/channelbooster/booster00"

function channelBooster.rectangle(room, entity)
  return utils.rectangle(entity.x-8, entity.y-8, 16, 16)
end

return channelBooster
