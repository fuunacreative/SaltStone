#pragma once
#include <Windows.h>
#include <string>
#include <map>
#include "include/lua.hpp"
#include "memorymappedfile.h"

typedef unsigned int uint;


class main
{
};



std::map<std::string, std::string> objdata;




int lua_test(lua_State* L);

// objの初期化処理
// 主な目的はhegiht,widthを返すこと
int lua_initobj(lua_State* L);
// 描画処理
int lua_render(lua_State* L);

static luaL_Reg functions[] = {
  {"test" ,lua_test },
  {"initobj" , lua_initobj },
  {"render" , lua_render },
  {nullptr ,nullptr }

};

extern "C" {
  __declspec(dllexport) int luaopen_SaltstoneLua(lua_State* L);
}