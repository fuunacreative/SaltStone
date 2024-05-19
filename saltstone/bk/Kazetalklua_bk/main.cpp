#include "include/lua.hpp"

int lua_test(lua_State* L)
{

  lua_pushinteger(L, 5);

  return 1;
}

int lua_initobj(lua_State* L)
{
  //local height , width = Kazetalk_luadll.initobj(obj.objid,obj.layer,obj.wavfile)
  // objid •¶š—ñ
  // layer ”’l
  // wavfile •¶š—ñ
  
  const char* objid = lua_tostring(L, 1);
  int layer = lua_tointeger(L, 2);
  const char* wavfile = lua_tostring(L, 3);

  int height = 320;
  int width = 400;
  lua_pushinteger(L, height);
  lua_pushinteger(L, width);
  return 2;
}

static luaL_Reg functions[] = {
  {"test" ,lua_test },
  {nullptr ,nullptr }

};

extern "C" {
  __declspec(dllexport) int luaopen_Kazetalk_luadll(lua_State* L)
  {
    luaL_register(L, "Kazetalk_luadll", functions);
    return 1;
  }
}