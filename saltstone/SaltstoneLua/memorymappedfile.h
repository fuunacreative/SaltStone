#pragma once
#include <stdlib.h>
#include <string>

// using namespace System;

class memorymappedfile
{
  // mmfにてc#へ引き渡すパラメータ
  struct _data {
    std::byte objid[4]; // DWORD 2byte -> asciiにして0xA1FF
    std::string charid; // キャラ識別子は２バイト以内、通常は１バイト
    int layer; // カスタムオブジェクトが配置されているレイヤー
    std::string wavfile; // 固定長の文字にするべきか？
    std::string parts;
    float time; // 現在フレームのwavファイル上での経過時間
    unsigned char *data; // 描画先のバッファ
    int height;
    int width;

    // シリアライズを考慮する必要がある


  };

public:
  unsigned int getobjieByString(char* arg);
};

