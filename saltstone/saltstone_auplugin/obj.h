#pragma once
#include "pch.h"
#include "include/lua.hpp"

class obj
{
public:
	const LPCSTR EXEDIT_NAME = "�g���ҏW";
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
	int objindex_start; // ������ĉ���\���Ă���񂾂�?
	// �I�����C���[�̊T�O�͂Ȃ� ���C���[����obj��pointer start���Ǝv����
	int objindex_end; // object�̃|�C���^�̐� 2����1�ɂȂ�
	int* indexstart; // layer����obj�̊J�n�ʒu�ƏI���ʒu��ێ�
	// layer�̍ő吔��100 �J�n�ƏI���������ꍇ�A����ȏ�obj�͂Ȃ�
	int* indexend;

	lua_State* luastate;
	

	// �K�v��obj table�ւ̃|�C���^���`����
	// EXEDIT_FILTER** StaticFilterTable;

	FILTER* fp;

	obj(FILTER* fp);
	void Init();

	int getFilterCount();
	FILTER_DLL* getExeditFilter();
	HWND getExEditHwnd();

};

