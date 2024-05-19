#include "memorymappedfile.h"


unsigned int memorymappedfile::getobjieByString(char* arg)
{

  return std::strtoul(arg, NULL, 16);

}

