#include "pch.h"

// class�ɂ���win dbg�ɏo�͂���ƂƂ���
// txt�ւ�log�����o�����s�����H

void dbprint(char* arg)
{
  const size_t size = strlen(arg) + 1;
  wchar_t* buff = new wchar_t[size];
  size_t retsize;
  // buff��size�����₵��
//  size_t i = sizeof(*buff);
  mbstowcs_s(&retsize,buff,size, arg, _TRUNCATE);
  //mbstowcs_s(buff, arg, size);
  OutputDebugStringW(buff);
  // OutputDebugStringW(L"\n");
  // ���s�������ƁAvs�ł͂��܂��������Adebugview�ł͋󔒍s������

}

void dbprint(std::string arg)
{
  const char* buff = arg.c_str();
  dbprint((char*)buff);
  // �����ŁAstd::string��dbprint���ēx�R�[�����Ă�
}
