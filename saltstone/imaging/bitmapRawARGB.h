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

// ARGBだと、xy 0,8でaplhaが0になる
// BGRAだと、全部透過として扱われる
// いったいどういうことなのか？

