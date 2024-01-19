#include "CPicturebox.h"
// #include <gdiplusheaders.h>
// #include <winuser.h>

// bitmapが複数あり、win32 api loadimageではwinV3までしか対応しておらず
// 正常に読み込みできない
// 1. 自前でbmpを解析しラスタイメージを構築し描画する
// 2. bmpをv3に変換できないか？
// https://www.weblio.jp/wkpja/content/Windows+bitmap_Windows+bitmapの概要

bool CPicturebox::sethinst(HINSTANCE hinst) 
{
  hinstance = hinst; // exeのinstance handle
  return true;
}

bool CPicturebox::sethwnd(HWND hwnd)
{
  pcthwnd = hwnd; // pictureboxのhwnd
  return true;
}
bool CPicturebox::load(LPCWSTR imgfile,Image* outimg)
{
  // lpcstr = unicodeの場合、const WCHAR*


  // HANDLE hImage = LoadImageA(hinstance, imgfile, IMAGE_BITMAP, LR_DEFAULTSIZE, LR_DEFAULTSIZE, LR_CREATEDIBSECTION);
    //    = loadimageA(hinstance,a, IMAGE_BITMAP);
    // CDC* cdc;
  // imageの大きさをどうやって判断するか？
  const int w = 100;
  const int h = 100;
  wchar_t cdir[255];
  GetCurrentDirectory(255, cdir);

  // imageをメモリに読み込みhandleをget
  // HANDLE hImage = LoadImageW(hinstance, imgfile, IMAGE_BITMAP, w, h, LR_LOADFROMFILE);
  // HANDLE hImage = LoadImageW(NULL, imgfile, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE || LR_DEFAULTSIZE || LR_CREATEDIBSECTION);
  // const wchar_t* buff = LR"(C:\Users\fuuna\01.bmp)";
  const wchar_t* buff = L"c:\\00.bmp";
  // const char* buff = "01.bmp";
  // HANDLE hImage = LoadImageW(hinstance, buff, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE || LR_DEFAULTSIZE || LR_CREATEDIBSECTION);
  // HBITMAP hImage = (HBITMAP)LoadImageW(NULL, buff, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE);
  // DWORD dw = GetLastError();
  // HBITMAP hImage = (HBITMAP)LoadImageA(NULL, TEXT("01.bmp"), IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE);
  // メモリのbitmapをconstolにセット
  // hImage = LoadBitmapW(hinstance, buff);
  // dw = GetLastError();

  // なぜかわからないが、himageがnullになる。ファイルが存在していないか、読み込みに失敗している
  // bitmapの画像形式が不正 ms paintで描画すると正常に読み込まれる
  // 複数種類のbitmapが存在しており、loadimageでは単純なwindowsV3しか対応していない


  // HDC hdc = GetDC(pcthwnd);
  Image* img = Image::FromFile(buff,TRUE);
  DWORD dw = GetLastError();
  // HBITMAP* hImage = (HBITMAP*)img;
  int nWidth = 400;
  int nHeight = 320;
  BITMAP bitm;
  bitm.bmType = 0;
  bitm.bmHeight = nHeight;
  bitm.bmWidth = nWidth;
  // HBITMAP hbp = CreateBitmapIndirect(&bitm);
  int ih = img->GetHeight();
  int iw = img->GetWidth();
  HBITMAP p = (HBITMAP)img;
  // outbmp = &((HBITMAP)img);
  // hbpに読みこんだimgのrasterをコピーすればよい

  // HDC pcthdc = GetDC(pcthwnd); // 画面表示のpictureboxのhdc device context
  // imgには読み込んだbmp これをpcthdcにコピーする


     

  img = NULL;
  // int intret = ReleaseDC(pcthwnd, pcthdc);

  return true;
}

CPicturebox::~CPicturebox()
{

}

void CPicturebox::Dispose(void)
{
  pcthwnd = NULL;
  hinstance = NULL;

}

bool CPicturebox::getrect(RECT* rect)
{
  return GetWindowRect(pcthwnd, rect);
}

HWND CPicturebox::gethwnd(void)
{
  return pcthwnd;
}

/*
  // CDC* cdc;
cdc = m_Picture2.GetDC();
iWidth = rect2.right - rect2.left;
iHeight = rect2.bottom - rect2.top;
*/
