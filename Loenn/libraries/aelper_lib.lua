local drawableSprite = require("structs.drawable_sprite")
local drawableRectangle = require("structs.drawable_rectangle")
local entities = require("entities")
local decals = require("decals")
local utils = require("utils")
local logging = require("logging")
local depths = require("consts.object_depths")
local matrix = require("utils.matrix")

local settings = require("mods").getModSettings("auspicioushelper")
local menubar = require("ui.menubar").menubar
local viewMenu = $(menubar):find(menu -> menu[1] == "view")[2]
if not $(viewMenu):find(item -> item[1] == "auspicioushelper_legacyicons") then
    table.insert(viewMenu,{
        "auspicioushelper_legacyicons",
        function() settings.auspicioushelper_legacyicons = not settings.auspicioushelper_legacyicons end,
        "checkbox",
        function() return settings.auspicioushelper_legacyicons or false end
    })
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
        templates[template_name] = templates[template_name] or {}
        
        table.insert(templates[template_name], {entity, room})
    end,
    draw_template_sprites = function(name, x, y, room)
        local data = (templates[name] or {})[1]
        if data == nil then return {} end
        
        local toDraw = {}
        local offset = {
            data[1].x - (data[1].nodes or {{x=data[1].x}})[1].x,
            data[1].y - (data[1].nodes or {{x=data[1].y}})[1].y,
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
                else table.insert(toDraw, {
                    func=toInsert, 
                    depth=(type(entity.depth) == "func" and entity.depth(room, movedEntity, nil) or entity.depth) or 0})
                end
            end
        end
        for _,entity in ipairs(data[2].decalsBg) do
            if entity.x >= data[1].x-(entity.width or 0) and entity.x <= data[1].x+data[1].width and
                entity.y >= data[1].y-(entity.height or 0) and entity.y <= data[1].y+data[1].height then
                    
                local movedEntity = utils.deepcopy(entity)
                movedEntity.x=x + (entity.x - data[1].x) + offset[1]
                movedEntity.y=y + (entity.y - data[1].y) + offset[2]
                local toInsert = ({decals.getDrawable(entity.texture, nil, room, movedEntity, nil)})[1]
                table.insert(toDraw, {func=toInsert, depth=entity.depth or depths.bgDecals})
            end
        end 
        for _,entity in ipairs(data[2].decalsFg) do
            if entity.x >= data[1].x-(entity.width or 0) and entity.x <= data[1].x+data[1].width and
                entity.y >= data[1].y-(entity.height or 0) and entity.y <= data[1].y+data[1].height then
                    
                local movedEntity = utils.deepcopy(entity)
                movedEntity.x=x + (entity.x - data[1].x) + offset[1]
                movedEntity.y=y + (entity.y - data[1].y) + offset[2]
                local toInsert = ({decals.getDrawable(entity.texture, nil, room, movedEntity, nil)})[1]
                table.insert(toDraw, {func=toInsert, depth=entity.depth or depths.fgDecals})
            end
        end 
        for tx = -1, data[1].width/8+2 do
            for ty = -1, data[1].height/8+2 do
                if (tx<=0 or ty<=0 or tx>data[1].width/8 or ty>data[1].height/8) == false then
                    if data[2].tilesFg.matrix:getInbounds(tx+data[1].x/8, ty+data[1].y/8) ~= "0" then
                        table.insert(toDraw, {
                            func=drawableRectangle.fromRectangle("bordered", (tx-1)*8+x+offset[1],(ty-1)*8+y+offset[2], 8,8,
                                {0.8,0.8,0.8},{1,1,1}),
                            depth=depths.fgTerrain})
                    end
                if data[2].tilesBg.matrix:getInbounds(tx+data[1].x/8, ty+data[1].y/8) ~= "0" then
                        table.insert(toDraw, {
                            func=drawableRectangle.fromRectangle("bordered", (tx-1)*8+x+offset[1],(ty-1)*8+y+offset[2], 8,8,
                                {0.5,0.5,0.5},{0.6,0.6,0.6}),
                            depth=depths.bgTerrain})
                    end
                end
            end
        end
    
        table.sort(toDraw, function (a, b)
            return a.depth > b.depth
        end)
        
        for _,v in ipairs(toDraw) do
            v.func:draw() 
        end
    end,
    templateID_from_entity = templateID_from_entity,
    
    getIcon = function(name)
        return settings.auspicioushelper_legacyicons and (name.."_legacy") or name
    end,
}
