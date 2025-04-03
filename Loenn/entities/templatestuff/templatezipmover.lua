local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local entity = {}

entity.name = "auspicioushelper/TemplateZipmover"
entity.depth = 2000
entity.nodeLimits = {1,100}
entity.nodeLineRenderType = "line"

local rtypes = {"loop","none", "normal"}
local atypes = {"ride","rideAutomatic"}

entity.placements = {
  {
    name = "Template Zipmover",
    data = {
      template = "",
      depthoffset=5,
      return_type = "normal",
      activation_type = "ride",
      lastNodeIsKnot = true
    }
  }
}
entity.fieldInformation = {
  return_type ={
    options = rtypes,
  },
  activation_type={
    options = atypes
  }
}

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x-2, entity.y-2, 4, 4)
end
entity.fillColor = {1,0.3,0.3}

return entity