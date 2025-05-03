local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local materialcontroller = {}

materialcontroller.name = "auspicioushelper/MaterialController"
materialcontroller.depth = 2000

local ftypes = {"Always","Never","Linear","Cosine","Sqrt"}
materialcontroller.placements = {
  {
    name = "Material Controller",
    data = {
      path="",
      identifier="",
      params="",
      depth = 0,
      Fade_in = "Linear",
      fadeOut = "Linear",
      independent = true,
      simple = false,
      always = false,
      useprev = false,
      usebg = false,
      reload=false
    }
  }
}
materialcontroller.fieldInformation = {
  channel = {
    fieldType="string"
  },
  value = {
    fieldType="integer"
  },
  Fade_in = {
    options = ftypes
  },
  fadeOut = {
    options = ftypes
  }
}
materialcontroller.texture = "loenn/auspicioushelper/controllers/material"


function materialcontroller.rectangle(room, entity)
  return utils.rectangle(entity.x-4, entity.y-4, 8,8)
end

return materialcontroller