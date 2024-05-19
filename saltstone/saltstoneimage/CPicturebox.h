#pragma once
#include "framework.h"
#include <gdiplus.h>
using namespace Gdiplus;

// #include "resource.h"

// #include <Windows.h>
// #define _AFXDLL

// #include <afxwin.h>
class CPicturebox
{

private:
  HWND pcthwnd;
  HINSTANCE hinstance;
public:
  bool sethinst(HINSTANCE hinst);
  bool sethwnd(HWND hwnd);
  bool load(LPCWSTR imgfile, Gdiplus::Image* outimg);
  ~CPicturebox();
  void Dispose(void);
  bool getrect(RECT* rect);
  HWND gethwnd(void);
};

