local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

local entity = {}

entity.name = "auspicioushelper/TemplateBlock"
entity.depth = -13000

local sfxs = {
  "event:/game/general/wall_break_dirt",
  "event:/game/general/wall_break_ice",
  "event:/game/general/wall_break_wood",
  "event:/game/general/wall_break_stone"
}

entity.placements = {
  {
    name = "Template Block",
    data = {
      template = "",
      depthoffset=5,
      visible = true,
      collidable = true,
      active = true,
      only_redbubble_or_summit_launch = false,
      persistent = false,
      canbreak = true,
      breaksfx = "event:/game/general/wall_break_stone"
    }
  }
}
entity.fieldInformation = {
  breaksfx ={
    options = sfxs,
  }
}
function entity.rectangle(room, entity)
    return utils.rectangle(entity.x-8, entity.y-8, 16, 16)
end
function entity.draw(room, entity, viewport)
    aelperLib.draw_template_sprites(entity.template, entity.x, entity.y, room)
    drawableSprite.fromTexture("loenn/auspicioushelper/template/tblk", {
        x=entity.x,
        y=entity.y,
    }):draw()
end

return entity