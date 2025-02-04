local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local channelspritechain = {}

channelspritechain.name = "auspicioushelper/SpriteAnimChain"
channelspritechain.depth = 2000
channelspritechain.nodeLimits = {3,100}
channelspritechain.nodeLineRenderType = "line"

channelspritechain.placements = {
  {
    name = "Channel VFX spritechain",
    data = {
      depth = 0,
      seconds_per_node=1.8,
      addfreq=0.5,
      stack_ends=false,
      tangent_freq=1.0,
      tangent_magnitude=16.,
      atlas_directory="particles/starfield/",
      loop=false
    }
  }
}
channelspritechain.fieldInformation = {
  channel = {
    depth="integer"
  }
}
function channelspritechain.sprite(room, entity)
  color = {1, 1, 1, 1}
  local sprite = drawableSpriteStruct.fromTexture("util/rect", nil)
  sprite:useRelativeQuad(0, 0, entity.width, entity.height) 
  sprite.color = color 
  return sprite
end

function channelspritechain.rectangle(room, entity)
  return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

return channelspritechain