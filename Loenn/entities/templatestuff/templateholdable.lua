

local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local aelperLib = require("mods").requireFromPlugin("libraries.aelper_lib")

local entity = {}

entity.name = "auspicioushelper/templateholdable"
entity.depth = -100000
entity.nodeLimits = {0,1}
entity.nodeLineRenderType = "line"

entity.placements = {
  {
    name = "Template Holdable",
    data = {
      width = 8,
      height = 8,
      template = "",
      depthoffset=5,
      cannot_hold_timer=0.1,
      Holdable_collider_expand=4,
      slowfall=false,
      slowrun=true,
      always_collidable=false,
      player_momentum_weight=1.0,
      holdable_momentum_weight=0.0,
      wallhitsound="event:/game/05_mirror_temple/crystaltheo_hit_side",
      wallhit_speedretain=0.4,
      gravity=800,
      terminal_velocity=200,
      friction =350,
      die_to_barrier = false,
      respawning = false,
      respawnDelay = 2,
      die_to_barrier = false,
      respawning = false,
      dontFlingOff = false,
      respawnDelay = 1.5,
      start_floating = false,
      tutorial = false,
      
      _loenn_display_template = true,
    }
  }
}

function entity.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end
entity.fillColor = {0.4,0.9,0.4,0.3}
entity.borderColor = {0.5,1,0.5,1}
function entity.nodeRectangle(room,entity,node,nodeIndex)
  return utils.rectangle(node.x-3,node.y-3,6,6)
end
entity.nodeFillColor = {1,1,1,1}




return entity