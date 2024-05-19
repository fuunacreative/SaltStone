using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security;


// ref https://blog.sorairo.pictures/entry/2012/10/21/drop-files-from-code/


namespace saltstone
{
  public class aviutilcoop
  {


    const int WM_DROPFILES = 0x0233;


    // [DllImport("user32.dll")]
    // public static extern int SendMessageW(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

    // sendは待機、postは投げっぱなし
    // [SuppressUnmanagedCodeSecurity, DllImport("user32")]
    // static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    [SuppressUnmanagedCodeSecurity, DllImport("user32")]
    static extern bool PostMessageW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    // SendMessage is a define of SendMessageA or SendMessageW according to whether 
    // UNICODE is defined or not.There is no such *function* as SendMessage
    // http://www.verycomputer.com/5_f01c6e8931856619_1.htm


    // 普通にdropしたときのテスト
    // gcmzでx軸(current frame)はどうしてるんだろ？　drop しても currnet frameは変化しない
    // zoomしないとXとinsert frameが微妙になる。だから拡大してるんだ
    // プロパティwinを非表示にできないかな？
    // TODO currnet frmaeのxを求めているはずだが、、、

    // sequential -> 順番に並ぶ
    // charset ansi -> utf8?

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    //must specify a layout
    struct POINT
    {
      public Int32 X;
      public Int32 Y;
    }
    // DROPFILES' はアンマネージ構造体としてマーシャリングできません。有効なサイズ、またはオフセットの計算ができません。'

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    class DROPFILES
    {
      public Int32 size;
      public POINT pt;
      public Int32 fND;
      public Int32 WIDE;
    }


    public static void test()
    {
      Form f = new Form();
      IntPtr hwnd = f.Handle;
      DROPFILES d = new DROPFILES();
      d.size = Marshal.SizeOf(typeof(DROPFILES));
      d.pt = new POINT();
      d.pt.X = 50;
      d.pt.Y = 50;
      d.fND = 1; // クライアント領域かどうか  true = 1 0 = false
      d.WIDE = 1; // wide charactor maybe UTF16

      // string file = @"C:\Users\fuuna\source\saltstone\resource\icon_export.png" + "\0\0"; // \0\0 is terminator
      string file = @"C:\Users\fuuna\Videos\test.exo" +"\0\0";
      // 複数ファイルを送る場合は間に"\0"１バイトをいれる
      Encoding e = Encoding.Unicode;
      byte[] filebyte = e.GetBytes(file);
      IntPtr wparam = IntPtr.Zero;
      try
      {
        wparam = Marshal.AllocHGlobal(Marshal.SizeOf<DROPFILES>() + filebyte.Length);
        Marshal.StructureToPtr(d, wparam, true);
        Marshal.Copy(filebyte, 0, wparam + d.size, filebyte.Length);
        // Marshal.Copy(filebyte, d.size, wparam, filebyte.Length);

        // source , start index(src),dest(wparam)

        // hwndはnotepad固定にする
        hwnd = (IntPtr)0x00040386; // notepad exwin 00040386
        PostMessageW(hwnd, WM_DROPFILES, wparam, IntPtr.Zero);

        // binaryが直接pasteされる
        // pngはpaset ok
        // exoもOK
        // layer , start位置もexoで制御できる もちろん current_frameも
        // じゃあ、currnet frameはどうやって判断してる？
        // sdkよりget_frameで取得 -> exoのstartに設定

        // とりま、セリフ入力まで持っていければいいな


      }
      finally
      {
        if (wparam == IntPtr.Zero)
        {
          Marshal.FreeHGlobal(wparam);
        }
      }
    }




  }
}

/*
 
  // ここでaviuにsendmessageを送り、drop処理を行っている
	if _, err := sendMessage(windows.Handle(window), wmCopyData, uintptr(getConsoleWindow()), uintptr(unsafe.Pointer(cds))); err != nil {
		L.RaiseError("ごちゃまぜドロップスの外部連携API呼び出しに失敗しました: %v", err)
	}
-> gcmzのsendmessageをcallしている


	const wmCopyData = 0x4A
	cds := &copyDataStruct{
		Data: 0,
		Size: uint32(len(str) * 2),
		Ptr:  uintptr(unsafe.Pointer(&str[0])),
	}


func sendMessage(hwnd windows.Handle, uMsg uint32, wParam uintptr, lParam uintptr) (lResult uintptr, err error) {
	r0, _, e1 := syscall.Syscall6(procSendMessageW.Addr(), 4, uintptr(hwnd), uintptr(uMsg), uintptr(wParam), uintptr(lParam), 0, 0)
	lResult = uintptr(r0)
	if e1 != 0 {
		err = e1
	}
	return
}


getConsoleWindowって何？

wParam
A handle to the window passing the data.
lParam
A pointer to a COPYDATASTRUCT structure that contains the data to be passed.

たぶん、かんしくんのconsole winのことじゃないかな？
データの送信側のウィンドウハンドル

cdsには parameter(str)が入っている
	buf := make([]byte, 0, 64)
	buf = append(buf, strconv.Itoa(layer)...)
	buf = append(buf, 0x00)
	buf = append(buf, strconv.Itoa(frameAdv)...)

	n := files.MaxN()
	for i := 1; i <= n; i++ {
		buf = append(buf, 0x00)
		buf = append(buf, filepath.Join(dir, files.RawGetInt(i).String())...)
	}
frame advは挿入後に進むフレームの相対数 integer?


  MinZoomLevel = 19;
    SetZoomLevel(MinZoomLevel);

          SendMessage(FExEdit^.Hwnd, WM_VSCROLL, MAKELONG(SB_THUMBTRACK, SI.nPos), VScrollBar);
          SendMessage(FExEdit^.Hwnd, WM_VSCROLL, MAKELONG(SB_THUMBPOSITION, SI.nPos), VScrollBar);

    SendMessage(FWindow, WM_GCMZDROP, 10, {%H-}LPARAM(@GDDI));
-> 
              if not FAPILua.CallDropSimulated(PDDI^.DDI^.Files, PDDI^.DDI^.Point, PDDI^.DDI^.KeyState, PDDI^.FrameAdvance) then
                raise EAbort.Create('canceled ondropsimulated');

pddi = custom structure PDragDropInfo
  TDragDropInfo = record
    Point: TPoint;
    KeyState: DWORD;
    Files: TFiles;
    Effect: DWORD;
  end;


EmulateDrop util.pas
  HGlobal := GlobalAlloc(GMEM_ZEROINIT, SizeOf(TDropFiles) + Len);
  PDF := GlobalLock(HGlobal);
  try
    PDF^.pFiles := SizeOf(TDropFiles);
    PDF^.fWide := True;
    PDF^.fNC := False;
    PDF^.pt.x := Point.x;
    PDF^.pt.y := Point.y;
    ScreenToClient(Window, PDF^.pt);
    P := PByte(PDF);
    Inc(P, SizeOf(TDropFiles));
    for I := Low(WS) to High(WS) do
    begin
      Move(WS[I][1], P^, Length(WS[I]) * SizeOf(widechar));
      Inc(P, (Length(WS[I]) + 1) * SizeOf(widechar));
    end;
  finally
    GlobalUnlock(HGlobal);
  end;
  // ここでデータをdrop処理 exwindowにdropメッセージを送る
  // hglobalはファイル名だな
  SendMessageW(Window, WM_DROPFILES, WPARAM(HGlobal), 0);

    SetZoomLevel(OldZoom);

やはり、wm_dropfilesのsendmessageをしてるだけだな
問題は、mouse x,yからlayerの計算をしなきゃいけないこと
なぜzoomをいじるのか？

かんしくんのemulatedropをコールしたほうがいいかもね
  まわりくどくてめんどくさすぎる
  -> かんしくんはgoで作られており、luaのsendfileしか利用できない

	if _, err := sendMessage(windows.Handle(window), wmCopyData, uintptr(getConsolewinWindow()), uintptr(unsafe.Pointer(cds))); err != nil {

これがいいな
 pointerを計算しなくていい -> すぐにできる ただしgcmzが必須
wparam -> drop 元のwindow handle
cds
->
	cds := &copyDataStruct{
		Data: 0,
		Size: uint32(len(str) * 2),
		Ptr:  uintptr(unsafe.Pointer(&str[0])),
	}
strunct 	type copyDataStruct struct {
		Data uintptr
		Size uint32
		Ptr  uintptr
	}

str ->
	buf := make([]byte, 0, 64)
	buf = append(buf, strconv.Itoa(layer)...)
	buf = append(buf, 0x00)
	buf = append(buf, strconv.Itoa(frameAdv)...)

	n := files.MaxN()
	for i := 1; i <= n; i++ {
		buf = append(buf, 0x00)
		buf = append(buf, filepath.Join(dir, files.RawGetInt(i).String())...)
	}	
str := utf16.Encode([]rune(string(buf)))


explorerからdropしたときはutf16だったわ


*/
