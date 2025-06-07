local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local xnaColors = require("consts.xna_colors")

local entity = {}

entity.name = "auspicioushelper/DieWater"
entity.depth = -9999

entity.placements = {
  {
    name = "main",
    data = {
      width = 8,
      height=8
    }
  }
}

entity.fillColor = {xnaColors.LightBlue[1] * 0.3, xnaColors.LightBlue[2] * 0.3, xnaColors.LightBlue[3] * 0.3, 0.6}
entity.borderColor = {1,xnaColors.LightBlue[2] * 0.3, xnaColors.LightBlue[3] * 0.3, 0.8}

return entity