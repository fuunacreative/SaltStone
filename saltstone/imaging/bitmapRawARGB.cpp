#include "pch.h"
#include "bitmapRawARGB.h"

bool bitmapRawARGB::isBlank()
{
  // �w�肳�ꂽ�|�C���g���F����̉摜�|�C���g���𔻒f����
  if (A == 0)
  {
    return true;
  }
  // �Ƃ������Ƃ́A���ߐF��0����Ȃ����_�ŁA�F����|�C���g�Ɣ��f�ł���
  // 0=���� 255=���߂Ȃ�

  // A��0�̏ꍇ������ -> ���F�H�@���������ȁB��\00.png�ł͂���ȐF�͂Ȃ�������xy=0,8�ł�
  return false;
  
  // if (R != 0)
  // {
  //   return true;
  // }
  // if ((R || G || B) == 0)
  // {
  //   return true;
  // }
  // return false;

}



bitmapRawARGB* bitmapRawARGB::overlay(bitmapRawARGB* src, bitmapRawARGB* dst)
{
  // �P���ɉ��Z�ł����H
  // 255�𒴂�����H 
  // ���̉摜���֌W����͂� ��̏�ɖڂ��ڂ���̂�����Aalpha���֌W���Ă���
  // outputRed = (foregroundRed * foregroundAlpha) + (backgroundRed * (1.0 - foregroundAlpha));
  // https://stackoverflow.com/questions/9014729/manually-alpha-blending-an-rgba-pixel-with-an-rgb-pixel
  double alpha = src->A / 255;
  dst->R = (src->R * alpha) + (dst->R * (1 - alpha));
  dst->G = (src->G * alpha) + (dst->G * (1 - alpha));
  dst->B = (src->B * alpha) + (dst->B * (1 - alpha));
  return dst;
}
bitmapRawARGB* bitmapRawARGB::multiply(bitmapRawARGB* src, bitmapRawARGB* dst)
{
  // http://aska-sg.net/pstips/manual/030-palette/030-0011-04_jouzan.html
  dst->A = (int)((dst->A * src->A) / 255);
  dst->R = (int)((dst->R * src->R) / 255);
  dst->G = (int)((dst->G * src->G) / 255);
  dst->B = (int)((dst->B * src->B) / 255);
  return dst;
}


