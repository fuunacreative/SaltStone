#include "main.h"



int lua_test(lua_State* L)
{

  lua_pushinteger(L, 5);

  return 1;
}

int lua_initobj(lua_State* L)
{
  //local height , width = Kazetalk_luadll.initobj(obj.objid,obj.layer,obj.wavfile)
  // objid ������
  // layer ���l
  // wavfile ������

  const char* objid = lua_tostring(L, 1);
  const char* charaid = lua_tostring(L, 2);
  int layer = lua_tointeger(L, 3); // ������ĕK�v�H
  const char* wavfile = lua_tostring(L, 4);
  // charaid 0x0eb8bbf0 "r"
  // charaid������png��height,width���擾
  // objid , charaid , layer , wavfile��ۑ�
  // customobj���������ɓW�J���邩�H -> yes

  // dll -> c#��chara����(exe) -> c++��img��������(dll)
  // c#��chara�����Ƃ̘A�g�͂ǂ����邩�H memory mapped file
  // memory mapped file��struct�ɂ���H

  // ���̂܂�c# exe��mmf�ɓn���΂����̂����ǁA
  // �ł� �Ƃ��āAobjid��charaid�����ۑ����Ƃ�
  std::string charaidstr = charaid;
  std::string objidstr = objid;
  objdata[objidstr] = charaidstr;


  int height = 320;
  int width = 400;
  lua_pushinteger(L, height);
  lua_pushinteger(L, width);
  return 2;
}

int lua_render(lua_State* L)
{
  // Kazetalk_luadll.render(obj.objid, bodyparts, obj.time, data)
  // arg
  //  objid
  //  bodyparts
  //  time wav�t�@�C���̉��ʂ��擾���邽�߂̌o�ߎ���
  //  data png��draw���邽�߂̃|�C���^

  // demo
  // objid�Ɋ�Â��� �ꂢ�� or �܂肳 ��`��
  const char* objid = lua_tostring(L, 1);
  const char* bodyparts = lua_tostring(L, 2);
  const lua_Number frametime = lua_tonumber(L, 3);
  const void* data = lua_topointer(L, 4);
  
  // objid���ǂ����邩�H
  // �Q�o�C�g��W�J���� A1B1�ȂǂƂ���H
  unsigned long objid_long =  std::strtoul(objid, NULL, 16);
  uint objid_int = (int)(objid_long % 0x10000);

  std::string objid_str = objid;
  std::string charaid = objdata[objid_str];
  if (charaid == "�ꂢ��")
  {

  }
  else if (charaid == "�܂肳")
  {

  }
  // data 
  // 1 �s�N�Z�� 4 �o�C�g�ō\������Ă���A
  // ���ʃo�C�g���� B, G, R, A �ƂȂ��Ă��܂��B
  // c++��png��ǂݍ��ނ͓̂��
  // file����Ԃ��Aaviu lua����obj.load�����ق����͂₻��

  return 1; // �Ԃ�l�̐�
}


extern "C" {
  __declspec(dllexport) int luaopen_SaltstoneLua(lua_State* L)
  {
    luaL_register(L, "SaltstoneLua", functions);
    return 1;
  }
}