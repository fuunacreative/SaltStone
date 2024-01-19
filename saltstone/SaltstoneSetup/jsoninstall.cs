using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace saltstone
{
  public class jsoninstall
  {
    // jsonファイルを使いインストールを行うクラス

    public string jsonfile;
    public string url;

    public jsoninstall(string argurl)
    {

      url = argurl;
      // urlからjsonファイル名を取得
      // web classを作るべきかもな downloadとか
      // html classを使用する
      jsonfile = html.getFilenameFromUrl(url);

      // check urlがjsonファイルかどうか
      if (jsonfile.Contains(".json") == false)
      {
        jsonfile = "";
        return;
      }
    }

    public static bool install(ref program p)
    {
      // json readが行われている前提
      // なぜわけるかというと、将来的にすべてのpgをjsonに変更するため
      // gui側でどのpgをインストールするのか判断できるようにするため

      return true;
    }

    // jsonのinstall aryを読み込み
    public bool read(ref List<program> pgs)
    {
      if (jsonfile == null || jsonfile.Length == 0)
      {
        return false;
      }
      if (pgs == null)
      {
        pgs = new List<program>();
      }
      pgs.Clear();

      // urlよりjsonをdw
      html insthtm = new html(url);
      // 最初にinstall.htmlを読み込みディレクトリ設定を取得している
      // つまりこの時点ではtempは決まっていない
      // string tempfile = Globals.tempdir + "\\" + jsonfile;
      // c# library create tempfileを使用する
      string tempfile = Utils.Files.createTemp();
      try
      {
        insthtm.webDownloadFile(url, tempfile);

        string buff = Utils.readAllText(tempfile);
        // jsonのinstall aryを読み込み
        dynamic data = Json.Decode(buff);
        dynamic pditem = data.installer;
        program p;
        foreach (dynamic pg in pditem)
        {
          p = new program();
          pgs.Add(p);
          p.id = pg.id;
          p.parentid = pg.parentid;
          p.dispname = pg.dispname;
          p.dispflag_str = pg.dispflag;
          p.url = pg.url;
          p.downloadurl = pg.downloadurl;
          p.xpath = pg.xpath;
          p.file = pg.filename;
          p.setups_str = Json.Encode(pg.setups);
          p.version = pg.version;
          p.installflag_str = pg.intallflag;
          p.memo = pg.memo;
          p.openurl = pg.openurl;
        }
        // json -> class -> class member setup
        // get gg apikey
        string apikey_src = SaltstoneSetup.Properties.Resources.googleapi;
        byte[] buffer = Convert.FromBase64String(apikey_src);
        string apikey = Encoding.UTF8.GetString(buffer);

        foreach (program pitem in pgs)
        {
          pitem.exefile = "";
          pitem.subdir = "";
          pitem.name = pitem.dispname;
          pitem.dispflag = true;
          if (pitem.dispflag_str == "false")
          {
            pitem.dispflag = false;
          }
          // google apiキーのdecode + replace
          buff = pitem.url;
          pitem.url = buff.Replace("[googleapikey]", apikey);

          if (pitem.downloadurl == "same")
          {
            pitem.downloadurl = pitem.url;
          }
          // setupsはそのまま文字列としてsetupsに保存して、後ろのループで処理したいが、、、
          // ここでやるしかないかな？
          // dynamic setups = pg.setups;
          dynamic setups = Json.Decode(pitem.setups_str);
          foreach (dynamic setup in setups)
          {
            // {"cmd":"exe","file","vcredist2005_x64.exe"}
            buff = setup.cmd;
            switch (buff)
            {
              case "exe":
                // p.installfile = setup.file;
                // pitem.exefile = setup.file; htmlのexe or install実行時に入るexe
                InstallMethod inm = new InstallMethod();
                inm.cmd = InstallMethod.enum_cmd.exe;
                inm.exefile = setup.file;
                if (pitem.installmethods == null)
                {
                  pitem.installmethods = new List<InstallMethod>();
                }
                pitem.installmethods.Add(inm);
                inm = null;
                break;
            }
          }

          pitem.installflag = true;
          if (pitem.installflag_str == "false")
          {
            pitem.installflag = false;
          }



        }
      } finally
      {
        Utils.Files.delete(tempfile);

      }

      return true;

      // aryをinstall classに展開
      // jsonでのurlには[google api]が含まれているので置換する
    }



  }

}
