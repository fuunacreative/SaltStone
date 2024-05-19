#include "CSBitmap.h"

bool CSSBitmap::setBitmap(Bitmap* argBitmap)
{
  pBitmap = argBitmap;
  return true;
}
Bitmap* CSSBitmap::getBitmap()
{
  return pBitmap;
}

int CSSBitmap::getHeight()
{
  return height;
}
int CSSBitmap::getWidth()
{
  return width;
}


bool CSSBitmap::lockBit()
{
  pBitmapdata = new BitmapData();
  Rect rect(0, 0, width, height);
  pBitmap->LockBits(&rect, ImageLockModeRead | ImageLockModeWrite, PixelFormat32bppARGB, pBitmapdata);
  byteptr = (CSSPixel*)pBitmapdata->Scan0;

  return true;

}

bool CSSBitmap::lockBitReadonly()
{
  pBitmapdata = new BitmapData();
  Rect rect(0, 0, width, height);
  pBitmap->LockBits(&rect, ImageLockModeRead, PixelFormat32bppARGB, pBitmapdata);
  byteptr = (CSSPixel*)pBitmapdata->Scan0;
  if (byteptr == nullptr)
  {
    int ix = 10;
  }
  return true;

}

bool CSSBitmap::unlockBit()
{
  pBitmap->UnlockBits(pBitmapdata);
  return true;
}


CSSPixel* CSSBitmap::getbyteptr()
{
  return byteptr;
}


bool CSSBitmap::load(const wstring* fname)
{
  pBitmap = new Bitmap(fname->c_str());
  if(pBitmap == nullptr)
  {
    int i = GetLastError();
    return false;
  }
  // fnameは存在しているののに、pbitmapがnullになる？
  height = pBitmap->GetHeight();
  width = pBitmap->GetWidth();
  return true;
}

// １、体→顔(通常)→髪(不透明)→眉→目→口→髪（半透明） Aパターン
// ２，体→髪(不透明)→眉→目→口→髪（半透明）→　他 →顔（乗算） 後ろは？

byte calcpixel(byte* src, byte* dst, byte* srcalpha, byte* dstalpha)
{
  // int buf = (int)((*src*(255-*alpha) + (double)(*dst * ((double)*alpha/255))));
  // TODO かすたまいず & 省計算力化


  //double a = *src * (255 - *alpha) + (*alpha) * (*dst);
  //int buff = (int)(a / 255.0);
  //if (*alpha > 10)
  //{
  //  int x = 10;
  //}


  //if (buff > 255) {
  //  buff = 255;
  //}

  // int buf = (int)*src + (int)(*dst);
  //buff = CSSBitmap::ANDVAL & buff;
  // if (buff < 0) {
  //  buff = 0;
  // }
  //alpha = alpha / 255.0;
  // んー。　aC = C' - (1-a)Ctra = ab(1-af)Cb + af*Cfがよくわからんな
  // aCって何？ なんでaをかけてるの？ 透過だからだと思うな
  // buleでも透過が0.5なら125の色にするってことかな
  // alphaはrgbの前に計算できるから、、、
  // この関数ではaCを返す？
  //double a = *src * (255 - *alpha) + (*alpha) * (*dst);
  // 通常合成の場合は、C' = Cf(Color front)になるので、
  //double c = (*src) * (255 - *dstalpha) + (*dst) * (*dstalpha);
  //// c = c / 255.0;
  //// c = c / 255.0;
  //int buff = (int)(c);
  double cback = *src / 255.0;
  double cfront = *dst / 255.0;
  double alphaback = *srcalpha / 255.0;
  double alphafront = *dstalpha / 255.0;
  //if (cfront == 1.0)
  //{
  //  int yyy = 10;

  //}
  //double cdash = (alphafront) * (cfront);
  // cdash = cdash + (1 - alphaback) * cfront;
  double cdash = cfront;
  // double c = (srcv) * (1 - srcalphav) * (dstalphav);
  double c = alphaback * (1 - alphafront) * cback;
  c = c + (alphafront * 1 * cdash);
  //c = c / 255.0;
  int buff = (int)(c * 255);
  return buff;
}

// multiplyの場合、
byte calcpixelmultiply(byte* src, byte* dst, byte* srcalpha, byte* dstalpha)
{
  // https://qiita.com/kerupani129/items/4bf75d9f44a5b926df58
  // double a = *src * (255 - *alpha) * (*alpha) * (*dst);
  // color = alpha * (src*dst) + (1-alpha)(*dst)
  // alpha = front(backgroundのみ) src=back dst=front
  //double a = *src * (*dst);
  // うーん。alphaがわるさしているのはわかるんだが、、、
  // src=0 , dst=255 , alpha=5のとき、0 or 5にならないとおかしいのだが、255になる
  // alpha=1透過の場合にはbackがそのまま表示される
  // 非透過の場合はc_front * (c_back/255)で計算されるはず = *dst側が255 しろになるのだから
  // *dst=255の場合には alphaに関係なくcolor_back=*srcが計算されなければならない
  // alphaは*src側、背景側の透過色
  // 透過色を含めたブレンドを考えなければならない
  //double alp = *alpha / 255.0;
  //double ds = *dst / 255.0;
  //// double a = (alp * (ds * *src)) + ((1 - alp) * ds);
  //double a = (ds * *src);
  //// alphaが255のため、1=透過に変更する -> background=*srcのみの透過を考慮して計算
  //// a = a / 255.0;
  //// int buff = (int)((int)a / 255);
  //// int buff = (int)(a / 255.0);
  //int buff = (int)(a);
  //buff = buff & CSSBitmap::ANDVAL; // 下の１バイトのみ取得
  //buff = CSSBitmap::ANDVAL & buff;
  // alpha = back alpha = 下地側のalpha
  // C'f = Ab * (Cb * Cf) + (1 - Ab)*Cf それぞれ1が上限、
  // back = src , front = dst
  double cback = *src / 255.0;
  double cfront = *dst / 255.0;
  double alphaback = *srcalpha / 255.0;
  double alphafront = *dstalpha / 255.0;
  //if (cfront == 1.0)
  //{
  //  int yyy = 10;

  //}
  double cdash = (alphafront) * (cback) * (cfront);
  cdash = cdash + (1 - alphaback) * cfront;
  // double c = (srcv) * (1 - srcalphav) * (dstalphav);
  double c = alphaback * (1 - alphafront) * cback;
  c = c + (alphafront * 1 * cdash);
  // c = c / 255.0;
  int buff = (int)(c * 255);
  return buff;

}


// bool CSSBitmap::mergeimage(const CSSPicturefile* fname, CSSBitmap_compositemode cpmode)
bool CSSBitmap::mergeimage(const CSSPicturefile* fileinfo)
{
  byte(*cp)(byte*, byte*, byte*, byte*);
  lockBit();
  CSSBitmap* dst = new CSSBitmap();
  dst->load(fileinfo->filename); // dest (overlay)するimage
  dst->lockBitReadonly();
  CSSPixel* srcptr =dst->getbyteptr();
  // 目でerrorが発生する
  cp = &calcpixel;
  if (fileinfo->multiply == true)
  {
    cp = &calcpixelmultiply;
  }

  double work;
  double alphaback = 0;
  double alphafront = 0;
  int alpha;
  byte srcalpha;
  for (int iy = 0; iy < height; iy++) {
    for (int ix = 0; ix < width; ix++) {
      // TODO CSSpixelの合成をclassにまとめる
      if (srcptr->alpha == 0) {
        // 上書き側のalpha 0=透過 => 無視してスキップ
        srcptr++; // 上書き側
        byteptr++;
        continue;
      }
      
      // alphaの計算式変更
      //alpha = srcptr->alpha;
      //dalpha = byteptr->alpha;
      // srcptr=上書きする側の画像 byteptr=下地となる画像
      // ab = 後ろ af=手前
      // Source Over
      //// alpha = ab * (1-af) + af * 1  255を1として扱うので、、、
      //if (iy == 230)
      //{
      //    if (ix == 170)
      //    {
      //      int xxxx = 10;

      //    }
      //}
      // alphafront = srcptr->alpha; // 上書きする方 Alpha_Front
      alphafront = srcptr->alpha * fileinfo->alpha; // 上書きする方 Alpha_Front  上書き側でalphaが指定されている場合は掛け合わせる
      alphaback = byteptr->alpha;
      work = alphaback * (1 - (alphafront/ 255.0)) + alphafront;
      alpha = (int)(work);
      
      if(alphaback >= 255)
      { 
        int xxx = 100;
      }
      if (iy == 240)
      {
        int xxxyyy = 100;
        if (ix == 120)
        {
          int xxxyyyzz = 100;
        }
      }


      // srcptrが上書き側
      
      //byteptr->blue = calcpixel(&(byteptr->blue), &(srcptr->blue), &alpha);
      //byteptr->green = calcpixel(&(byteptr->green), &(srcptr->green), &alpha);
      //byteptr->red = calcpixel(&(byteptr->red), &(srcptr->red), &alpha);
      byteptr->alpha = alpha;
      
      srcalpha = (byte)( srcptr->alpha * fileinfo->alpha); // srcptr->alpha 0~255 , fileinfo.alpha 0-0.25-1
      //byteptr->blue = (*cp)(&(byteptr->blue), &(srcptr->blue), &alpha);
      //byteptr->green = (*cp)(&(byteptr->green), &(srcptr->green), &alpha);
      //byteptr->red = (*cp)(&(byteptr->red), &(srcptr->red), &alpha);
      byteptr->blue = (*cp)(&(byteptr->blue), &(srcptr->blue), &(byteptr->alpha) , &(srcalpha));
      byteptr->blue = (int)((double)byteptr->blue / (alpha / 255.0));
      byteptr->green = (*cp)(&(byteptr->green), &(srcptr->green), &(byteptr->alpha), &(srcalpha));
      byteptr->green = (int)((double)byteptr->green / (alpha / 255.0));
      byteptr->red = (*cp)(&(byteptr->red), &(srcptr->red), &(byteptr->alpha), &(srcalpha));
      byteptr->red = (int)((double)byteptr->red / (alpha / 255.0));
      srcptr++;
      byteptr++;

    }
  }
  dst->unlockBit();
  unlockBit();


  return true;
}

void CSSBitmap::Dispose()
{
  if (pBitmap != nullptr) {
    delete pBitmap;
  }
  if (pBitmapdata != nullptr) {
    delete pBitmapdata;
  }
}


//
// https://learn.microsoft.com/ja-jp/windows/win32/api/gdiplusheaders/nf-gdiplusheaders-bitmap-bitmap(int_int_int_pixelformat_byte)

// 明るさ
// http://www.mis.med.akita-u.ac.jp/~kata/image/colorgamma.html

// こんとらすと
// https://algorithm.joho.info/image-processing/tone-curve/#toc3

// 彩度
// https://blog1.mammb.com/entry/2020/01/20/090000#彩度を変更する
// 
// 色相
// https://blog1.mammb.com/entry/2020/01/20/090000#色相を変更する

// save
// CLSID id = { 0x557cf401, 0x1a04, 0x11d3,{ 0x9a, 0x73, 0x00, 0x00, 0xf8, 0x1e, 0xf3, 0x2e } };
// image.Save(path.c_str(), &id);
// https://learn.microsoft.com/ja-jp/windows/win32/gdiplus/-gdiplus-retrieving-the-class-identifier-for-an-encoder-use

bool CSSBitmap::mergeimagefiles(CSSInputcommand* incmd)
{
  // incmdを元に画像合成を行い、内部 member bitmapに保存
  std::vector<CSSPicturefile*>* picturefiles = incmd->getPicturefiles();
  size_t maxpct = picturefiles->size();
  CSSPicturefile* pctf = picturefiles->at(0);
  bool ret = load(pctf->filename);
  if(ret == false)
  {
    return false;
  }
  for (int i = 1; i <= (maxpct - 1); i++)
  {
    pctf = picturefiles->at(i);
    mergeimage(pctf);
    //} else {
    //  mergeimage(pctf->filename);
    //}
    // multiplyの場合は？
    // csspictuturefile を mergeimageにわたし、内部でmultiplyで演算を変更する
      
    
  }
  return true;
}

bool CSSBitmap::save(wstring* pctfname)
{
  // pBitmap->Save()
  CLSID id;
  // EncoderのCLSIDの取得
  {
    UINT num = 0;
    UINT size = 0;
    Gdiplus::GetImageEncodersSize(&num, &size);
    Gdiplus::ImageCodecInfo* pImageCodecInfo = new Gdiplus::ImageCodecInfo[size];
    Gdiplus::GetImageEncoders(num, size, pImageCodecInfo);
    for (UINT j = 0; j < num; ++j) {
      if (wcscmp(pImageCodecInfo[j].MimeType, L"image/png") == 0) {
        id = pImageCodecInfo[j].Clsid;
        break;
      }
    }
    delete[] pImageCodecInfo;
  }
  // pBitmap->Save(pctfname, &id);
  pBitmap->Save(pctfname->c_str(), &id, NULL);
  

  return true;

}
