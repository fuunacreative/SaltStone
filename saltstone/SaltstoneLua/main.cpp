#include "main.h"



int lua_test(lua_State* L)
{

  lua_pushinteger(L, 5);

  return 1;
}

int lua_initobj(lua_State* L)
{
  //local height , width = Kazetalk_luadll.initobj(obj.objid,obj.layer,obj.wavfile)
  // objid 文字列
  // layer 数値
  // wavfile 文字列

  const char* objid = lua_tostring(L, 1);
  const char* charaid = lua_tostring(L, 2);
  int layer = lua_tointeger(L, 3); // これって必要？
  const char* wavfile = lua_tostring(L, 4);
  // charaid 0x0eb8bbf0 "r"
  // charaidを元にpngのheight,widthを取得
  // objid , charaid , layer , wavfileを保存
  // customobjをメモリに展開するか？ -> yes

  // dll -> c#のchara処理(exe) -> c++のimg合成処理(dll)
  // c#のchara処理との連携はどうするか？ memory mapped file
  // memory mapped fileをstructにする？

  // そのままc# exeのmmfに渡せばいいのだけど、
  // でも として、objidとcharaidだけ保存しとく
  std::string charaidstr = charaid;
  std::string objidstr = objid;
  objdata[objidstr] = charaidstr;


  int height = 320;
  int width = 400;
  lua_pushinteger(L, height);
  lua_pushinteger(L, width);
  return 2;
}

int lua_render(lua_State* L)
{
  // Kazetalk_luadll.render(obj.objid, bodyparts, obj.time, data)
  // arg
  //  objid
  //  bodyparts
  //  time wavファイルの音量を取得するための経過時間
  //  data pngをdrawするためのポインタ

  // demo
  // objidに基づいて れいむ or まりさ を描画
  const char* objid = lua_tostring(L, 1);
  const char* bodyparts = lua_tostring(L, 2);
  const lua_Number frametime = lua_tonumber(L, 3);
  const void* data = lua_topointer(L, 4);
  
  // objidをどうするか？
  // ２バイトを展開して A1B1などとする？
  unsigned long objid_long =  std::strtoul(objid, NULL, 16);
  uint objid_int = (int)(objid_long % 0x10000);

  std::string objid_str = objid;
  std::string charaid = objdata[objid_str];
  if (charaid == "れいむ")
  {

  }
  else if (charaid == "まりさ")
  {

  }
  // data 
  // 1 ピクセル 4 バイトで構成されており、
  // 下位バイトから B, G, R, A となっています。
  // c++でpngを読み込むのは難しい
  // file名を返し、aviu lua側でobj.loadしたほうがはやそう

  return 1; // 返り値の数
}


extern "C" {
  __declspec(dllexport) int luaopen_SaltstoneLua(lua_State* L)
  {
    luaL_register(L, "SaltstoneLua", functions);
    return 1;
  }
}