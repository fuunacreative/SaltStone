#pragma once
#include <string>
using namespace std;


class CSSPicturefile
{
public:
  wstring* filename; // psd�̏ꍇ�̓��C���[��
  bool multiply;
  int brightness; // �u���C�g�l�X ���邳
  int contrast; // �R���g���X�g
  int saturation; // �ʓx
  int hue; // �F��
  double alpha; // �����x
};

