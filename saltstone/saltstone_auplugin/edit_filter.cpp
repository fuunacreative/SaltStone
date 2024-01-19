//----------------------------------------------------------------------------------
//		SaltStone plugin for AviUtl ver0.99i�ȍ~
//----------------------------------------------------------------------------------
#include "pch.h"

#define	WINDOW_W		320
#define	WINDOW_H		240

EXFUNC* exfunc;

//---------------------------------------------------------------------
//		�t�B���^�\���̒�`
//---------------------------------------------------------------------
FILTER_DLL filter = {
  // FILTER_FLAG_DISP_FILTER | FILTER_FLAG_WINDOW_HSCROLL | FILTER_FLAG_WINDOW_THICKFRAME | FILTER_FLAG_ALWAYS_ACTIVE | FILTER_FLAG_WINDOW_SIZE | FILTER_FLAG_PRIORITY_LOWEST | FILTER_FLAG_EX_INFORMATION,
  FILTER_FLAG_ALWAYS_ACTIVE | FILTER_FLAG_EX_INFORMATION,
  //WINDOW_W,WINDOW_H,
  NULL,NULL,
  "SaltStone", // filter name
  NULL,NULL,NULL,
  NULL,NULL,
  NULL,NULL,NULL,
  NULL,
  NULL,
  NULL,
  NULL,
  func_WndProc,
  NULL,NULL,
  NULL,
  NULL,
  "SaltStone version 0.10 by Fuuna", // filter information flag_ex_informatin���ݒ肳��Ă���ꍇ�ɐݒ��
  NULL, // func_save_start
  NULL, // func_save_end
  NULL, //exfunc
  NULL, // hwnd
  NULL, // dll_hinst
  NULL, // ex_data_def
  NULL, // func_is_saveframe
  func_project_load, // project�ɕۑ������f�[�^���Ȃ��ꍇ�ɂ�call����Ȃ�
  func_project_save, // project�ɕۑ������f�[�^���Ȃ��ꍇ�ɂ�call����Ȃ�
};


//---------------------------------------------------------------------
//		�t�B���^�\���̂̃|�C���^��n���֐�
//---------------------------------------------------------------------
EXTERN_C FILTER_DLL __declspec(dllexport)* __stdcall GetFilterTable(void)
{
  return &filter;
}




//---------------------------------------------------------------------
//		�\��
//---------------------------------------------------------------------
void disp(HWND hwnd, FILTER* fp, void* editp, int n, int l, int w, int h)
{
  // HDC					hdc;
  // RECT				rc;
  // BITMAPINFO			bmi;
  // TCHAR				b[MAX_PATH];
  static TCHAR* inter_txt[] = {
    "","���]","�","����","��d��","����"
  };

  if (!fp->exfunc->is_filter_window_disp(fp)) return;



  /*
  GetClientRect(hwnd,&rc);

  //	�t���[���̕\��
  hdc = GetDC(hwnd);
  SetStretchBltMode(hdc,STRETCH_DELETESCANS);
  ZeroMemory(&bmi,sizeof(bmi));
  bmi.bmiHeader.biSize        = sizeof(bmi.bmiHeader);
  bmi.bmiHeader.biWidth       = w;
  bmi.bmiHeader.biHeight      = h;
  bmi.bmiHeader.biPlanes      = 1;
  bmi.bmiHeader.biBitCount    = 24;
  bmi.bmiHeader.biCompression = BI_RGB;
  bmi.bmiHeader.biPlanes      = 1;
  if( fp->exfunc->is_editing(editp) && l ) {
    DIBits(hdc,0,0,rc.right,rc.bottom,0,0,w,h,fp->exfunc->get_pixelp(editp,n),&bmi,DIB_RGB_COLORS,SRCCOPY);
  } else {
    FillRect(hdc,&rc,(HBRUSH)(COLOR_ACTIVEBORDER+1));
  }
  ReleaseDC(hwnd,hdc);
  */

  //	�^�C�g���o�[�̕\��
  /*
  if( fp->exfunc->is_editing(editp) && l ) {
    FRAME_STATUS	fs;
    fp->exfunc->get_frame_status(editp,n,&fs);
    wsprintf(b,"%s  [%d/%d]  %s %s",filter.name,n+1,l,fp->exfunc->get_config_name(editp,fs.config),inter_txt[fs.inter]);
    if( fp->exfunc->is_keyframe(editp,n)   ) lstrcat(b,"*");
    if( fp->exfunc->is_recompress(editp,n) ) lstrcat(b,"!");
    if( !fp->exfunc->is_saveframe(editp,n) ) lstrcat(b,"X");
    if( fs.edit_flag&EDIT_FRAME_EDIT_FLAG_KEYFRAME  ) lstrcat(b,"K");
    if( fs.edit_flag&EDIT_FRAME_EDIT_FLAG_MARKFRAME ) lstrcat(b,"M");
    if( fs.edit_flag&EDIT_FRAME_EDIT_FLAG_DELFRAME  ) lstrcat(b,"D");
    if( fs.edit_flag&EDIT_FRAME_EDIT_FLAG_NULLFRAME ) lstrcat(b,"N");
  } else {
    wsprintf(b,"%s",filter.name);
  }
  SetWindowText(hwnd,b);
  */
}


//---------------------------------------------------------------------
//		�X�N���[���o�[�̐ݒ�
//---------------------------------------------------------------------
void set_scrollbar(HWND hwnd, int n, int l)
{
  SCROLLINFO		si;

  si.cbSize = sizeof(si);
  si.fMask = SIF_DISABLENOSCROLL | SIF_PAGE | SIF_POS | SIF_RANGE;
  si.nMin = 0;
  si.nMax = l - 1;
  si.nPage = 1;
  si.nPos = n;
  // SetScrollInfo(hwnd,SB_HORZ,&si,TRUE);

  return;
}


//---------------------------------------------------------------------
//		WndProc
//---------------------------------------------------------------------
#define	COPY_MODE_VIDEO	0
#define	COPY_MODE_AUDIO	1
#define	COPY_MODE_ALL	2
BOOL func_WndProc(HWND hwnd, UINT message, WPARAM wparam, LPARAM lparam, void* editp, FILTER* fp)
{
  //	TRUE��Ԃ��ƑS�̂��ĕ`�悳���

  // int				i;
  static	int		frame, frame_n, frame_w, frame_h;
  static	int		copy_frame, copy_mode;
  // FILE_INFO		fi;
  SYS_INFO sip;
  // TCHAR			txt[256],txt2[256];
  // std::ofstream fs;
  // wchar_t exepath[MAX_PATH + 2];
/*	DWORD rc = GetModuleFileNameW(NULL, exepath, sizeof(exepath));
  if (rc != 0)
  {
    std::wstring exefile(exepath);
    std::string avidir = std::filesystem::path(exefile).parent_path().u8string();

    fs.open("b.txt");
    fs << avidir << std::endl;
    fs.close();
  }
  */

  switch (message) {

    /*
    case WM_COMMAND:
      if( fp->exfunc->is_editing(editp) != TRUE ) break;	//	�ҏW���łȂ���ΏI��
        switch(LOWORD(wparam)) {


        case MID_EDIT_FILE_INFO:
          fp->exfunc->get_file_info(editp,&fi);
          ZeroMemory(txt,sizeof(txt));
          if( fi.flag&FILE_INFO_FLAG_VIDEO ) {
            wsprintf(txt,"�t�@�C���� : %s\n�T�C�Y : %dx%d\n�t���[�����[�g : %d.%03dfps",
              fi.name,
              fi.w,
              fi.h,
              (int)((double)fi.video_rate/fi.video_scale),
              (int)((double)fi.video_rate*1000/fi.video_scale)%1000
            );
          }
          ZeroMemory(txt2,sizeof(txt2));
          if( fi.flag&FILE_INFO_FLAG_AUDIO ) {
            wsprintf(txt2,"\n�T���v�����O���[�g : %dkHz\n�`�����l���� : %dch",
              fi.audio_rate,
              fi.audio_ch
            );
          }
          lstrcat(txt,txt2);
          MessageBox(hwnd,txt,"�t�@�C���̏��",MB_OK);
          break;
      }
      break;
    */
    //	�X�N���[���o�[����
      /*
    case WM_HSCROLL:
        switch(LOWORD(wparam)) {
        case SB_LINEDOWN:
          frame++;
          if( frame > frame_n-1 ) frame = frame_n-1;
          set_scrollbar(hwnd,frame,frame_n);
          disp(hwnd,fp,editp,frame,frame_n,frame_w,frame_h);
                break;
        case SB_LINEUP:
          frame--;
          if( frame < 0 ) frame = 0;
          set_scrollbar(hwnd,frame,frame_n);
          disp(hwnd,fp,editp,frame,frame_n,frame_w,frame_h);
                break;
        case SB_PAGEDOWN:
          frame+=10;
          if( frame > frame_n-1 ) frame = frame_n-1;
          set_scrollbar(hwnd,frame,frame_n);
          disp(hwnd,fp,editp,frame,frame_n,frame_w,frame_h);
                break;
        case SB_PAGEUP:
          frame-=10;
          if( frame < 0 ) frame = 0;
          set_scrollbar(hwnd,frame,frame_n);
          disp(hwnd,fp,editp,frame,frame_n,frame_w,frame_h);
                break;
        case SB_THUMBTRACK:
          SCROLLINFO	si;
          MSG			msg;
          si.cbSize=sizeof(si);
          si.fMask=SIF_ALL;
          GetScrollInfo(hwnd,SB_HORZ,&si);
          if( PeekMessage(&msg,hwnd,WM_MOUSEMOVE,WM_MOUSEMOVE,PM_NOREMOVE) ) break;
          frame = si.nTrackPos;
          set_scrollbar(hwnd,frame,frame_n);
          disp(hwnd,fp,editp,frame,frame_n,frame_w,frame_h);
              break;
      }
          break;
          */
  case 	WM_FILTER_UPDATE:
    break;

  case WM_FILTER_FILE_OPEN:
  case WM_FILTER_SAVE_END:
    BOOL ret;
    ret = fp->exfunc->get_sys_info(editp, &sip);
    //ret = fp->exfunc->get_file_info(editp, &fi);
    //			std::string a(fi.name);
    if (ret == 0)
    {
      // aviutl 1=true 0=false
      break;
    }
    // fs.open("a.txt");
    // fs << sip.project_name << std::endl;
    // fs.close();
    // aup�t�@�C���̕ۑ��ꏊ���擾���ݒ�t�@�C���ɕۑ�
    // �ݒ�t�@�C����plugindir�֕ۑ�������
    func_exfilter_info(editp, fp);
    // �I������Ă���layer��1������ layer1��obj�����擾�ł��Ȃ��̂ł́H
    break;
    /*
    case WM_FILTER_UPDATE:

      if( fp->exfunc->is_editing(editp) != TRUE ) break;	//	�ҏW���łȂ���ΏI��
      frame_n = fp->exfunc->get_frame_n(editp);
      set_scrollbar(hwnd,frame,frame_n);
      disp(hwnd,fp,editp,frame,frame_n,frame_w,frame_h);
      break;
      */
      /*
      case WM_FILTER_INIT:
        HMENU	hmenu;
        hmenu = LoadMenu(fp->dll_hinst,"FILTER");
        SetMenu(hwnd,hmenu);
        DrawMenuBar(hwnd);
      case WM_FILTER_FILE_CLOSE:
        frame = frame_n = 0;
        set_scrollbar(hwnd,frame,frame_n);
        disp(hwnd,fp,editp,frame,frame_n,frame_w,frame_h);
        break;
      */
  }

  return FALSE;
}


//---------------------------------------------------------------------
//		�v���W�F�N�g�t�@�C���ւ̃f�[�^�ۑ��T���v��
//---------------------------------------------------------------------
typedef struct {
  int		a, b, c;
} PROJECT_FILE_DATA;
static PROJECT_FILE_DATA project_file_data;

BOOL func_project_load(FILTER* fp, void* editp, void* data, int size)
{
  if (size == sizeof(project_file_data)) {
    memcpy(&project_file_data, data, sizeof(project_file_data));
  }
  func_exfilter_info(editp, fp);
  return TRUE;
}

BOOL func_project_save(FILTER* fp, void* editp, void* data, int* size)
{
  *size = sizeof(project_file_data);
  if (data) {
    memcpy(data, &project_file_data, sizeof(project_file_data));
  }
  return TRUE;
}

// ex filter �����p

void func_exfilter_info(void* editp, FILTER* fparg)
{
  // OutputDebugStringW(L"get filter info");
  // OutputDebugStringW(L"get filter info");
  dbprint("get filter info");
  // bool ret;

  obj* f = new obj(fparg);
  // exedit�Ǝ���filter�̒�`���object�����擾����
  // ���̂��߂ɂ́A�A�A
  // hinst���K�v�Ȃ̂��ȁH
  f->Init();
  EXEDIT_FILTER** exfilters = f->loadfilters;
  EXEDIT_FILTER* exfilter;
  LPCSTR* buff;
  int cnt = 0;
  while (true)
  {
    exfilter = *exfilters;

    if (exfilter == nullptr)
    {
      break;
    }
    buff = &(exfilter->name);

    cnt++;
    exfilters++;
  }


  // dbprint("exedit hinst");
  hex* i = new hex(f->hinst);
  i->dbp("exedit hinst:");
  // dbprint(i.toHex());
  // object_buffer_info��malloc����Ă��炸�A
  // �����ȃf�[�^�������Ă�����ۂ�
  // �ǂ��炩�Ƃ�����SortedObjectTable�̕������₵��
  // objectbuffer��hinst�œǂݍ���dll�̃������͈͊O�ɂȂ��Ă�
  i->value = (int)f->objbuffer;
  i->dbp("objectbuffer pointer:");
  i->value = f->objbuffer->exdata_size;
  i->dbp("datasize:");
  i->value = (int)f->objbuffer->data;
  i->dbp("data pointer:");
  i->value = (int)f->objbuffer->exdata;
  i->dbp("exdata pointer:");

  // ��������Aobj.text�̒l���擾����܂ł����؂���
  // datapointer�̓�����obj.text�̒l�������Ă���
  // ����sortedobjecttable
  // object�ɂ̓��C���[�P��obj�����i�[����ĂȂ��H
  // ���C���[�P��obj�͂ǂ��ɂ������H
  // �ǂ�����Ă������́H
  /*
  if (f->objindex_end == -1)
  {
    // -1���ƃ��C���[�P��obj��object�ɐݒ肳��Ȃ�
    return;
  }*/

  EXEDIT_OBJECT** objs = f->object;
  // for (int j = 0; j <= f->objindex_end; j++)
  while (true)
  {
    if (objs == nullptr)
    {
      break;
    }
    EXEDIT_OBJECT* a = *(objs);
    if (a == nullptr)
    {
      break;
    }
    char* name = a->dispname; // 64���� // �^�C�����C�����text���擾�ł���
    dbprint(name);
    int layer = a->layer_disp;
    int frame_start = a->frame_begin; // �����ʂ��Ă�frame start�ʒu
    int frame_end = a->frame_end;

    // void* objexdata = (void*)(a->exdata_offset); // �K��0
    BYTE exist = a->exists; // ���݂���ꍇ��0x01
    BYTE type = a->type; // text�̏ꍇ��0x01?
    DWORD dataoffset = a->exdata_offset; // ���������₵��
    int filternum = a->GetFilterNum();
    DWORD filteroffset = a->ExdataOffset(0);
    // exfilter 0 ->����t�@�C��
    // exfilter 1 ->�摜�t�@�C��
    // exfilter 2 -> �����t�@�C��
    // ���Ԃ�Aobject_buffer_info�̃|�C���^��dataoffset��������
    // �f�[�^�ɃA�N�Z�X����΂����񂾂Ǝv���񂾂���,,,

    objs++;
  }
  // obj��id�I�Ȃ��̂��Ȃ�
  // �����Ă����΁Aobject pointer��index���ȁH
  // lua script��obj�ɑ΂��� obj.text��ݒ肵�����H
  // ���ƁAobj.text�ɐݒ肳���l�͂Ƃ肠����name�ł悢��
  // ���ۂɂ�filter��text���擾����K�v������̂ł́H
  // filter�\����? obj����filter�\���̂������H
  // -> EXEDIT_FILTER -> EXDATA_USE(exdata_use)
  // ���ʂɍl���ăt�B���^�̒�`�������Ă����Ē��g�̃f�[�^��
  // �ʂ�object�ɕۑ����Ă���Ǝv����񂾂��ǁA�A�A
  // �����Ƀt�B���^���̃f�[�^��ۑ����Ă���̂ł́H

  // filter���������A�f�[�^�T�C�Y���擾�H
  // object_buffer_info��text���͂����Ă�H

  // �Ƃ肠�����Aname�ɍő�64�����܂Ŏ擾�ł���悤�ɂȂ���
  // ���́A�A�A
  // lua����ǂ��Ăяo�����H
  // rikkey module�ł�lua��obj�Ɋ֐���ǉ����Ă�
  // ���z��obj.text�Ƀf�[�^���Z�b�g���鎖

  int ii = lua_gettop(f->luastate);





  // dll�œƎ���data��ێ����Ă���
  // �ǂ��ŕۑ�����Ă��邩�H auslib����dll�̐擪����̃I�t�Z�b�g�Ńf�[�^���擾���Ă���
  // SortedObjectTable��0 �����ݒ肳��Ă��Ȃ�
}

std::string UTF8toSjis(std::string srcUTF8)
{
  //Unicode�֕ϊ���̕����񒷂𓾂�
  int lenghtUnicode = MultiByteToWideChar(CP_UTF8, 0, srcUTF8.c_str(), srcUTF8.size() + 1, NULL, 0);

  //�K�v�ȕ�����Unicode������̃o�b�t�@���m��
  wchar_t* bufUnicode = new wchar_t[lenghtUnicode];

  //UTF8����Unicode�֕ϊ�
  MultiByteToWideChar(CP_UTF8, 0, srcUTF8.c_str(), srcUTF8.size() + 1, bufUnicode, lenghtUnicode);

  //ShiftJIS�֕ϊ���̕����񒷂𓾂�
  int lengthSJis = WideCharToMultiByte(CP_THREAD_ACP, 0, bufUnicode, -1, NULL, 0, NULL, NULL);

  //�K�v�ȕ�����ShiftJIS������̃o�b�t�@���m��
  char* bufShiftJis = new char[lengthSJis];

  //Unicode����ShiftJIS�֕ϊ�
  WideCharToMultiByte(CP_THREAD_ACP, 0, bufUnicode, lenghtUnicode + 1, bufShiftJis, lengthSJis, NULL, NULL);

  std::string strSJis(bufShiftJis);

  delete bufUnicode;
  delete bufShiftJis;

  return strSJis;
}