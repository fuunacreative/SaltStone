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
  // fname�͑��݂��Ă���̂̂ɁApbitmap��null�ɂȂ�H
  height = pBitmap->GetHeight();
  width = pBitmap->GetWidth();
  return true;
}

// �P�A�́���(�ʏ�)����(�s����)�������ځ��������i�������j A�p�^�[��
// �Q�C�́���(�s����)�������ځ��������i�������j���@�� ����i��Z�j ���́H

byte calcpixel(byte* src, byte* dst, byte* srcalpha, byte* dstalpha)
{
  // int buf = (int)((*src*(255-*alpha) + (double)(*dst * ((double)*alpha/255))));
  // TODO �������܂��� & �Ȍv�Z�͉�


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
  // ��[�B�@aC = C' - (1-a)Ctra = ab(1-af)Cb + af*Cf���悭�킩����
  // aC���ĉ��H �Ȃ��a�������Ă�́H ���߂����炾�Ǝv����
  // bule�ł����߂�0.5�Ȃ�125�̐F�ɂ�����Ă��Ƃ���
  // alpha��rgb�̑O�Ɍv�Z�ł��邩��A�A�A
  // ���̊֐��ł�aC��Ԃ��H
  //double a = *src * (255 - *alpha) + (*alpha) * (*dst);
  // �ʏ퍇���̏ꍇ�́AC' = Cf(Color front)�ɂȂ�̂ŁA
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

// multiply�̏ꍇ�A
byte calcpixelmultiply(byte* src, byte* dst, byte* srcalpha, byte* dstalpha)
{
  // https://qiita.com/kerupani129/items/4bf75d9f44a5b926df58
  // double a = *src * (255 - *alpha) * (*alpha) * (*dst);
  // color = alpha * (src*dst) + (1-alpha)(*dst)
  // alpha = front(background�̂�) src=back dst=front
  //double a = *src * (*dst);
  // ���[��Balpha����邳���Ă���̂͂킩��񂾂��A�A�A
  // src=0 , dst=255 , alpha=5�̂Ƃ��A0 or 5�ɂȂ�Ȃ��Ƃ��������̂����A255�ɂȂ�
  // alpha=1���߂̏ꍇ�ɂ�back�����̂܂ܕ\�������
  // �񓧉߂̏ꍇ��c_front * (c_back/255)�Ōv�Z�����͂� = *dst����255 ����ɂȂ�̂�����
  // *dst=255�̏ꍇ�ɂ� alpha�Ɋ֌W�Ȃ�color_back=*src���v�Z����Ȃ���΂Ȃ�Ȃ�
  // alpha��*src���A�w�i���̓��ߐF
  // ���ߐF���܂߂��u�����h���l���Ȃ���΂Ȃ�Ȃ�
  //double alp = *alpha / 255.0;
  //double ds = *dst / 255.0;
  //// double a = (alp * (ds * *src)) + ((1 - alp) * ds);
  //double a = (ds * *src);
  //// alpha��255�̂��߁A1=���߂ɕύX���� -> background=*src�݂̂̓��߂��l�����Čv�Z
  //// a = a / 255.0;
  //// int buff = (int)((int)a / 255);
  //// int buff = (int)(a / 255.0);
  //int buff = (int)(a);
  //buff = buff & CSSBitmap::ANDVAL; // ���̂P�o�C�g�̂ݎ擾
  //buff = CSSBitmap::ANDVAL & buff;
  // alpha = back alpha = ���n����alpha
  // C'f = Ab * (Cb * Cf) + (1 - Ab)*Cf ���ꂼ��1������A
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
  dst->load(fileinfo->filename); // dest (overlay)����image
  dst->lockBitReadonly();
  CSSPixel* srcptr =dst->getbyteptr();
  // �ڂ�error����������
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
      // TODO CSSpixel�̍�����class�ɂ܂Ƃ߂�
      if (srcptr->alpha == 0) {
        // �㏑������alpha 0=���� => �������ăX�L�b�v
        srcptr++; // �㏑����
        byteptr++;
        continue;
      }
      
      // alpha�̌v�Z���ύX
      //alpha = srcptr->alpha;
      //dalpha = byteptr->alpha;
      // srcptr=�㏑�����鑤�̉摜 byteptr=���n�ƂȂ�摜
      // ab = ��� af=��O
      // Source Over
      //// alpha = ab * (1-af) + af * 1  255��1�Ƃ��Ĉ����̂ŁA�A�A
      //if (iy == 230)
      //{
      //    if (ix == 170)
      //    {
      //      int xxxx = 10;

      //    }
      //}
      // alphafront = srcptr->alpha; // �㏑������� Alpha_Front
      alphafront = srcptr->alpha * fileinfo->alpha; // �㏑������� Alpha_Front  �㏑������alpha���w�肳��Ă���ꍇ�͊|�����킹��
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


      // srcptr���㏑����
      
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

// ���邳
// http://www.mis.med.akita-u.ac.jp/~kata/image/colorgamma.html

// ����Ƃ炷��
// https://algorithm.joho.info/image-processing/tone-curve/#toc3

// �ʓx
// https://blog1.mammb.com/entry/2020/01/20/090000#�ʓx��ύX����
// 
// �F��
// https://blog1.mammb.com/entry/2020/01/20/090000#�F����ύX����

// save
// CLSID id = { 0x557cf401, 0x1a04, 0x11d3,{ 0x9a, 0x73, 0x00, 0x00, 0xf8, 0x1e, 0xf3, 0x2e } };
// image.Save(path.c_str(), &id);
// https://learn.microsoft.com/ja-jp/windows/win32/gdiplus/-gdiplus-retrieving-the-class-identifier-for-an-encoder-use

bool CSSBitmap::mergeimagefiles(CSSInputcommand* incmd)
{
  // incmd�����ɉ摜�������s���A���� member bitmap�ɕۑ�
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
    // multiply�̏ꍇ�́H
    // csspictuturefile �� mergeimage�ɂ킽���A������multiply�ŉ��Z��ύX����
      
    
  }
  return true;
}

bool CSSBitmap::save(wstring* pctfname)
{
  // pBitmap->Save()
  CLSID id;
  // Encoder��CLSID�̎擾
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
