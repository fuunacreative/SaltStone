#include "pch.h"

obj::obj(FILTER* fp)
{
  this->fp = fp;
}

void obj::Init()
{
  exedit = getExeditFilter();
  hinst = exedit->dll_hinst;
  hwnd = getExEditHwnd();
  void* p;
  p = (void*)((uint)hinst + EXEDIT_MEMORY_OFFSET::StaticFilterTable);
  filter = (EXEDIT_FILTER**)p;
  p = (void*)((uint)hinst + EXEDIT_MEMORY_OFFSET::SortedObjectTable);
  object = (EXEDIT_OBJECT**)p;
  p = (void*)((uint)hinst + EXEDIT_MEMORY_OFFSET::ObjectBufferInfo);
  objbuffer = (OBJECT_BUFFER_INFO*)p;
  p = (void*)((uint)hinst + EXEDIT_MEMORY_OFFSET::SortedObjectTable_LayerIndexBegin);
  objindex_start = (int)(*(int*)p);
  indexstart = (int*)p;
  p = (void*)((uint)hinst + EXEDIT_MEMORY_OFFSET::SortedObjectTable_LayerIndexEnd);
  objindex_end = (int)(*(int*)p);
  indexend = (int*)p;
  // object pointer をincrementすると次のobjを参照できる
  // objindex_start -> objindex_endまで
  // objectの数はend - start + 1(２個だとendは1)
  p = (void*)((uint)hinst + EXEDIT_MEMORY_OFFSET::LoadedFilterTable);
  loadfilters = (EXEDIT_FILTER**)p;

  p = (void*)((uint)hinst + EXEDIT_MEMORY_OFFSET::LuaState);
  luastate = (lua_State*)p;
}

int obj::getFilterCount()
{
  SYS_INFO si;
  fp->exfunc->get_sys_info(NULL, &si);
  return si.filter_n;

}

FILTER_DLL* obj::getExeditFilter()
{
  HINSTANCE temp;
  for (int i = getFilterCount(); i;) {
    FILTER_DLL *exedit = (FILTER_DLL*)fp->exfunc->get_filterp(--i);
    temp = exedit->dll_hinst;
    if (!strcmp(exedit->name, EXEDIT_NAME)) return exedit;
  }
  return NULL;

}

HWND obj::getExEditHwnd()
{
  FILTER_DLL* exedit = getExeditFilter();
  if (exedit == NULL)
  {
    return NULL;
  }
  return exedit->hwnd;
}

// dll_hinstからある程度のサイズをbinで保存し、中身を確認する





