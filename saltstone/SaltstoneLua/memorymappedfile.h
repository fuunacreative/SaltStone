#pragma once
#include <stdlib.h>
#include <string>

// using namespace System;

class memorymappedfile
{
  // mmf�ɂ�c#�ֈ����n���p�����[�^
  struct _data {
    std::byte objid[4]; // DWORD 2byte -> ascii�ɂ���0xA1FF
    std::string charid; // �L�������ʎq�͂Q�o�C�g�ȓ��A�ʏ�͂P�o�C�g
    int layer; // �J�X�^���I�u�W�F�N�g���z�u����Ă��郌�C���[
    std::string wavfile; // �Œ蒷�̕����ɂ���ׂ����H
    std::string parts;
    float time; // ���݃t���[����wav�t�@�C����ł̌o�ߎ���
    unsigned char *data; // �`���̃o�b�t�@
    int height;
    int width;

    // �V���A���C�Y���l������K�v������


  };

public:
  unsigned int getobjieByString(char* arg);
};

