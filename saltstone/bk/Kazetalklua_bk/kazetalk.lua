local P = {}

debug_print(package.path)
debug_print(package.cpath)
local Kazetalk_luadll = require("Kazetalk_luadll")


local function render(obj)

  -- local height , width = Kazetalk_luadll.initobj(obj.objid,obj.layer,obj.wavfile)
  local height = 320
  local width = 400
  obj.setoption("drawtarget","tempbuffer" , width , height)
  obj.load("tempbuffer")
  local data ,w ,h = obj.getpixeldata("work")

  -- print(obj,obj.body);
  -- debug_print((obj.body or ""))
  local bodytbl = {}
  -- B00F00E00Y00L00K00O00 // B=body,F=face,E=eye,Y=mayu,L=lip,K=back,O=other
  if obj.body == nil then
    print(obj,"nil")
  end
  table.insert(bodytbl,"B"..(obj.body or "")); -- 体
  table.insert(bodytbl,"F"..obj.track0); -- 顔
  table.insert(bodytbl,"E"..obj.track1); -- 目
  table.insert(bodytbl,"Y"..obj.track3); -- 眉
  table.insert(bodytbl,"L"..obj.track2); -- 口
  table.insert(bodytbl,"K"..obj.back); -- 後ろ
  table.insert(bodytbl,"O"..obj.other); -- 他
  local bodyparts = table.concat(bodytbl)
  -- A00
  -- ex B00F0E0Y0L0KO

  -- Kazetalk_luadll.render(obj.objid, bodyparts,obj.time, data, width, height)

  debug_print(bodyparts)

  obj.putpixeldata(data)
end

local function print(obj, msg)
  obj.load("figure", "\148\119\140\105", 0, 1, 1)
  obj.alpha = 0.75
  obj.draw()
  obj.setfont("MS UI Gothic", 16, 0, "0xffffff", "0x000000")
  obj.load("text", "<s,,B>" .. msg)
  obj.draw()
  -- テキストのぼやけ防止
  obj.ox = obj.w % 2 == 1 and 0.5 or 0
  obj.oy = obj.h % 2 == 1 and 0.5 or 0
end


P.render = render
return P
