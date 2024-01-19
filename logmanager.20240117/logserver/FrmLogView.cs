using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.IO;
using System.Runtime.Versioning;


namespace saltstone
{
  public partial class frmLogView : Form
  {
    private DataTable? _tablelog = null;
    private LogServer? lserver = null;

    [SupportedOSPlatform("windows")]
    public frmLogView()
    {
      InitializeComponent();
      pf_init();
    }
    public void pf_init()
    {
      lblMessage.Text = "";

      // Logs.evt_notifygui = displog;
      Globals.main_messagectl = new MsgControl(this.lblMessage, null);
      lserver = LogServer.getInstance();
      //　mainのstatusにlogを出力する
      Logs.init(Globals.main_messagectl);

      // lserver.evt_displog += displog;

      _tablelog = new DataTable();
      DataColumn dc;
      foreach (DataGridViewColumn c in lstLog.Columns)
      {
        dc = new DataColumn(c.Name);
        dc.Caption = c.HeaderText;
        _tablelog.Columns.Add(dc);
        // header textを表示したいのだが、、b
      }
      lstLog.Columns.Clear();
      lstLog.Rows.Clear();
      lstLog.DataSource = _tablelog;
      for (int i = 0; i < lstLog.Columns.Count; i++)
      {
        lstLog.Columns[i].HeaderText = _tablelog.Columns[i].Caption;
      }
    }

    public void displog(Logs l)
    {
      string a = l.message;
      if (_tablelog == null)
      {
        return;
      }
      DataRow dr = _tablelog.NewRow();
      dr[0] = l.logdate;
      dr[1] = l.exename;
      // dr[2] = Logs.getlogtypename(l.logtype);
      dr[3] = l.message;
      _tablelog.Rows.Add(dr);
      if (lstLog.InvokeRequired)
      {

        lstLog.Invoke(new Action(() => { lstLog.AutoResizeColumns(); }));
        return;
      }
      this.lstLog.AutoResizeColumns();
    }

    private void testToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Logs.write("messsageだよー？");
    }

    private void mmftestToolStripMenuItem_Click(object sender, EventArgs e)
    {
      // memoryでlogを作る
      // -> c#のstringがどのようにメモリに保存されるか確認
      // ipclogをシリアライズしてmemoryに保存
      // desirializeしてipclogを復元
      // ipclogをサーバ側queueに登録 -> ログに書き出し
      // byte[] buffer;
      string a = "a";
      MemoryStream ms = new MemoryStream();
      BinaryWriter bw = new BinaryWriter(ms);
      bw.Write(Encoding.UTF8.GetBytes(a)); // 4バイト
      bw.Close();
      byte[] ary = ms.ToArray();
      // ipclogにmemorystreamに書き込むmethodを用意
      // memstreamを受け取り、メンバーをbyte配列に変換してmemstに書き込む
      // ４つ？かな 先頭４つにint32（４バイト）のオフセットを書き込む
      // それから後ろはnull termed stringにする
      // これでserialize/deserializeができる
      // 問題は合計で1024バイトをこえてはいけないこと
      // あとe.stacktrace
      // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.ptrtostructure?view=net-6.0
      // unmanaged memory to managed instance 
      // これでcopyされるっぽいな

      // 課題
      // sharememに保存するstructをいちいち定義するか？
      // 共通化できないか？
      // 先頭に全体サイズ
      // 次のはintの個数 + intのary
      // 次はstringの個数 + string offset
      // string data (null offset)
      // bitmap data

    }
  }
}
