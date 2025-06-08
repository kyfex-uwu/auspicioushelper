local drawableSprite = require("structs.drawable_sprite")
local entities = require("entities")
local utils = require("utils")
local logging = require("logging")

local oldPlaceItem = entities.placeItem
entities.placeItem = function(room, layer, item)
    print("placed item")
    return oldPlaceItem(room, layer, item)
end

--#####--

local dark_multiplier = 0.65

local channel_color = {230/255, 167/255, 50/255}
local channel_color_dark = {channel_color[1]*dark_multiplier, channel_color[2]*dark_multiplier, channel_color[3]*dark_multiplier}
function channel_spriteicon(x,y)
    return drawableSprite.fromTexture("loenn/auspicioushelper/channel_icon", {
        x=x, y=y
    })
end

local templates = {}
function delete_template(entity, oldName)
    for k, v in ipairs(templates[oldName or entity.template_name] or {}) do
        if v == entity then
            table.remove(templates[oldName or entity.template_name], k)
            break
        end
    end
    if #(templates[oldName or entity.template_name] or {nil}) == 0 then
        templates[oldName or entity.template_name] = nil
    end
end

function templateID_from_entity(entity, room)
    return string.sub(room.name, #"zztemplates-"+1).."/"..entity.template_name
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
    
    update_template = function(entity, room, data)
        data = data or {}
        if data.deleting then 
            delete_template(entity)
            return
        end
    
        if data.oldName then delete_template(entity, oldName) end
        local template_name = templateID_from_entity(entity, room)
        logging.info("[Auspicious Helper] "..template_name)
        templates[template_name] = templates[template_name] or {}
        
        table.insert(templates[template_name], {entity, room})
    end,
    draw_template_sprites = function(name, x, y, room)
        local data = (templates[name] or {})[1]
        if data == nil then return {} end
        
        local toDraw = {}
        local offset = {
            data[1].x - (data[1].nodes[1] or {x=data[1].x}).x,
            data[1].y - (data[1].nodes[1] or {x=data[1].y}).y,
        }
        for _,entity in ipairs(data[2].entities) do
            if entity.x >= data[1].x-(entity.width or 0) and entity.x <= data[1].x+data[1].width and
                entity.y >= data[1].y-(entity.height or 0) and entity.y <= data[1].y+data[1].height then
                    
                local movedEntity = utils.deepcopy(entity)
                movedEntity.x=x + (entity.x - data[1].x) + offset[1]
                movedEntity.y=y + (entity.y - data[1].y) + offset[2]
                local toInsert = ({entities.getEntityDrawable(movedEntity._name, nil, room, movedEntity, nil)})[1]
                if toInsert.draw == nil then 
                    for _,v in ipairs(toInsert) do table.insert(toDraw, v) end
                else table.insert(toDraw, toInsert) end
            end
        end
    
        table.sort(toDraw, function (a, b)
            return (a.depth or 0) > (b.depth or 0)
        end)
        
        for _,v in ipairs(toDraw) do
            v:draw(0.9) 
        end
    end,
    templateID_from_entity = templateID_from_entity,
}