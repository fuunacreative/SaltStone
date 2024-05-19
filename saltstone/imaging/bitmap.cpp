#include "pch.h"
#include "bitmap.h"

// TODO simd���g���Ƒ����Ȃ�炵��
// https://qiita.com/saka1_p/items/72c7755086ec985cade6

bool bitmap::getPctStartXY(char* images, int width, int height, int* x, int* y, int* endx, int* endy)
{
  if (width > maxwidth)
  {
    return false;
  }
  if (height > maxheight)
  {
    return false;
  }
  *x = 0;
  *y = 0;
  int curx = 0;
  int cury = 0;
  // char B, G, R, A;
  char* curpoint = images;
  *x = 0;
  *y = 0;
  // int* endx = new int(0);
  // int* endy = new int(0);
  bitmapRawARGB* bitmap;
  bool findmode = false; // false -> find start loc , true -> find end loc
  for (int ih = 0; ih < height; ih++)
  {

    // if (i > 267) {
    //  int xx = 0;
    // }
    for (int iw = 0; iw < width; iw++)
    {
      // char*�𖳗����bitmapARGB��cast����̂ŁAreinterpret_cast���K�v
      bitmap = reinterpret_cast<bitmapRawARGB*>(curpoint);
      // bitmap = static_cast<bitmapARGB*>(curpoint);
      // bitmap�����ߗ�255�ŁA���̏ꍇ�A�X�L�b�v
      __try
      {
        if (bitmap->isBlank() == true)
        {
          continue;
        }
        if (findmode == false)
        {
          // �摜�J�n�ʒu�̒T�� -> ���𔭌������̂ŕۑ�
          // 332 , 318?  ��������
          // 157,266�����肶��Ȃ��� -> x y���ԈႦ�Ă�
          if (*x == 0 || *y == 0)
          {
            *x = iw;
            *y = ih;
            *endx = *x;
            *endy = *y;
            findmode = true; // �Ō�̃|�C���g�T���փ��[�h��؂�ւ���

            continue;
          }
        }

        // �����čŌ�̃|�C���g��T��
        // �摜������Ό��Ƃ��ĕۑ�
        // �S�|�C���g��T������΁A�Ō�̉摜�|�C���g��endx,y�ɕۑ������
        if (iw > *endx)
        {
          *endx = iw;
        }
        if (ih > *endy)
        {
          *endy = ih;
        }
      }
      __finally {
        curpoint += sizeof(bitmapRawARGB);
      }
      // *x = i;*y = j; break; // �Ō�̉摜�_���v�Z����ꍇ�͂���ɑ��s
      // curpoint += sizeof(bitmap);
      // curpoint += sizeof(bitmapRawARGB);

      // A(alpha�ɂ�255�������Ă���͂����� 0x00�������Ă���H -> pixelformt�̎w�肪�ԈႦ�Ă���


      // A���ĉ����͂����Ă�񂾂�H���ߐF������A255�������Ă���Ǝv����
      // �ł����class�Ƃ��Ď󂯎�肽�� dataptr ,x,y
      // rectangle�Ƃ��Apoint�Ƃ�
      // �Ăяo�����ɂ�class��`���K�v
    }
  }
  int recwidth = *endx - *x;
  int recheith = *endy - *y;
  return true;
}



bool bitmap::getPctStartXY2(BitmapInfo* image, int* x, int* y)
{
  // BitmapInfo* image = reinterpret_cast <BitmapInfo*>(imagept);
  int w = image->width;
  int h = image->height;
  char* currentp = image->data;
  *x = 310;
  *y = 104;
  return true;
}

bool bitmap::clipping(BitmapInfo* src, BitmapInfo* dst)
{
  // src��bitmap rgba32�Ŏ��͂̋󔒂��폜����
  // �N���b�v��̉摜��dst��rgba32�ō쐬

  int x, y, endx, endy;
  bool ret = getPctStartXY(src->data, src->width, src->height, &x, &y, &endx, &endy);
  if (ret == false)
  {
    return false;
  }
  // start loc = x,y
  // end loc = endx,endy
  x -= padding;
  y -= padding;

  // start loc x�� endx����쐬����摜��width���v�Z
  // x��0�X�^�[�g������A���ۂ̒l��+1 ? -1 ?
  int width = endx - x + padding;
  int height = endy - y + padding;
  char* srclinep;
  char* destp = dst->data;
  int pixelbytesize = sizeof(bitmapRawARGB); // 4�o�C�g
  int srclinebyte = pixelbytesize * src->width; // src�̂P�s�̒���
  int destlinebyte = pixelbytesize * width;
  srclinep = src->data + (srclinebyte * y); // �R�s�[�J�n�s�̌v�Z
  char* srcp;
  size_t copylen = (width * pixelbytesize);
  // �����̓R�s�[����Ă��邪�A�����Ƃ̓R�s�[����ĂȂ� -> srcp�̖�肾����
  // margin�����邩�H 2px�قǍ�邩
  for (int iw = 0; iw < height; iw++) {
    // dst.data��src.data��start x -> width���R�s�[
    srcp = srclinep + (pixelbytesize * x); // �J�n�ʒu��pt���v�Z
    std::memcpy(destp, srcp, copylen);
    //srcp += srclinebyte; 
    srclinep += srclinebyte; // srclinep <- �R�s�[����height�ʒu
    destp += destlinebyte; // -> copylen�Ɠ����ɂȂ�͂������A�A�A
  }
  dst->height = height;
  dst->width = width;

  // ���c#���łǂ�����bitmap�ɕϊ����₷�����H

  return true;
}




 bool bitmap::composeOverlay(BitmapInfo* src, BitmapInfo* dst)
{
   compose(src, dst, overlay);
   return true;
}
// dst�ɑ΂��Asrc����Z(Multiply)�ō�������
// c#����̌Ăяo���p
bool bitmap::composeMultiply(BitmapInfo* src, BitmapInfo* dst)
{
  compose(src, dst, multiply);
  return true;

}
// C++�����ł̌Ăяo���p
bool bitmap::compose(BitmapInfo* src, BitmapInfo* dst, composemode mode)
{
  int width = src->width;
  int height = src->height;
  if (dst->width != width)
  {
    return false;
  }
  if (dst->height != height)
  {
    return false;
  }
  bitmapRawARGB* srcp;
  bitmapRawARGB* destp;
  srcp = (bitmapRawARGB*)src->data;
  destp = (bitmapRawARGB*)dst->data;
  for(int ih = 0; ih < height;ih++)
  {
    for(int iw = 0;iw < width;iw++)
    { 
      // blend over�Ȃ�P����argb�����Z
      // multiply�Ȃ� % 255 ��]
      if (mode == multiply)
      {
        destp = destp->multiply(srcp, destp);
        continue;
      }
      // ��Z���Ă���ł����̂��ȁH
      // ���Z�ł́H
      destp = destp->overlay(srcp, destp);
    }
  }
  return true;

}



// bmp���王�_�ƏI�_������rectangle���擾����ɂ͕ʂ̕��@���K�v
// memory mapped file���g�p���Ĉ����R�s�[�̎�Ԃ��Ȃ���
// mutex���g�p����mmf�����b�N����
// call source
//  1. mutex lock
//  2. mmf��data write
//  3. call c++ method
// called dest
//  1. mutex lock
//  2. mmf����data read
//  3. process
//  4. return
// call source after
//  1. mutex lock
//  2. mmf����data read
// �P���Ɋ֐����Ăяo���Č��ʂ��ق����ꍇ�ɂ͂���ŏ\���Ȃ͂���
// ���́Aasync�ŏ������I���܂őҋ@����ꍇ
// �����I���p��mutex�𗘗p����Ƃ��A�ʂ̕��@���l����K�v������
// ���񂵂��񂪎Q�l�ɂȂ�͂���