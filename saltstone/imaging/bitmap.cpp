#include "pch.h"
#include "bitmap.h"

// TODO simdを使うと早くなるらしい
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
      // char*を無理やりbitmapARGBにcastするので、reinterpret_castが必要
      bitmap = reinterpret_cast<bitmapRawARGB*>(curpoint);
      // bitmap = static_cast<bitmapARGB*>(curpoint);
      // bitmapが透過率255で、黒の場合、スキップ
      __try
      {
        if (bitmap->isBlank() == true)
        {
          continue;
        }
        if (findmode == false)
        {
          // 画像開始位置の探索 -> 候補を発見したので保存
          // 332 , 318?  おかしい
          // 157,266あたりじゃないと -> x yを間違えてた
          if (*x == 0 || *y == 0)
          {
            *x = iw;
            *y = ih;
            *endx = *x;
            *endy = *y;
            findmode = true; // 最後のポイント探索へモードを切り替える

            continue;
          }
        }

        // 続けて最後のポイントを探索
        // 画像があれば候補として保存
        // 全ポイントを探索すれば、最後の画像ポイントがendx,yに保存される
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
      // *x = i;*y = j; break; // 最後の画像点を計算する場合はさらに続行
      // curpoint += sizeof(bitmap);
      // curpoint += sizeof(bitmapRawARGB);

      // A(alphaには255が入っているはずだが 0x00が入っている？ -> pixelformtの指定が間違えていた


      // Aって何がはいってるんだろ？透過色だから、255が入っていると思うな
      // できればclassとして受け取りたい dataptr ,x,y
      // rectangleとか、pointとか
      // 呼び出し側にもclass定義が必要
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
  // srcのbitmap rgba32で周囲の空白を削除する
  // クリップ後の画像をdstへrgba32で作成

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

  // start loc xと endxから作成する画像のwidthを計算
  // xは0スタートだから、実際の値は+1 ? -1 ?
  int width = endx - x + padding;
  int height = endy - y + padding;
  char* srclinep;
  char* destp = dst->data;
  int pixelbytesize = sizeof(bitmapRawARGB); // 4バイト
  int srclinebyte = pixelbytesize * src->width; // srcの１行の長さ
  int destlinebyte = pixelbytesize * width;
  srclinep = src->data + (srclinebyte * y); // コピー開始行の計算
  char* srcp;
  size_t copylen = (width * pixelbytesize);
  // 何かはコピーされているが、ちゃんとはコピーされてない -> srcpの問題だった
  // marginがいるか？ 2pxほど作るか
  for (int iw = 0; iw < height; iw++) {
    // dst.dataにsrc.dataのstart x -> widthをコピー
    srcp = srclinep + (pixelbytesize * x); // 開始位置のptを計算
    std::memcpy(destp, srcp, copylen);
    //srcp += srclinebyte; 
    srclinep += srclinebyte; // srclinep <- コピーするheight位置
    destp += destlinebyte; // -> copylenと同じになるはずだが、、、
  }
  dst->height = height;
  dst->width = width;

  // 後はc#側でどうやればbitmapに変換しやすいか？

  return true;
}




 bool bitmap::composeOverlay(BitmapInfo* src, BitmapInfo* dst)
{
   compose(src, dst, overlay);
   return true;
}
// dstに対し、srcを乗算(Multiply)で合成する
// c#からの呼び出し用
bool bitmap::composeMultiply(BitmapInfo* src, BitmapInfo* dst)
{
  compose(src, dst, multiply);
  return true;

}
// C++内部での呼び出し用
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
      // blend overなら単純にargbを加算
      // multiplyなら % 255 剰余
      if (mode == multiply)
      {
        destp = destp->multiply(srcp, destp);
        continue;
      }
      // 乗算ってこれでいいのかな？
      // 加算では？
      destp = destp->overlay(srcp, destp);
    }
  }
  return true;

}



// bmpから視点と終点を示すrectangleを取得するには別の方法が必要
// memory mapped fileを使用して引数コピーの手間をなくす
// mutexを使用してmmfをロックする
// call source
//  1. mutex lock
//  2. mmfにdata write
//  3. call c++ method
// called dest
//  1. mutex lock
//  2. mmfからdata read
//  3. process
//  4. return
// call source after
//  1. mutex lock
//  2. mmfからdata read
// 単純に関数を呼び出して結果がほしい場合にはこれで十分なはずだ
// 問題は、asyncで処理が終わるまで待機する場合
// 処理終了用のmutexを利用するとか、別の方法を考える必要がある
// かんしくんが参考になるはずだ