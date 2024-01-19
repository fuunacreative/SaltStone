#include "pch.h"

// classにしてwin dbgに出力するとともに
// txtへもlog書き出しを行うか？

void dbprint(char* arg)
{
  const size_t size = strlen(arg) + 1;
  wchar_t* buff = new wchar_t[size];
  size_t retsize;
  // buffのsizeがあやしい
//  size_t i = sizeof(*buff);
  mbstowcs_s(&retsize,buff,size, arg, _TRUNCATE);
  //mbstowcs_s(buff, arg, size);
  OutputDebugStringW(buff);
  // OutputDebugStringW(L"\n");
  // 改行をいれると、vsではうまくいくが、debugviewでは空白行が入る

}

void dbprint(std::string arg)
{
  const char* buff = arg.c_str();
  dbprint((char*)buff);
  // ここで、std::stringのdbprintを再度コールしてる
}
