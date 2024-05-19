#pragma once
#include <string>
using namespace std;


class CSSPicturefile
{
public:
  wstring* filename; // psdの場合はレイヤー名
  bool multiply;
  int brightness; // ブライトネス 明るさ
  int contrast; // コントラスト
  int saturation; // 彩度
  int hue; // 色相
  double alpha; // 透明度
};

