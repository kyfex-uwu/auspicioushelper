local drawableSprite = require("structs.drawable_sprite")

local channel_color = {230/255, 167/255, 50/255}
local dark_multiplier = 0.65
local channel_color_dark = {channel_color[1]*dark_multiplier, channel_color[2]*dark_multiplier, channel_color[3]*dark_multiplier}

function channel_spriteicon(x,y)
    return drawableSprite.fromTexture("loenn/auspicioushelper/channel_icon", {
        x=x, y=y
    })
end

return {
    channel_color = channel_color,
    channel_color_halfopacity = {channel_color[1], channel_color[2], channel_color[3], 0.5},
    channel_color_dark = channel_color_dark,
    channel_color_dark_halfopacity = {channel_color_dark[1], channel_color_dark[2], channel_color_dark[3], 0.5},
    channel_spriteicon = channel_spriteicon,
    channel_spriteicon_entitycenter = function(entity)
        return channel_spriteicon(entity.x+(entity.width or 0)/2, entity.y+(entity.height or 0)/2)
    end,
}