// saltstoneimage.cpp : アプリケーションのエントリ ポイントを定義します。
//

#include "framework.h"
#include "saltstoneimage.h"
// #include "sqlite3.h" //sqlite3にはaccessしない incmd.txtで全部指定する
// #include "resource.h"
#include <gdiplus.h>
#include "CPicturebox.h"
#include "CSBitmap.h"
#include "CSSInputcommand.h"
// #define _AFXDLL
// #include <afxwin.h>


// outputをどうするか？
// 最終的にはsharememにするが、
// テスト的にはどうする？
// cacheへの保存もほしい

// share memよりcmdを受け取り、bitmap演算してsharememに書き込む
// -> cinputcommandを受け取り、bitmap演算するクラスがほしい
// それをpictureboxにdrawする
// csbitmapがよいかな

// outをどうするか？
// aviuからはsharememでアクセス -> dll -> aviu_script
// imageprocess -> c#control -> dll -> aviu_script


// incmdの指定でoutが指定されている場合、合成した画像をout=fnameに出力する


// aviuのdllで渡すときはraw data raster bitmap bgra
// https://aviutlscript.wiki.fc2.com/wiki/obj.getpixeldata%2C%20obj.putpixeldata
// bitmap の pixcel argbでいいかもね


#define MAX_LOADSTRING 100
#define SIGNATURE_NUM 8 // pngのsignatureバイト数
INT_PTR CALLBACK DlgWndProc(HWND hWnd, UINT msg, WPARAM wp, LPARAM lp);
bool paintpicture(HWND hWnd);
bool mergeimage();
bool paintred(HWND hWnd);


// test用事 pictureboxの再描画 wm_paintで実装するためフラグにする
bool pctpaintred;

const int WM_USER_DIALOG = 0x0400+100;
const int MSG_PAINT_PICTURE = 0x0010U;


GdiplusStartupInput gdiSI; // gdi+を使用するために必要なもの
ULONG_PTR gdiToken;
// HBITMAP pctbitmap;
// Image* pctimage;
// HWND dialoghwnd; // debug picturebox dialog

// グローバル変数:
HINSTANCE hInst;                                // 現在のインターフェイス
WCHAR szTitle[MAX_LOADSTRING];                  // タイトル バーのテキスト
WCHAR szWindowClass[MAX_LOADSTRING];            // メイン ウィンドウ クラス名
CPicturebox* ctlpicturebox; // picturebox ctlのwrap class
// bitmap raw byteを格納するメモリ and class
//Bitmap* dstbmp;
CSSBitmap* dstbmp;
CSSInputcommand* cinputcmd;


// どーやって raster byte arrayを保管するか？
// pictureboxへのdrawはbyte arrayをコピーしてwm_paintでdrawする





// このコード モジュールに含まれる関数の宣言を転送します:
ATOM                MyRegisterClass(HINSTANCE hInstance);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);
// CString imgdst; // 重ね合わせる画像の元 バック
// CString imgsrc; // 重ね合わせるフロントの画像

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    size_t readSize;
    png_byte signature[8];

    GdiplusStartup(&gdiToken, &gdiSI, NULL); // gdi+を使用するために必要なもの


    cinputcmd = nullptr;
    dstbmp = nullptr;
    if (lpCmdLine != NULL && lpCmdLine[0] != 0) {
      // input command (incmd.txt)が指定されたと判断
      // これにより合成処理を行う
      // TODO　通常はbackgroundで実行しておき、share mem & semaphore & pipeでオンメモリで合成処理をお行う
      // CString buff = CString(lpCmdLine);
      cinputcmd = new CSSInputcommand();
      cinputcmd->parse(lpCmdLine);
      // std::list<std::wstring>::const_iterator pctptr = cinputcmd->getPctlists()->begin();

      // buffにはinput command.txtが設定されている for debug
      // ファイルが存在するかチェックし、解析を行う
      
      //  int i = buff.Find(L" ");
      //if (i == 0) {
      //  return false; // 空白を含んでいない場合は引数なしと判断
        // バックグランドで実行させておき、セマフォ・共有メモリにて画像合成処理を行う
      //}
      // imgdst = CString(lpCmdLine);
      // FILE* fh;
      // errno_t err = _tfopen_s(&fh, imgdst, _T("rb"));
      // FILE* fh = _tfopen_s(imgdst, _T("rb"));
      // readSize = fread(signature, 1, SIGNATURE_NUM, fh);
      // if (png_sig_cmp(signature, 0, SIGNATURE_NUM)) {
      //   printf("png_sig_cmp error!\n");
      //   return -1;
      // }
      // fclose(fh);
      //sqlite3* db = nullptr;
      // const char* db_name = "chara.db";
      // int ret = sqlite3_open(db_name, &db);
      // ret = sqlite3_close(db);
      //  dstbmp = new Bitmap(srcfname);
      dstbmp = new CSSBitmap();
      dstbmp->mergeimagefiles(cinputcmd);
      dstbmp->save(cinputcmd->outfile);

      if (cinputcmd->debugmode == false)
      {
        GdiplusShutdown(gdiToken);
        return 0; // wWinmainのret -> exit codeになるのでは？
      }

    }

    


    // グローバル文字列を初期化する
    LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadStringW(hInstance, IDC_SALTSTONEIMAGE, szWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);

    // アプリケーション初期化の実行:
    if (!InitInstance (hInstance, nCmdShow))
    {
        return FALSE;
    }

    HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_SALTSTONEIMAGE));

    MSG msg;

    // メイン メッセージ ループ:
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }

    return (int) msg.wParam;
}



//
//  関数: MyRegisterClass()
//
//  目的: ウィンドウ クラスを登録します。
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEXW wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = 0;
    wcex.hInstance      = hInstance;
    //wcex.hIcon          = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_SALTSTONEIMAGE));
    wcex.hCursor        = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName   = MAKEINTRESOURCEW(IDC_SALTSTONEIMAGE);
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassExW(&wcex);
}

//
//   関数: InitInstance(HINSTANCE, int)
//
//   目的: インスタンス ハンドルを保存して、メイン ウィンドウを作成します
//
//   コメント:
//
//        この関数で、グローバル変数でインスタンス ハンドルを保存し、
//        メイン プログラム ウィンドウを作成および表示します。
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
  setlocale(LC_ALL, "ja_JP.utf8");

   hInst = hInstance; // グローバル変数にインスタンス ハンドルを格納する



   //HWND hWnd = CreateWindowW(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
   //   CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, nullptr, nullptr, hInstance, nullptr);

   //if (!hWnd)
   //{
   //   return FALSE;
   //}

   //ShowWindow(hWnd, nCmdShow);
   //UpdateWindow(hWnd);

   // IDD_ABOUTBOX a;
   //   CreateDialog(hInstance, MAKEINTRESOURCE(IDD_ImageViwer), hWnd, NULL);z
   // メインウィンドウはないため、hwndはnullにする
   HWND dialoghwnd = CreateDialog(hInstance, MAKEINTRESOURCE(IDD_DIALOGIMG), nullptr, (DLGPROC)DlgWndProc);

   ShowWindow(dialoghwnd, SW_SHOW);
   SendMessage(dialoghwnd, WM_USER, MSG_PAINT_PICTURE, 0);

   ctlpicturebox = new CPicturebox();
   ctlpicturebox->sethinst(hInstance);
   ctlpicturebox->sethwnd(GetDlgItem(dialoghwnd, IDC_PICTURE));

   // ここでdst（元)picを読み込んでおく
   // それをpictureboxに表示させる
   // VC++ bitmap? cbitmap? image?
   // const wstring* srcfname = new wstring( L"C:\\Users\\fuuna\\Videos\\charas\\れいむ\\体\\00.png");
  //   Bitmap* dstbmp = new Bitmap(srcfname);


   // init instanceでin cmdより画像合成を行う

   // TODO フラグを管理し、debug onならpictureboxを表示

   return TRUE;
}

//
//  関数: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  目的: メイン ウィンドウのメッセージを処理します。
//
//  WM_COMMAND  - アプリケーション メニューの処理
//  WM_PAINT    - メイン ウィンドウを描画する
//  WM_DESTROY  - 中止メッセージを表示して戻る
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
    case WM_COMMAND:
        {
            int wmId = LOWORD(wParam);
            // 選択されたメニューの解析:
            switch (wmId)
            {
            case IDM_ABOUT:
                DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
                break;
            case IDM_EXIT:
                DestroyWindow(hWnd);
                break;
            default:
                return DefWindowProc(hWnd, message, wParam, lParam);
            }
        }
        break;
    case WM_PAINT:
        {
            // PAINTSTRUCT ps;
            // HDC hdc = BeginPaint(hWnd, &ps);
            // TODO: HDC を使用する描画コードをここに追加してください...
            // EndPaint(hWnd, &ps);
        }
        break;
    case WM_DESTROY:
        if (ctlpicturebox != nullptr) {
          ctlpicturebox->Dispose();
        }
        if (dstbmp != nullptr) {
          dstbmp->Dispose();
          dstbmp = NULL;
        }
        GdiplusShutdown(gdiToken);
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

// バージョン情報ボックスのメッセージ ハンドラーです。
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}

INT_PTR CALLBACK DlgWndProc(HWND hWnd, UINT msg, WPARAM wp, LPARAM lp) {
  INT_PTR ret = FALSE;
  bool fret = false;
  switch (msg)
  {
  case WM_USER:
    if (wp != MSG_PAINT_PICTURE) {
      break;
    }
    /*
    fret = paintpicture(hWnd);
    ret = FALSE;
    if (fret = true) {
      ret = TRUE;
    }
    */

    break;
  case WM_COMMAND:
    switch (wp) {
    case IDC_OK:
      pctpaintred = true;
      // MessageBox(hWnd, _T("IDOK"), 0, 0);
      // https://learn.microsoft.com/ja-jp/windows/win32/gdi/invalidating-the-client-area
      // InvalidateRect
      // imageをマージする
      // fret = mergeimage();
      // if (fret == false) {
      //   break;
      // }
      /*
      HWND pctWnd = NULL;
      pctWnd = GetDlgItem(dialoghwnd, IDC_PICTURE);
      // InvalidateRect(hWnd, &rc, TRUE);
      UpdateWindow(pctWnd);
      */

      ret = TRUE;
      // DestroyWindow(hWnd);
      // PostQuitMessage(0); // DestroyWindowのretとして使用する
      break;
    case IDCANCEL:
      // MessageBox(hWnd, _T("IDCANCEL"), 0, 0);
      if (ctlpicturebox != nullptr)
      {
        ctlpicturebox->Dispose();
      }
      if (dstbmp != nullptr) {
        dstbmp->Dispose();
        dstbmp = NULL;
      }

      DestroyWindow(hWnd);
      PostQuitMessage(0);  // DestroyWindowのretとして使用する
      ret = TRUE;
      break;
    }
    break;
  case WM_CLOSE:
    if (ctlpicturebox != nullptr)
    {
      ctlpicturebox->Dispose();
    }
    if (dstbmp != nullptr) {
      dstbmp->Dispose();
      dstbmp = NULL;
    }

    DestroyWindow(hWnd);
    PostQuitMessage(0);  // DestroyWindowのretとして使用する
    ret = TRUE;
    break;
  case WM_PAINT:
    // if (pctpaintred == true) {
    //  paintred(hWnd);
    //  break;
    // }
    fret = paintpicture(hWnd);

  }
  return ret;
}


// dialog window WM_PAINTからcall
// 最初のimageをpictureboxに描画
bool paintpicture(HWND hWnd)
{
  /*
  char buf[10000];
  HANDLE h;
  WIN32_FIND_DATA find;
  PVOID OldValue = NULL;
  Wow64DisableWow64FsRedirection(&OldValue);
  CString sfname;

  memset(buf, 0, sizeof(buf));
  const wchar_t* searchdir = L"C:\\Users\\fuuna\\Videos\\charas\\れいむ\\*";
  
  h = FindFirstFile(searchdir, &find);
  DWORD dw = GetLastError();
  sfname = find.cFileName;
  bool sfret = false;
  while (1)
  {
    sfret = FindNextFile(h, &find);
    if (sfret == false) {
      break;
    }
    sfname = find.cFileName;
  }
  FindClose(h);
  */

  if(dstbmp == nullptr)
  { 
    return false;
  }

  HWND dhWnd = ctlpicturebox->gethwnd();



  PAINTSTRUCT ps;
  //  HDC hdc = BeginPaint(pctWnd, &ps);
  HDC hdc = BeginPaint(dhWnd, &ps);

  Graphics graphics(hdc);

  // 画像の読み込み「image」は変数名。
  // Image image(fname);
  // int ih = image.GetHeight();
  // int iw = image.GetWidth();
  int ih = dstbmp->getHeight();
  int iw = dstbmp->getWidth();

  // 画像の描画。 ダミー変数 graphics を仲介して描画する必要がある.
  // graphics.DrawImage(&image, 0, 0, iw, ih);
  Bitmap* p = dstbmp->getBitmap();
  graphics.DrawImage(p, 0, 0, iw, ih);
  // imgをglobalに保存しておく
  EndPaint(dhWnd, &ps);
  ReleaseDC(dhWnd, hdc);



  // SendMessage(pctWnd, STM_SETIMAGE, IMAGE_BITMAP, (LPARAM)pctbitmap);

  return true;
}

/*
bool paintred(HWND hWnd)
{
  // hwndはdialogのhwnd ここからpictureboxのhwndを取得
  // HWND pctWnd = GetDlgItem(dialoghwnd, IDC_PICTURE);
  PAINTSTRUCT ps;
  HDC hdc = BeginPaint(ctlpicturebox->gethwnd(), &ps);
  // const Color clrc = Color::MakeARGB(255, 255, 0, 0); // black?
  const Color clrc = Color::MakeARGB(255, 0, 0, 0); // black?
  // alpha 0=透明
  // bitmapなので、transparnetはなしになる　なのでblackになってしまう
  // bmpじゃなく、transparent可能なpng or gifにする必要がある1
  Graphics graphics(hdc);

  // graphics.Clear(Color::White);
  //

  // byteのptr0を検査


  EndPaint(ctlpicturebox->gethwnd(), &ps);
  ReleaseDC(ctlpicturebox->gethwnd(), hdc);

  return true;


}
*/