local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local materialcontroller = {}

materialcontroller.name = "auspicioushelper/MaterialController"
materialcontroller.depth = 2000

local ftypes = {"Always","Never","Linear","Cosine","Sqrt"}
materialcontroller.placements = {
  {
    name = "main",
    data = {
      path="",
      identifier="",
      params="",
      depth = 0,
      Fade_in = "Linear",
      fadeOut = "Linear",
      independent = true,
      quadFirst = true,
      always = true,
      useprev = false,
      usebg = false,
      drawInScene = true,
      reload=false
    }
  }
}
materialcontroller.fieldInformation = {
  Fade_in = {
    options = ftypes
  },
  fadeOut = {
    options = ftypes
  }
}
materialcontroller.texture = "loenn/auspicioushelper/controllers/material"

return materialcontroller