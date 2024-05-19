#pragma once
#include "framework.h"
#include <gdiplus.h>
#include <string>
#include "CSSInputcommand.h"
// #include <gdiplusheaders.h>
using namespace Gdiplus;

// bitmapÇÃwrap class

struct  CSSPixel
{
  byte blue;
  byte green;
  byte red;
  byte alpha;
};

typedef enum 
{
  lockreadonly,
  lockwrite

} CSSBitmap_Lockmode;

typedef enum
{
  normal
  ,multiply
} CSSBitmap_compositemode;

class CSSBitmap
{
private:
  Bitmap* pBitmap;
  BitmapData* pBitmapdata;
  int height;
  int width;
  CSSPixel* byteptr;
  // byte calcpixel(byte* src, byte* dst, int* alpha);
  // byte calcpixelmultiply(byte* src, byte* dst, int* alpha);

public:
  const static int ANDVAL = (INT_MAX & 0x00ff); // pixelââéZÇ…óòópÇ∑ÇÈ and ò_óùòa static const
  bool setBitmap(Bitmap* argBitmap);
  Bitmap* getBitmap();
  bool lockBit();
  bool lockBitReadonly();
  bool unlockBit();
  bool load(const wstring* fname);
  // bool mergeimage(const wstring* fname);
  // bool mergeimage(const wstring* fname, CSSBitmap_compositemode cpmode = CSSBitmap_compositemode::normal);
  bool mergeimage(const CSSPicturefile* fileinfo);
  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  CSSPixel* getbyteptr();
  int getHeight();
  int getWidth();
  void Dispose();
  bool mergeimagefiles(CSSInputcommand* incmd);
  bool save(wstring* pctfname);
};

