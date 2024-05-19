using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;


// pginiとenvini inifileとの違いは？
// pginiはloadのみ行う
// これはyb_guiのみで使う
// regkeyの取得がメイン
// 追加でかんしくんとぼうよみちゃんのexepathを取得し、iniに保存


namespace saltstone
{
  /// <summary>
  /// saltstone.iniを管理するクラス
  /// inifileをinheritしている
  /// </summary>
  public class PGInifile : Inifile, IDisposable
  {
    public const string settingfile = "saltstone.ini";
    public const string INI_Bouyomichan = "BouyomiChan";
    public const string INI_Forceparser = "forcepser";
    public const string INI_Avipath = "avipath";
    public const string INI_Chardir = "chardir";
    public const string INI_SettingDB = "settingdb";
    public const string INI_CharaDB = "charadb";
    public const string INI_WAV_Lowcut = "wavlowcut";
    public const string INI_WAV_Highcut = "wavhighcut";
    public const string INI_WAV_Threshold = "wavthreshold";
    public const string INI_WAV_Sensitivity = "wavsensitivity";
    public const string INI_Aupdir = "aupdir";
    public const string INI_Voicedir = "voicedir";



    // pg 用のiniファイルを読み込み
    // 各exe(yb,aviutil,かんしくん,wavesampling)のパスを保存
    // yu_guiのlastfileとlastsectionを保存
    public bool load()
    {
      //string execdir = Utils.getexepath("aaa");
      string execdir = Utils.getexecdir();
      string inif = execdir + "\\" + settingfile;
      // pg.iniを読み込む
      // レジストリに登録してwinでglobal化する
      // なければ、デフォルトディレクトリに保存する
      // ほしい情報は
      // -> ぼうよみexeのpath
      // -> かんしくんexeのpath
      // -> aviutil exeのpath
      // yu_guiのでlastfile
      // yb_guiでのlastsection

      /*
      if(Utils.existFile(inif) == false)
      {
          RegistryKey reg;
          reg = Registry.LocalMachine.OpenSubKey(regkey, true);

          // TODO レジストリがない場合は登録
          // TODO レジストリのiniパスが存在しない場合は実行時ディレクトリに作成

          // RegistryKey reg = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(regkey);
          if (reg == null)
          {
              // permissionの問題でcreatesubkeyができない
              // キーはあるのにopwnsubkeyが失敗する
              reg = Registry.LocalMachine.CreateSubKey(regkey);
              if (reg == null)
              {
                  Logs.write("reg key create failed or not found");
                  return false;
              }
          }
          // pginiが設定されているか判定 ない場合はデフォルトディレクトリにsetting.iniで作成する
          //RegistryKey inikey = reg.OpenSubKey(regkeypgini);
          string reginifile = (string)reg.GetValue(regkeypgini);
          bool fexist = File.Exists(reginifile);
          if (string.IsNullOrEmpty(reginifile) || (fexist == false))
          {
              // regにiniファイルの設定がない場合、デフォルト値を設定
              // 実行時ディレクトリのsetting.iniを使用する
              DirectoryInfo curdir = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location);
              string inipath = Path.GetFullPath(curdir.ToString() + "\\" + pginifile);
              reg.SetValue(regkeypgini, (object)inipath);
              reginifile = inipath;
          }

          // ここで別のインスタンスを作成しているからおかしくなる
          // どうするべきかというと、Inifileに対して

          inif = reginifile;
          // 
      }
      */


      Globals.envini = this.load(inif);
      // Inifile ifs = new Inifile(reginifile);
      // Globals.envini  = this;

      // aup.iniの読み込み
      // aviutilのgcmzよりデータを読み込み
      // aviutilが起動していないとデータが取得できない

      if (String.IsNullOrEmpty(this.get("BouyomiChan")))
      {
        // 棒読みちゃんのexeパスが設定されていない
        // MessageBox.Show("棒読みちゃんが起動されていません");
        // logに出力しぼうよみを起動してから再起動するように促す
        // logをどうやって実装するか？
        // System.Windows.Forms.MessageBox.Show("棒読みちゃんのexepathが設定されていません\r\n棒読みちゃんを起動して再実行してください");
        Logs.write("棒読みちゃんのexepathが設定されていません\r\n棒読みちゃんを起動して再実行してください");

        return false;
      }
      // 必須EXEの起動確認 ぼうよみちゃんは必須ではなく、dllに置き換え
      // 起動した場合、実行時パスをiniに保存
      // 棒読みちゃんの起動確認
      //System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName("BouyomiChan");
      //if (procs.Length == 0)
      //{
      //  // this.Log("not run BouyomiChan");
      //  // iniに棒読みは不要 Yb_guiでしか使用しない
      //  // globalsには登録しておきたい
      //  //  execBouyomichan = false;
      //  return false;
      //}
      // pginiで必要なのはboyomiへの実行パスのみ
      // 実際に起動する必要はない
      //string a = "BouyomiChan ProcessID[" + procs[0].Id.ToString() + "]";
      //Logs.write(a);
      //// this.Log(a);
      //// execBouyomichan = true;
      //// 棒読みちゃんのexeのパスを取得
      //System.Diagnostics.Process p = procs[0];
      //// this.Log(p.Modules[0].FileName);
      //Globals.envini.settings["BouyomiChan"] = p.Modules[0].FileName;

      // かんしくんの実行パス
      System.Diagnostics.Process[] procs;
      System.Diagnostics.Process p;

      procs = System.Diagnostics.Process.GetProcessesByName("forcepser");
      if (procs.Length != 0)
      {
        p = procs[0];
        Globals.envini.settings["forcepser"] = p.Modules[0].FileName;
      }

      // string a = YukkuriBatch.Uty.getexepath("a");
      string avipath = Utils.Sysinfo.getexepath("aviutl");
      //string avipath = Uty.getexepath("aviutl");
      if (String.IsNullOrEmpty(avipath) == false)
      {
        Globals.envini.set("avipath", avipath);
      }
      return true;
    }

    // ぼうよみちゃんの実行をどこでやるか？
    // このクラスでやるべき？　データはiniファイルにそろってるが、、、



  }
}
