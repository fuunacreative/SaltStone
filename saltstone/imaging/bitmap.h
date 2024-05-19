#pragma once

// rgba = B 1�o�C�g,G�P�o�C�g,R�P�o�C�g,A�P�o�C�g = 1px 4�o�C�g

DLLEXPORT bool getPctStartXY(char* imeges, int width, int height, int* x, int* y);

enum composemode
{
  overlay,
  multiply
};

//  bitmap rgba�����Z����N���X
class bitmap
{
public:
  static const int maxwidth = 6000; // 5k 5120
  static const int maxheight = 6000; // 5k 2880
  static const int padding = 2; // clipping ����Ƃ��̗]��
  // �摜�J�nxy�𓾂�֐�
  // static bool getPctStartXY(char* imeges,int width,int height,int* x, int* y);
  static bool getPctStartXY(char* images, int width, int height, int* x, int* y, int* endx, int* endy);

  // static bool getClippedBitmap(bitmap src,bitmap dest);
  //  dest��memory alloc�͏I����Ă�����
  // src�ŉ摜�����݂���̈�݂̂�clip����dest��bitmap rgba32���쐬
  static bool getPctStartXY2(BitmapInfo* image, int* x, int* y);
  static bool clipping(BitmapInfo* src, BitmapInfo* dst);
  // dst�ɑ΂��Asrc��ovelayr���č�������
  // c#����̌Ăяo���p
  static bool composeOverlay (BitmapInfo* src, BitmapInfo* dst);
  // dst�ɑ΂��Asrc����Z(Multiply)�ō�������
  // c#����̌Ăяo���p
  static bool composeMultiply(BitmapInfo* src, BitmapInfo* dst);
  // C++�����ł̌Ăяo���p
  static bool compose(BitmapInfo* src, BitmapInfo* dst, composemode mode = overlay);
};

// class rectanble
// {
//  int locationX
//  int locationY
//  int width
//  int height
// };

// interface���ǂ����邩�H
// �ق����@�\
// 1. clipping
//   -> rectangle��Ԃ��Ac#����clipping����H
//     https://stackoverflow.com/questions/734930/how-to-crop-an-image-using-c
//     or
//     c++���łق�������������memcopy���s���A�V����bitmaprgba��Ԃ� -> c#����byte arry -> bitmap�ɕϊ�����
//     ��҂̕����y����
//       https://stackoverflow.com/questions/21555394/how-to-create-bitmap-from-byte-array
//     �����A�A�Amemcopy��������Ƃ߂�ǂ���������
// 2. multiply
//  src�̉摜��dst�֏㏑��(compose overray(�P�����Z) or multiply(��Z)
//  dst�摜�����h�葤�ɂȂ�
//  ��Z�̏ꍇ�A255�𒴂������̂�mod 255�i255�̏��Z�̗]��j�ɂȂ�

