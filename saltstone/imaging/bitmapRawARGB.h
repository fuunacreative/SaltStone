#pragma once
class bitmapRawARGB
{
public:
  // unsigned char A;
  // unsigned char R;
  // unsigned char G;
  // unsigned char B;
  unsigned char B;
  unsigned char G;
  unsigned char R;
  unsigned char A;
  bool isBlank();
  bitmapRawARGB* overlay(bitmapRawARGB* src, bitmapRawARGB* dst);
  bitmapRawARGB* multiply(bitmapRawARGB* src, bitmapRawARGB* dst);
};

// ARGB���ƁAxy 0,8��aplha��0�ɂȂ�
// BGRA���ƁA�S�����߂Ƃ��Ĉ�����
// ���������ǂ��������ƂȂ̂��H

