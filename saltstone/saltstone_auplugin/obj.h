#pragma once
#include "pch.h"
#include "include/lua.hpp"

class obj
{
public:
	const LPCSTR EXEDIT_NAME = "拡張編集";
	FILTER_DLL* exedit;
	HWND hwnd;
	// HINSTANCE hinst;
	void* hinst;
	const int memsize = 0x261000;
	EXEDIT_FILTER** filter; // staticfilter
	EXEDIT_OBJECT** object;
	OBJECT_BUFFER_INFO* objbuffer;
	EXEDIT_FILTER** loadfilters;
	// EXEDIT_FILTER** 
	int objindex_start; // これって何を表しているんだろ?
	// 選択レイヤーの概念はない レイヤー毎のobjのpointer startだと思うな
	int objindex_end; // objectのポインタの数 2個だと1になる
	int* indexstart; // layer毎のobjの開始位置と終了位置を保持
	// layerの最大数は100 開始と終了が同じ場合、それ以上objはない
	int* indexend;

	lua_State* luastate;
	

	// 必要なobj tableへのポインタを定義する
	// EXEDIT_FILTER** StaticFilterTable;

	FILTER* fp;

	obj(FILTER* fp);
	void Init();

	int getFilterCount();
	FILTER_DLL* getExeditFilter();
	HWND getExEditHwnd();

};

