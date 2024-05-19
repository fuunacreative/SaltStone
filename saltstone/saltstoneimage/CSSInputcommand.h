#pragma once
#include <string>
// #include <vector>
#include <iostream>
#include <fstream>
#include <vector>
#include <codecvt>
#include <algorithm>
#include "CSSPicturefile.h"
#include <string>
#include <sstream> 

using namespace std;

class CSSInputcommand
{
private:
  wstring* filename; // input cmdのincmd.txt
  int height;
  int width;
  wstring* basedir;
  // std::list<std::wstring>* picturefiles;
  std::vector<CSSPicturefile*>* picturefiles;
  // multiplyをどうやってメモリに格納するか？ 専用class or 別vector
  int type; // ymm3 or ymm4 or psd
  // wstring* outfilename; // output file name

public:
  // accsessor
  wstring* outfile; // 出力するpngファイル名
  // デバッグモード
  bool debugmode;

  int getHeight(void);
  int getWidth(void);
  //std::list<std::wstring>* getPctlists(void);
  // listでは終了判定が難しい
  // forでまわすと iterator endが必ず必要になる
  // wstringのpointer配列がよさげだねー
  std::vector<CSSPicturefile*>* getPicturefiles(void);

  bool parse(const wchar_t* fname);
  void Dispose(void);
};

