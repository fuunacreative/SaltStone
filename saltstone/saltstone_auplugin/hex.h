#pragma once

// binary����舵���N���X

class hex
{
public:
  int value;
  char buff[20]; // dbgprint�ŎQ�Ƃ��邽�߂̃o�b�t�@

  hex(int arg);
  hex(DWORD arg);
  hex(void* arg);
  // hex(HINSTANCE arg);
  std::string toHex();
  void dbp(std::string message = NULL);
};

