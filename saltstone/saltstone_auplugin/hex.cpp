#include "pch.h"


hex::hex(int arg)
{
  value = arg;
}

hex::hex(DWORD arg)
{
  value = arg;
}

hex::hex(void* arg)
{
  value = (int)arg;
}


std::string hex::toHex()
{
  
  sprintf_s(buff,sizeof(buff), "0x%08x", value);
  return buff;
}

void hex::dbp(std::string message)
{
  std::string buff = message;
  buff += ":" + toHex();
  dbprint(buff);
}
