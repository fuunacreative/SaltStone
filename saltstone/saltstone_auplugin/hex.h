#pragma once

// binaryを取り扱うクラス

class hex
{
public:
  int value;
  char buff[20]; // dbgprintで参照するためのバッファ

  hex(int arg);
  hex(DWORD arg);
  hex(void* arg);
  // hex(HINSTANCE arg);
  std::string toHex();
  void dbp(std::string message = NULL);
};

