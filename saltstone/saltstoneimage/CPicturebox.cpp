#include "CPicturebox.h"
// #include <gdiplusheaders.h>
// #include <winuser.h>

// bitmap����������Awin32 api loadimage�ł�winV3�܂ł����Ή����Ă��炸
// ����ɓǂݍ��݂ł��Ȃ�
// 1. ���O��bmp����͂����X�^�C���[�W���\�z���`�悷��
// 2. bmp��v3�ɕϊ��ł��Ȃ����H
// https://www.weblio.jp/wkpja/content/Windows+bitmap_Windows+bitmap�̊T�v

bool CPicturebox::sethinst(HINSTANCE hinst) 
{
  hinstance = hinst; // exe��instance handle
  return true;
}

bool CPicturebox::sethwnd(HWND hwnd)
{
  pcthwnd = hwnd; // picturebox��hwnd
  return true;
}
bool CPicturebox::load(LPCWSTR imgfile,Image* outimg)
{
  // lpcstr = unicode�̏ꍇ�Aconst WCHAR*


  // HANDLE hImage = LoadImageA(hinstance, imgfile, IMAGE_BITMAP, LR_DEFAULTSIZE, LR_DEFAULTSIZE, LR_CREATEDIBSECTION);
    //    = loadimageA(hinstance,a, IMAGE_BITMAP);
    // CDC* cdc;
  // image�̑傫�����ǂ�����Ĕ��f���邩�H
  const int w = 100;
  const int h = 100;
  wchar_t cdir[255];
  GetCurrentDirectory(255, cdir);

  // image���������ɓǂݍ���handle��get
  // HANDLE hImage = LoadImageW(hinstance, imgfile, IMAGE_BITMAP, w, h, LR_LOADFROMFILE);
  // HANDLE hImage = LoadImageW(NULL, imgfile, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE || LR_DEFAULTSIZE || LR_CREATEDIBSECTION);
  // const wchar_t* buff = LR"(C:\Users\fuuna\01.bmp)";
  const wchar_t* buff = L"c:\\00.bmp";
  // const char* buff = "01.bmp";
  // HANDLE hImage = LoadImageW(hinstance, buff, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE || LR_DEFAULTSIZE || LR_CREATEDIBSECTION);
  // HBITMAP hImage = (HBITMAP)LoadImageW(NULL, buff, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE);
  // DWORD dw = GetLastError();
  // HBITMAP hImage = (HBITMAP)LoadImageA(NULL, TEXT("01.bmp"), IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE);
  // ��������bitmap��constol�ɃZ�b�g
  // hImage = LoadBitmapW(hinstance, buff);
  // dw = GetLastError();

  // �Ȃ����킩��Ȃ����Ahimage��null�ɂȂ�B�t�@�C�������݂��Ă��Ȃ����A�ǂݍ��݂Ɏ��s���Ă���
  // bitmap�̉摜�`�����s�� ms paint�ŕ`�悷��Ɛ���ɓǂݍ��܂��
  // ������ނ�bitmap�����݂��Ă���Aloadimage�ł͒P����windowsV3�����Ή����Ă��Ȃ�


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
  // hbp�ɓǂ݂���img��raster���R�s�[����΂悢

  // HDC pcthdc = GetDC(pcthwnd); // ��ʕ\����picturebox��hdc device context
  // img�ɂ͓ǂݍ���bmp �����pcthdc�ɃR�s�[����


     

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
