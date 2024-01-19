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

// obj‚Ì‰Šú‰»ˆ—
// å‚È–Ú“I‚Íhegiht,width‚ğ•Ô‚·‚±‚Æ
int lua_initobj(lua_State* L);
// •`‰æˆ—
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