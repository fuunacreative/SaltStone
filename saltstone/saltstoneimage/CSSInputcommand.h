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
  wstring* filename; // input cmd��incmd.txt
  int height;
  int width;
  wstring* basedir;
  // std::list<std::wstring>* picturefiles;
  std::vector<CSSPicturefile*>* picturefiles;
  // multiply���ǂ�����ă������Ɋi�[���邩�H ��pclass or ��vector
  int type; // ymm3 or ymm4 or psd
  // wstring* outfilename; // output file name

public:
  // accsessor
  wstring* outfile; // �o�͂���png�t�@�C����
  // �f�o�b�O���[�h
  bool debugmode;

  int getHeight(void);
  int getWidth(void);
  //std::list<std::wstring>* getPctlists(void);
  // list�ł͏I�����肪���
  // for�ł܂킷�� iterator end���K���K�v�ɂȂ�
  // wstring��pointer�z�񂪂悳�����ˁ[
  std::vector<CSSPicturefile*>* getPicturefiles(void);

  bool parse(const wchar_t* fname);
  void Dispose(void);
};

