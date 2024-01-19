#include "pch.h"
#include "bitmapRawARGB.h"

bool bitmapRawARGB::isBlank()
{
  // 指定されたポイントが色ありの画像ポイントかを判断する
  if (A == 0)
  {
    return true;
  }
  // ということは、透過色が0じゃない時点で、色ありポイントと判断できる
  // 0=透過 255=透過なし

  // Aが0の場合がある -> 黒色？　おかしいな。口\00.pngではそんな色はないしかもxy=0,8でだ
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
  // 単純に加算でいい？
  // 255を超えたら？ 
  // 下の画像も関係あるはず 顔の上に目を載せるのだから、alphaも関係してくる
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


