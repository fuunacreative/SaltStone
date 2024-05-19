#pragma once

// rgba = B 1バイト,G１バイト,R１バイト,A１バイト = 1px 4バイト

DLLEXPORT bool getPctStartXY(char* imeges, int width, int height, int* x, int* y);

enum composemode
{
  overlay,
  multiply
};

//  bitmap rgbaを演算するクラス
class bitmap
{
public:
  static const int maxwidth = 6000; // 5k 5120
  static const int maxheight = 6000; // 5k 2880
  static const int padding = 2; // clipping するときの余白
  // 画像開始xyを得る関数
  // static bool getPctStartXY(char* imeges,int width,int height,int* x, int* y);
  static bool getPctStartXY(char* images, int width, int height, int* x, int* y, int* endx, int* endy);

  // static bool getClippedBitmap(bitmap src,bitmap dest);
  //  destのmemory allocは終わっている状態
  // srcで画像が存在する領域のみをclipしてdestにbitmap rgba32を作成
  static bool getPctStartXY2(BitmapInfo* image, int* x, int* y);
  static bool clipping(BitmapInfo* src, BitmapInfo* dst);
  // dstに対し、srcをovelayrして合成する
  // c#からの呼び出し用
  static bool composeOverlay (BitmapInfo* src, BitmapInfo* dst);
  // dstに対し、srcを乗算(Multiply)で合成する
  // c#からの呼び出し用
  static bool composeMultiply(BitmapInfo* src, BitmapInfo* dst);
  // C++内部での呼び出し用
  static bool compose(BitmapInfo* src, BitmapInfo* dst, composemode mode = overlay);
};

// class rectanble
// {
//  int locationX
//  int locationY
//  int width
//  int height
// };

// interfaceをどうするか？
// ほしい機能
// 1. clipping
//   -> rectangleを返し、c#側でclippingする？
//     https://stackoverflow.com/questions/734930/how-to-crop-an-image-using-c
//     or
//     c++側でほしい部分だけをmemcopyを行い、新しいbitmaprgbaを返す -> c#側でbyte arry -> bitmapに変換する
//     後者の方が楽そう
//       https://stackoverflow.com/questions/21555394/how-to-create-bitmap-from-byte-array
//     だが、、、memcopyがちょっとめんどくさいかな
// 2. multiply
//  srcの画像をdstへ上書き(compose overray(単純加算) or multiply(乗算)
//  dst画像が下塗り側になる
//  乗算の場合、255を超えたものはmod 255（255の除算の余り）になる

