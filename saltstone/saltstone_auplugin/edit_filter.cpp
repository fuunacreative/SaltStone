//----------------------------------------------------------------------------------
//		SaltStone plugin for AviUtl ver0.99i以降
//----------------------------------------------------------------------------------
#include "pch.h"

#define	WINDOW_W		320
#define	WINDOW_H		240

EXFUNC* exfunc;

//---------------------------------------------------------------------
//		フィルタ構造体定義
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
  "SaltStone version 0.10 by Fuuna", // filter information flag_ex_informatinが設定されている場合に設定可
  NULL, // func_save_start
  NULL, // func_save_end
  NULL, //exfunc
  NULL, // hwnd
  NULL, // dll_hinst
  NULL, // ex_data_def
  NULL, // func_is_saveframe
  func_project_load, // projectに保存したデータがない場合にはcallされない
  func_project_save, // projectに保存したデータがない場合にはcallされない
};


//---------------------------------------------------------------------
//		フィルタ構造体のポインタを渡す関数
//---------------------------------------------------------------------
EXTERN_C FILTER_DLL __declspec(dllexport)* __stdcall GetFilterTable(void)
{
  return &filter;
}




//---------------------------------------------------------------------
//		表示
//---------------------------------------------------------------------
void disp(HWND hwnd, FILTER* fp, void* editp, int n, int l, int w, int h)
{
  // HDC					hdc;
  // RECT				rc;
  // BITMAPINFO			bmi;
  // TCHAR				b[MAX_PATH];
  static TCHAR* inter_txt[] = {
    "","反転","奇数","偶数","二重化","自動"
  };

  if (!fp->exfunc->is_filter_window_disp(fp)) return;



  /*
  GetClientRect(hwnd,&rc);

  //	フレームの表示
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

  //	タイトルバーの表示
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
//		スクロールバーの設定
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
  //	TRUEを返すと全体が再描画される

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
      if( fp->exfunc->is_editing(editp) != TRUE ) break;	//	編集中でなければ終了
        switch(LOWORD(wparam)) {


        case MID_EDIT_FILE_INFO:
          fp->exfunc->get_file_info(editp,&fi);
          ZeroMemory(txt,sizeof(txt));
          if( fi.flag&FILE_INFO_FLAG_VIDEO ) {
            wsprintf(txt,"ファイル名 : %s\nサイズ : %dx%d\nフレームレート : %d.%03dfps",
              fi.name,
              fi.w,
              fi.h,
              (int)((double)fi.video_rate/fi.video_scale),
              (int)((double)fi.video_rate*1000/fi.video_scale)%1000
            );
          }
          ZeroMemory(txt2,sizeof(txt2));
          if( fi.flag&FILE_INFO_FLAG_AUDIO ) {
            wsprintf(txt2,"\nサンプリングレート : %dkHz\nチャンネル数 : %dch",
              fi.audio_rate,
              fi.audio_ch
            );
          }
          lstrcat(txt,txt2);
          MessageBox(hwnd,txt,"ファイルの情報",MB_OK);
          break;
      }
      break;
    */
    //	スクロールバー制御
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
    // aupファイルの保存場所を取得し設定ファイルに保存
    // 設定ファイルはplugindirへ保存したい
    func_exfilter_info(editp, fp);
    // 選択されているlayerが1だから layer1のobjしか取得できないのでは？
    break;
    /*
    case WM_FILTER_UPDATE:

      if( fp->exfunc->is_editing(editp) != TRUE ) break;	//	編集中でなければ終了
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
//		プロジェクトファイルへのデータ保存サンプル
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

// ex filter 調査用

void func_exfilter_info(void* editp, FILTER* fparg)
{
  // OutputDebugStringW(L"get filter info");
  // OutputDebugStringW(L"get filter info");
  dbprint("get filter info");
  // bool ret;

  obj* f = new obj(fparg);
  // exedit独自のfilterの定義よりobject情報を取得する
  // そのためには、、、
  // hinstが必要なのかな？
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
  // object_buffer_infoはmallocされておらず、
  // 無効なデータが入っているっぽい
  // どちらかというとSortedObjectTableの方があやしい
  // objectbufferがhinstで読み込んだdllのメモリ範囲外になってる
  i->value = (int)f->objbuffer;
  i->dbp("objectbuffer pointer:");
  i->value = f->objbuffer->exdata_size;
  i->dbp("datasize:");
  i->value = (int)f->objbuffer->data;
  i->dbp("data pointer:");
  i->value = (int)f->objbuffer->exdata;
  i->dbp("exdata pointer:");

  // ここから、obj.textの値を取得するまでを検証する
  // datapointerの内部にobj.textの値が入っている
  // 次はsortedobjecttable
  // objectにはレイヤー１のobjしか格納されてない？
  // レイヤー１のobjはどこにいった？
  // どうやってさがすの？
  /*
  if (f->objindex_end == -1)
  {
    // -1だとレイヤー１のobjはobjectに設定されない
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
    char* name = a->dispname; // 64文字 // タイムライン上のtextを取得できる
    dbprint(name);
    int layer = a->layer_disp;
    int frame_start = a->frame_begin; // 動画を通してのframe start位置
    int frame_end = a->frame_end;

    // void* objexdata = (void*)(a->exdata_offset); // 必ず0
    BYTE exist = a->exists; // 存在する場合は0x01
    BYTE type = a->type; // textの場合は0x01?
    DWORD dataoffset = a->exdata_offset; // こいつがあやしい
    int filternum = a->GetFilterNum();
    DWORD filteroffset = a->ExdataOffset(0);
    // exfilter 0 ->動画ファイル
    // exfilter 1 ->画像ファイル
    // exfilter 2 -> 音声ファイル
    // たぶん、object_buffer_infoのポインタにdataoffsetをたして
    // データにアクセスすればいいんだと思うんだけど,,,

    objs++;
  }
  // objのid的なものがない
  // しいていえば、object pointerのindexかな？
  // lua scriptのobjに対して obj.textを設定したい？
  // あと、obj.textに設定される値はとりあえずnameでよいが
  // 実際にはfilterのtextを取得する必要があるのでは？
  // filter構造体? obj毎にfilter構造体を持つか？
  // -> EXEDIT_FILTER -> EXDATA_USE(exdata_use)
  // 普通に考えてフィルタの定義だけしておいて中身のデータは
  // 別にobjectに保存していると思われるんだけど、、、
  // ここにフィルタ毎のデータを保存しているのでは？

  // filterを検索し、データサイズを取得？
  // object_buffer_infoにtextがはいってる？

  // とりあえず、nameに最大64文字まで取得できるようになった
  // 次は、、、
  // luaからどう呼び出すか？
  // rikkey moduleではluaのobjに関数を追加してた
  // 理想はobj.textにデータをセットする事

  int ii = lua_gettop(f->luastate);





  // dllで独自にdataを保持している
  // どこで保存されているか？ auslibだとdllの先頭からのオフセットでデータを取得している
  // SortedObjectTableは0 何も設定されていない
}

std::string UTF8toSjis(std::string srcUTF8)
{
  //Unicodeへ変換後の文字列長を得る
  int lenghtUnicode = MultiByteToWideChar(CP_UTF8, 0, srcUTF8.c_str(), srcUTF8.size() + 1, NULL, 0);

  //必要な分だけUnicode文字列のバッファを確保
  wchar_t* bufUnicode = new wchar_t[lenghtUnicode];

  //UTF8からUnicodeへ変換
  MultiByteToWideChar(CP_UTF8, 0, srcUTF8.c_str(), srcUTF8.size() + 1, bufUnicode, lenghtUnicode);

  //ShiftJISへ変換後の文字列長を得る
  int lengthSJis = WideCharToMultiByte(CP_THREAD_ACP, 0, bufUnicode, -1, NULL, 0, NULL, NULL);

  //必要な分だけShiftJIS文字列のバッファを確保
  char* bufShiftJis = new char[lengthSJis];

  //UnicodeからShiftJISへ変換
  WideCharToMultiByte(CP_THREAD_ACP, 0, bufUnicode, lenghtUnicode + 1, bufShiftJis, lengthSJis, NULL, NULL);

  std::string strSJis(bufShiftJis);

  delete bufUnicode;
  delete bufShiftJis;

  return strSJis;
}