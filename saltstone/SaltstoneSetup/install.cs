using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;

namespace saltstone
{
  public class install : IDisposable
  {
    public const string Install_HTML = "https://fuunacreative.github.io/SaltStone/install.html";
    public const string Aviutl_INIFILE = "aviutl.ini";
    // public const string Aviutl_SUBDIR = "aviutl";
    public const string INI_VoiceSUBDIR = "voide";
    // public const string INI_TEMPDIR = @"C:\Users\yasuhiko\temp";
    public const string INI_PGDIR = "program";


    public string destdir;
    public string tempdir;
    public string aviutldir = ""; // aviutl用のフォルダ
    html htmldoc;
    public string html_pgver; // install.html上のinstaller ver

    public frmInstall mainform;

    public List<program> programs;
    public List<charaurl> charaurls;
    public Dictionary<string, voiceDefines> voiceurls;
    public Dictionary<string, directory> directorys;

    // gui側にmessegeを表示するfunc progressbarのperも服務
    public delegate void fc_showMessage(string arg, int per);

    // public delegate int fc_showProgress(out int percent);
    public fc_showMessage showMessage;

    public delegate void fc_setInstallPGStatus(frmInstall.enum_InstallList target, int rowindex, frmInstall.enum_InstallStatus status);

    public fc_setInstallPGStatus fnc_setInstallPGStatus;

    // progressbarのみ変更する場合
    // public html.fn_progress showProgress;


    public void Dispose()
    {
      if (htmldoc != null)
      {

      }
      if (programs != null)
      {
        programs.Clear();
        programs = null;
      }
      if (charaurls != null)
      {
        charaurls.Clear();
        charaurls = null;
      }
      if (voiceurls != null)
      {
        voiceurls.Clear();
        voiceurls = null; 
      }
      if (directorys != null)
      {
        directorys.Clear();
        directorys = null;
      }

    }





    // TODO delegateを定義し、program設定を読み込んだらイベントをcall する
    // public delegate void getProgramdata(ref program progname);
    //　何がしたいかというと、インストールするprogramをGUIで選択させる
    // ここまでする必要があるかどうかだな


    public install()
    {
      // c:\users\[]\program を想定
      // this.destdir = destdir;
      init();
    }


    public void init()
    {
      if (programs != null)
      {
        programs.Clear();
        programs = null;
      }
      if (charaurls != null)
      {
        charaurls.Clear();
        charaurls = null;
      }
      if (voiceurls != null)
      {
        voiceurls.Clear();
        voiceurls = null;
      }
      programs = new List<program>();
      charaurls = new List<charaurl>();
      voiceurls = new Dictionary<string, voiceDefines>();
      directorys = new Dictionary<string, directory>();
      // tempdir = INI_TEMPDIR;
      // TODO USER HOME dirを取得してtempを作成
      // TODO tempをc#のgetTempPathから取得し、subfolderをcreateする

    }

    html curdwhtml;

    public int getDownloadPercent()
    {
      if (curdwhtml == null)
      {
        return 0;
      }
      return curdwhtml.downloadpercent;
    }

    private void pf_showMessage(string arg, int per)
    {
      if (showMessage == null)
      {
        return;
      }
      if (per < 0)
      {
        int i = 0;
        Console.WriteLine("progress bar minus" + i.ToString());
      }
      showMessage(arg, per);
      // int j = 0;
    }

    public bool readInstallhtml()
    {
      init();
      htmldoc = new html(Install_HTML);
      // int i = 0;
      program p;

      pf_showMessage("プログラム情報取得", 1);


      // ディレクトリ設定
      // pgの前にdirを読み込み、tempを取得
      // json installで使用するため
      string buff;
      directory d;
      HtmlAgilityPack.HtmlNodeCollection nodes;
      nodes = htmldoc.getNodes("//table[@class='directory']/tbody/tr");
      string home = Utils.Sysinfo.getHomedir();
      foreach (HtmlAgilityPack.HtmlNode n in nodes)
      {
        d = new directory();
        buff = htmldoc.getText(n, ".//td[1]").Trim().TrimEnd(Environment.NewLine.ToCharArray());
        d.id = buff;
        directorys.Add(buff, d);
        d.name = htmldoc.getText(n, ".//td[2]");

        d.origpath = htmldoc.getText(n, ".//td[3]");
        d.path = d.origpath.Replace("%HOME%", home).Replace(((char)165).ToString(), "\\"); ;
        d.path = d.path.Replace("%DOWNLOAD%", Utils.Sysinfo.getDownloaddir()).Replace(((char)165).ToString(), "\\"); ;
        d.memo = htmldoc.getText(n, ".//td[4]");
        /* ここではまだ作成しない 
        if (d.id == "tempdir")
        {
          // ここでtempdirを設定しちゃっていいのか？
          // jsonファイルを読み込むため
          // とりあえず作っておいて、変更があったら削除する
          Globals.tempdir = d.path;
          Utils.mkdir(Globals.tempdir);
        }
        */
        // c# library create temp fileを使用する
      }
      // nodeをクリアするとhtml objが壊れるのでdisposeで行う
      // nodes.Clear();
      //nodes = null;

      nodes = htmldoc.getNodes("//table[@class='pg']/tbody/tr");

      // pg tableの処理 installするpg
      foreach (HtmlAgilityPack.HtmlNode n in nodes)
      {
        // trが選択されてるはず
        // 常に１番目のtrが選択され、必ずaviutlが入ってしまう
        // i = 0;
        p = new program();
        programs.Add(p);
        // p.name = n.SelectSingleNode(".//td[1]").InnerText;
        // p.url = n.SelectSingleNode(".//td[2]").InnerText;
        // p.downloadurl = n.SelectSingleNode(".//td[3]").InnerText;
        // p.xpath = n.SelectSingleNode(".//td[4]").InnerText;
        p.name = htmldoc.getText(n, ".//td[1]");
        p.id = p.name; // pgのidとしてnameを使用 jsonではid定義されている
        p.url = htmldoc.getText(n, ".//td[2]").Trim();
        p.downloadurl = htmldoc.getText(n, ".//td[3]");
        p.xpath = htmldoc.getText(n, ".//td[4]");
        p.file = htmldoc.getText(n, ".//td[5]");
        p.subdir = htmldoc.getText(n, ".//td[6]");

        /*
         * 表示は\を使いたいのでそのまま
        buff =
        if (buff.Contains(@"\") == true)
        {
          buff = buff.Replace(@"\", "\\");
        } else
        {
          p.subdir = buff;
        }
        */

        p.openurl = htmldoc.getText(n, ".//td[7]").Trim();
        p.openurlflag = true;
        if (p.openurl == "false")
        {
          p.openurlflag = false;
        }
        if (p.downloadurl == null || p.downloadurl.Length == 0)
        {
          p.downloadurl = p.url;
        }
        p.version = htmldoc.getText(n, ".//td[8]").Trim();
        p.exefile = htmldoc.getText(n, ".//td[9]").Trim();
        p.xpath = htmldoc.getText(n, ".//td[4]");
        p.dispflag = true;

        p.partentrowindex = -1;
        p.partentname = "";
        if (p.subdir.Contains("json") == true)
        {
          // 子供のjsonがある
          string[] ary = p.subdir.Split(',');
          if (ary.Length != 2)
          {
            continue;
          }
          // 子供のjsonを読み込んでここのprogramsに追加しないといけない
          // progress barを正常に表示させるため
          jsoninstall jinst = new jsoninstall(p.downloadurl);
          List<program> jsonpg = new List<program>();
          bool ret = jinst.read(ref jsonpg);
          if (ret == false)
          {
            continue;
          }
          foreach (program childp in jsonpg)
          {
            childp.partentname = p.name;
            programs.Add(childp);
          }
          // ここでは設定できない
          // programs.Last().partentrowindex = -1;

          jsonpg.Clear();
          jsonpg = null;
        }
      }


      charaurl c;
      nodes = htmldoc.getNodes("//table[@class='chara']/tbody/tr");
      // nodes = htmldoc.getNodes("//table[@class='voice']/tbody/tr");
      // キャラ素材 tableを内部クラスに保存
      foreach (HtmlAgilityPack.HtmlNode n in nodes)
      {
        c = new charaurl();
        charaurls.Add(c);
        c.name = htmldoc.getText(n, ".//td[1]");
        c.type = htmldoc.getText(n, ".//td[2]");
        c.url = htmldoc.getText(n, ".//td[3]");
      }


      // voice voiceを検索
      voiceDefines vo;
      // nodes.Clear(); nodesをclearすると、htmldocにまで影響を及ぼす
      // nodes = null;
      nodes = htmldoc.getNodes("//table[@class='voice']/tbody/tr");
      // voice tableを内部クラスに保存
      foreach (HtmlAgilityPack.HtmlNode n in nodes)
      {
        vo = new voiceDefines();
        // voiceurls.Add(vo);
        buff = htmldoc.getText(n, ".//td[2]");
        if (voiceurls.ContainsKey(buff) == false)
        {
          this.voiceurls.Add(buff, vo);
        }
        vo.id = buff;
        vo.name = htmldoc.getText(n, ".//td[3]");
        vo.talkEngine = htmldoc.getText(n, ".//td[4]");
        vo.url = htmldoc.getText(n, ".//td[5]");
        vo.downloadurl = htmldoc.getText(n, ".//td[6]");
        vo.xpath = htmldoc.getText(n, ".//td[7]");
        vo.setup = htmldoc.getText(n, ".//td[8]");
        vo.exefile = htmldoc.getText(n, ".//td[9]");
        buff = htmldoc.getText(n, ".//td[1]");
        vo.installflag = false;
        if (buff == "true")
        {
          vo.installflag = true;
        }

        // v.name = n.SelectSingleNode(".//td[1]").InnerText;
        // v.talkEngine = n.SelectSingleNode(".//td[2]").InnerText;
        // v.url = n.SelectSingleNode(".//td[3]").InnerText;
        // v.downloadurl = n.SelectSingleNode(".//td[4]").InnerText;
        // v.xpath = n.SelectSingleNode(".//td[5]").InnerText;
      }

      // version check
      string pgver = htmldoc.getNode("//div[@class='saltstonesetup']");
      this.html_pgver = pgver;


      htmldoc.Dispose();

      // htmldoc.clear



      return true;
    }


    public bool installAviutl()
    {
      pf_showMessage("インストール開始", 1);
      // showMessage("インストール開始");
      // showProgress(0);

      Utils.mouseCursor.wait();

      if (htmldoc == null)
      {
        return false;
      }

      if (tempdir == null)
      {
        pf_showMessage("tempdirが設定されていません", 0);
        return false;
      }
      Utils.Files.mkdir(tempdir);

      // program install
      // table class pg


      // programsに対してdownload -> extract処理
      // TEMPの削除
      // TODO USER HOME dirを取得してtempを作成
      // TODO tempをc#のgetTempPathから取得し、subfolderをcreateする
      // const string tempdir = @"C:\Users\yasuhiko\temp";
      Utils.Files.delete(tempdir);
      Utils.Files.mkdir(tempdir);
      // destフォルダの作成
      // aviutl
      //string aviudir = Utils.getHomedir() + "\\program\\aviutl";
      // string aviudir = this.destdir + "\\" + Aviutl_SUBDIR;
      string aviudir = this.destdir;
      Utils.Files.mkdir(aviudir);
      string buff = aviudir + "\\Plugins"; // plugin用ディレクトリを作成
      // Utils.mkdir(buff);
      string purl = "";
      int progress = 0;
      int interval = (int)((double)100.0 / (double)programs.Count);
      int pgprogress = 0; // pg install毎にカウントアップ
      foreach (program ps in programs)
      {
        pf_showMessage("Win待機:" + ps.name, pgprogress);
        Utils.sleep(50);
        Utils.setWindowForground(this.mainform);

        // progress += interval;
        pgprogress += interval;
        progress = pgprogress;
        pf_showMessage("インストール:" + ps.name, pgprogress);
        Utils.sleep(500);
        mainform.BringToFront();
        mainform.Focus();
        Utils.sleep(10);

        ps.error = "";


        if (ps.partentrowindex == -1)
        {
          fnc_setInstallPGStatus(frmInstall.enum_InstallList.program, ps.rowindex, frmInstall.enum_InstallStatus.progress);
        }

        if (ps.subdir.Contains("json,") == true)
        {
          // jsonは画面に表示させたいのでreadでは読み込んでおく
          // ただし、子供は展開されているのでinstallはskipする

          // statusを表示する場合は、完了判定はどうするか？
          // programsにstatus表示の親rowindexを保存しかないといけないな
          continue;
        }

        string name = ps.name; // for debug

        string fullfilename = "";
        string filename;
        // programよりzipをダウンロード
        // webを開いてnodeを選択
        if (ps.xpath == "openurlonly")
        {
          Utils.openURL(ps.url);
          fnc_setInstallPGStatus(frmInstall.enum_InstallList.program, ps.rowindex, frmInstall.enum_InstallStatus.success);
          continue;

        }
        if (ps.subdir.Length == 0 && ps.file.Length == 0)
        {
          // subdirもfileも指定がない場合は処理できない
          // ?
          ps.error = "処理方法が指定されていません";
          fnc_setInstallPGStatus(frmInstall.enum_InstallList.program, ps.rowindex, frmInstall.enum_InstallStatus.error);
          continue;
        }

        if (ps.xpath == "direct")
        {
          filename = ps.file;
          purl = ps.downloadurl;
        } else if (ps.xpath.Length != 0)
        {
          html dwhtml = new html(ps.downloadurl);
          // curdwhtml = dwhtml;
          // htmlで改行が入っているとここでfilenameに\nが入る
          filename = dwhtml.getAttribute(ps.xpath, "href").TrimEnd(Environment.NewLine.ToCharArray());
          // filenameにルートからのパスが入っている場合の処理

          // fullfilename = tempdir + "\\" + filename;
          // dwhtmlのpath
          if (filename.Substring(0, 4) == "http")
          {
            purl = filename; // download urlを編集
            filename = filename.Split('/').Last();
          } else
          {
            purl = dwhtml.url + filename; // download urlを編集

          }
          dwhtml = null;
          // buff = purl + filename;
        } else
        {
          // ファイル名の指定がない場合はurlより抽出する
          buff = ps.downloadurl;
          purl = buff;
          // buffよりfilenameを切り出すc
          filename = buff.Split('/').Last();
          // fullfilename = tempdir + "\\" + filename;
        }
        // ファイル名を正規化 
        filename = filename.Trim().TrimEnd(Environment.NewLine.ToCharArray());
        // 空白の場合%20となる
        filename = filename.Replace("%20", "_");
        fullfilename = tempdir + "\\" + filename;

        // Downloadフォルダにないかチェック
        // ある場合はdwをスキップし、Downloadフォルダよりcopy
        string dwdir = Utils.Sysinfo.getDownloaddir();
        string copyfile = dwdir + "\\" + filename;
        if (Utils.Files.exist(copyfile) == true)
        {
          // temp file delete
          Utils.Files.delete(fullfilename);
          Utils.Files.Copy(copyfile, fullfilename);
        } else
        {
          // downloadにファイルがなければwebよりダウンロード
          // dw fileを downlodsにcopy 
          progress += 1;
          pf_showMessage("ダウンロード:" + ps.name, progress);
          // htmldoc.pf_progress = showProgress;
          curdwhtml = htmldoc;
          // curdwhtml.downloadpercent = 0;
          // 
          htmldoc.webDownloadFile(purl, fullfilename);
          Utils.Files.Copy(fullfilename, copyfile);

        }

        // html.webDownloadFile(purl, fullfilename);


        // ここまででダウンロードできた
        // 展開先は？
        // home\program\aviutl

        // error sample用
        /*
        if (ps.name.Contains("かんたん") == true)
        {
          pf_showMessage("ダウンロードに失敗しました:" + ps.name, progress);
          ps.error = "ダウンロード失敗";
          int errorrowindex = ps.rowindex;
          if (ps.childprogram != null)
          {
            errorrowindex = ps.partentrowindex;
          }
          fnc_setInstallPGStatus(frmInstall.enum_InstallList.program, errorrowindex, frmInstall.enum_InstallStatus.error);
          // errorはguiで見れるよう保存したので、次のinstallを行う
          continue;

        }
        */

        // urlが不正だとdownloadに失敗する
        // ファイルが正常なファイルか確認する必要がある
        long filesize = Utils.Files.getFileSize(fullfilename);
        if (filesize == 0)
        {
          pf_showMessage("ダウンロードに失敗しました:" + ps.name, progress);
          ps.error = "ダウンロード失敗";
          fnc_setInstallPGStatus(frmInstall.enum_InstallList.program, ps.rowindex, frmInstall.enum_InstallStatus.error);
          // errorはguiで見れるよう保存したので、次のinstallを行う
          continue;
        }


        // zipの展開
        // pluginsフォルダを考慮しないといけない
        string fext = Utils.Files.getextention(fullfilename);
        if (fext == ".zip")
        {
          progress += 1;
          pf_showMessage("展開:" + ps.name, progress);
          // zipの展開 -> utils libraryに移動

          Zip zipf = new Zip(fullfilename);
          bool zipfret = zipf.extract(tempdir);

          /*
          // System.IO.Compression.ZipFile.ExtractToDirectory(fullfilename, tempdir); // overrite true
          // zipfileを使うと文字化けする
          ReadOptions opt = new ReadOptions();
          //opt.StatusMessageWriter =
          opt.Encoding = System.Text.Encoding.GetEncoding(932);

          // downloadがうまくできてない
          ZipFile zipf = ZipFile.Read(fullfilename, opt);
          zipf.ExtractAll(tempdir, ExtractExistingFileAction.OverwriteSilently);
          zipf.Dispose();
          zipf = null;
          */
          // 元ファイルの削除
          Utils.Files.delete(fullfilename); // debug comment out
        }

        progress += 1;
        pf_showMessage("Install開始:" + ps.name, progress);

        // subdirでexe指定がされていればそれを実行
        // 単なるdirectory名の場合は展開
        // subdir=destdir で インストール方法を定義している

        // setupsが定義されている場合、それを使用する
        if (ps.installmethods != null &&  ps.installmethods.Count > 0)
        {
          foreach (InstallMethod instm in ps.installmethods)
          {
            switch (instm.cmd)
            {
              case InstallMethod.enum_cmd.exe:
                try
                {
                  progress += 1;
                  pf_showMessage("Winを待機:" + ps.name, progress);

                  Utils.setWindowForground(this.mainform);
                  progress += 1;
                  pf_showMessage("Install実行:" + ps.name, progress);

                  Utils.mouseCursor.clear();
                  // TODO aviutlのshortcutが作成されない
                  // Utils.runexec(fullfilename, ""
                  //  , Utils.enum_runmode.waitforexit
                  //  , Utils.enum_runwinfront.bringToFront);
                  // frontにmainを持ってくるとsetupが隠れる
                  Utils.runexec(fullfilename, ""
                    , Utils.enum_runmode.waitforexit);
                  Utils.mouseCursor.wait();
                  progress += 1;
                  pf_showMessage("完了:" + ps.name, progress);
                }
                catch (Exception e)
                {
                  ps.error = e.Message;
                  ps.error = "ダウンロード失敗";
                  fnc_setInstallPGStatus(frmInstall.enum_InstallList.program, ps.rowindex, frmInstall.enum_InstallStatus.error);

                }

                break;
            }
          }
          // continueしてもいいはずだが、、、
        }

        if (ps.subdir.Contains("exe") == true)
        {
          progress += 1;
          pf_showMessage("Setup実行:" + ps.name, progress);
          // ここは通らないはず
          string[] ary = ps.subdir.Split(','); // exe,ndp48.exe,/q /restart
          if (ary.Length <= 1)
          {
            ps.error = "想定していないロジックを通りました";
            fnc_setInstallPGStatus(frmInstall.enum_InstallList.program, ps.rowindex, frmInstall.enum_InstallStatus.error);
            continue;
          }
          string exe = ary[1];
          string exeopt = "";
          if (ary.Length > 2)
          {
            exeopt = ary[2];
          }

          // buffにはexeが入っているはず
          string exefile = tempdir + "\\" + exe;
          // \マーク a5をc5に変更
          exefile = exefile.Replace(((char)165).ToString(), "\\");


          // jsonでも子供のjsonを含めすべてprograms二展開されてる
          // ここは通らないはず
          if (ps.sourcefiletype == program.enum_sourcetype.json)
          {
            // urlで指定されたjsonを読み込んで処理を実行
            // json clas?
            // json,install_c++.json
            program jsonpg = ps;
            bool ret = jsoninstall.install(ref jsonpg);
            if (ret == false)
            {
              pf_showMessage("インストール失敗:" + ps.name + "install from json failed", progress);
              Utils.sleep(1000); // waitしてuserがわかるように
                                 // TODO もしくはmessageboxをgui側で表示する
                                 // 処理自体は続行させる
              ps.error = "json指示によるインストールが失敗しました";
              fnc_setInstallPGStatus(frmInstall.enum_InstallList.program, ps.rowindex, frmInstall.enum_InstallStatus.error);
              continue;
            }

            continue;
          }


          // ここも通らないはず
          if (Utils.Files.exist(exefile) == false)
          {
            // exeの存在チェック
            // install.batはsubfolder/intall.batで記述
            // 
            pf_showMessage("Setup失敗:" + ps.name + "setup faile not found", progress);
            Utils.sleep(1000); 
            // waitしてuserがわかるように
                               // TODO もしくはmessageboxをgui側で表示する
                               // 処理自体は続行させる？
            ps.error = "setup.exeが指定されていません";
            fnc_setInstallPGStatus(frmInstall.enum_InstallList.program, ps.rowindex, frmInstall.enum_InstallStatus.error);
            return false;
          }
          Utils.exeshellcmd(exefile, exeopt);

        } else if(ps.xpath != "direct")
        {
          // jsonでもここを通っていたので修正
          progress += 1;
          pf_showMessage("コピー:" + ps.name, progress);
          // fileが指定されていたらそのファイルのみコピー
          // 指定がなければすべてコピー
          string destdir = aviudir;
          // \はA5,c5がある winではa5は入力不可
          // if (ps.subdir.Contains(((char)165).ToString()) == true)
          // {
          //  ps.subdir = ps.subdir.Replace(((char)165).ToString(), "\\");
          // }
          // \マーク a5をc5に変更
          ps.subdir = ps.subdir.Replace(((char)165).ToString(), "\\");
          if (ps.subdir.Length > 0)
          {
            destdir = aviudir + "\\" + ps.subdir;
            Utils.Files.mkdir(destdir);
          }
          // xcopyでファイルがないとエラーになる
          // 原因不明

          Utils.mouseCursor.wait();
          buff = "*.*";
          if (ps.file.Length > 0)
          {
            buff = ps.file;
            System.IO.File.Copy(tempdir + "\\" + buff, destdir + "\\" + buff, true); // overwrite
          } else
          {
            // string cmd = tempdir + @"\" + buff + " " + destdir + @" /Y /S /E";
            // TODO xcopyではなく、c#のfilecopyを使う
            string option = "/C xcopy " + tempdir + "\\*.* " + destdir + " /Y /Q /S /E";
            // Utils.exeshellcmd("xcopy", cmd);
            Utils.exeshellcmd("cmd.exe", option);

            ps.existfile = false;
            // shurtcut作成のため、元exeを保存
            filename = ps.exefile;
            if (filename.Length > 0)
            {
              filename = destdir + "\\" + filename;
              // exefileはps.exefileに展開先のexeが入っているはず
              if (Utils.Files.exist(filename) == true)
              {
                ps.exefullfilename = filename;
                // ps.exefile = filename;
                ps.existfile = true;
              }
            }

          }
          Utils.mouseCursor.wait();
          if (ps.name.Contains("aviutl") == true)
          {
            this.aviutldir = destdir;
          }
        }

        // Utils.runexec("xcopy",option);
        // 展開したファイルを削除、次のファイルへ備える
        if ((ps.url.Length > 0 && ps.openurl.Length == 0) || (ps.openurl != "false"))
        {
          progress += 1;
          pf_showMessage("URL:" + ps.name, progress);

          // Utils.openURL(ps.url,Utils.enum_openurl_mode.minimized);
          Utils.openURL(ps.url);
        }

        int rowid = ps.rowindex;
        if (ps.partentrowindex != -1)
        {
          rowid = ps.partentrowindex;
        }
        fnc_setInstallPGStatus(frmInstall.enum_InstallList.program, rowid, frmInstall.enum_InstallStatus.success);



        // tempをいったん掃除
        Utils.Files.delete(tempdir);
      }

      // voice tableの処理 installするボイス

      // キャラ素材ディレクトリの処理 install するキャラ素材
      // webを開き、ユーザにダウンロードさせる

      pf_showMessage("インストール完了", 100);
      Utils.mouseCursor.clear();


      // htm = null;

      // TODO installしたバージョンをversion.txtに書き出し
      return true;
    }


    public bool setupAviutl()
    {
      // aviutlの初期環境設定
      // 最大画像サイズ
      // キャッシュサイズ 32～1048576
      // opt 編集のレジューム機能
      // opt ファイルのddでウィンドウをアクティブにする
      // opt 編集ファイルが閉じられる前に確認ダイアログを表示する
      // 書き出しファイル名

      // exeditの初期設定
      // レイヤー幅
      // おそらくexedit.iniにすべて保存されている

      // 単純にiniファイルをコピーするだけ
      string inif = Aviutl_INIFILE;
      // 実行時ディレクトリに保存されているはず
      if (Utils.Files.exist(inif) == false)
      {
        pf_showMessage("設定失敗:iniファイルがありません", 100);
        return false;
      }
      // destdirはprogramになってる aviutl dirを選択しないといけない

      // exedit.iniとの違いは？
      // exedit.iniでは読み込めるファイルを定義している
      string destf = this.aviutldir + "\\" + inif;
      System.IO.File.Copy(inif, destf, true); // overrite true

      return true;
    }

    public bool installVoice(string destdir)
    {
      pf_showMessage("ボイスインストールを開始します", 1);

      // init();
      // html htmldoc = new html(Install_HTML);
      if (htmldoc == null)
      {
        return false;
      }

      string buff;
      // string filename = "Bouyomi";
      // string fullfilename = "";
      //string tempfile = tempdir + "\\" + filename + ".zip"; // downloadするときの固定ファイル名
      // string voicedest = this.destdir + "\\voice\\" + filename;
      // string voicedest;
      // Utils.mkdir(voicedest);
      // TODO ダウンロードしたフィル名ってわからないのかな？
      Utils.Files.delete(tempdir);
      Utils.Files.mkdir(tempdir);
      string tempfile = tempdir + "\\download.dat";
      string filename = "";
      int i = 0;
      int interval = (int)((double)100.0 / (double)this.voiceurls.Count);
      foreach (voiceDefines v in this.voiceurls.Values)
      {
        i += interval;
        pf_showMessage("インストール:" + v.name, i);


        if (v.installflag == false)
        {
          continue;
        }
        if (v.xpath.Length == 0)
        {
          continue;
        }
        // TODO install.htmlに定義を追加
        // 日本語でもOK?
        // fullfilename = this.destdir + "\\" + INI_VoiceSUBDIR + "\\" + filename;

        fnc_setInstallPGStatus(frmInstall.enum_InstallList.voice, v.rowindex, frmInstall.enum_InstallStatus.progress);

        // 直接ダウンロードでも ファイル名がある場合もあれば、ファイル名がない場合もある
        if (v.xpath.Contains("direct") == true)
        {
          // TODO download配下に同名ファイルがないかチェックしたい
          // しかし、、、dwするまでファイル名がわからないんだよねー
          // やるとするとdwしたファイルをどこかに保存しておくとかだな

          // ダウンロードしたzipを展開
          i++;
          pf_showMessage("ダウンロード:" + v.name, i);

          filename = htmldoc.webDownloadFile(v.downloadurl, tempfile);
          filename = tempdir + "\\" + filename;
          Utils.Files.rename(tempfile, filename);
          ReadOptions opt = new ReadOptions();

          // filenameのextを検査、zipだったら展開
          if (Utils.Files.getextention(filename) == ".zip")
          {
            i++;
            pf_showMessage("展開:" + v.name, i);
            //opt.StatusMessageWriter =
            opt.Encoding = System.Text.Encoding.GetEncoding(932);
            ZipFile zipf = ZipFile.Read(filename, opt);
            zipf.ExtractAll(tempdir, ExtractExistingFileAction.OverwriteSilently);
            zipf.Dispose();
            zipf = null;
            Utils.Files.delete(filename);
          }
          /*else
          {
            // pathを取得しないと
            // buff = tempdir + "\\" + filename;
            // System.IO.File.Move(tempfile, buff);
            // 普通のfileだったら、tempfileをrenameする？
            // なんかめんどくさいな


          }
          */


          try
          {
            // TODO setup exeを含む
            // install.batを含む
            // table voiceのxpathで判定
            if (v.xpath.Contains("exe") == true)
            {
              string[] ary = v.xpath.Split(','); // exe,ndp48.exe,/q /restart
              if (ary.Length <= 2)
              {
                break;
              }
              string exe = ary[1];
              string exeopt = "";
              if (ary.Length > 2)
              {
                exeopt = ary[2];
              }

              // buffにはexeが入っているはず
              string exefile = tempdir + "\\" + exe;
              if (Utils.Files.exist(exefile) == false)
              {
                // exeの存在チェック
                // install.batはsubfolder/intall.batで記述
                break;
              }
              Utils.exeshellcmd(exefile, exeopt);

            } else
            {
              buff = filename;
              i = buff.IndexOf("_");
              if (i > 0)
              {
                // _があればそれを除き、先頭のpg名だけsubdirとして使用する

                filename = buff.Substring(0, i);
              }

              // TODO bouyomi pluginだとdstはbouyomi dstになる
              // どうやってbouyomiのsubdirを特定するか?
              // どうやってinstall.htmlで指定させるか？
              buff = "";
              string voicedest = destdir + "\\" + Utils.Files.getbasename(filename);
              // voicedest = this.destdir + "\\voice\\" + Utils.getbasename(filename);
              bool extrasetupflag = false;
              // setup filecopy or filecopy,ID=Bouyomi
              string[] ary = v.setup.Split(Environment.NewLine.ToCharArray());
              if (ary.Length > 1)
              {
                ary = ary[0].Split(',');
                if (ary.Length == 2)
                {
                  buff = ary[1].Split('=').Last();
                  voicedest = voiceurls[buff].destdir;
                  extrasetupflag = true;
                }
              }
              // aryが0のときはどうするの？ "filecopy"のときはvoicedestはそのまま filenameのbase
              // string[] ary = v.setup.Split(',');
              if (extrasetupflag == false)
              {
                // ぼうよみちゃんがターゲット
                // pluginなどで元のvoice softのdirをほかで使用する
                // buff = ""のままとすると、pluginではなく、voice soft本体
                // voicedest = this.destdir + "\\voice\\" + Utils.getbasename(filename);
                // Utils.mkdir(voicedest);
                v.destdir = voicedest; // plugin用にフォルダを保存
              }
              Utils.Files.mkdir(voicedest);

              i++;
              pf_showMessage("コピー:" + v.name, i);

              string option = "/C xcopy " + tempdir + "\\*.* " + voicedest + " /Y /Q /S /E";
              // Utils.exeshellcmd("xcopy", cmd);
              Utils.exeshellcmd("cmd.exe", option);

              Utils.Files.delete(tempdir);

              // 通常はsetupがあるはず
              // bouyomiはそのままコピーするだけ

              // shortcut用にexefileの存在チェックを行う
              if (v.exefile.Length > 0)
              {
                buff = v.destdir + "\\" + v.exefile;
                if (Utils.Files.exist(buff) == true)
                {
                  v.existfile = true;
                  v.exefile = buff;
                }
              }
            }
          }
          finally
          {
            fnc_setInstallPGStatus(frmInstall.enum_InstallList.voice, v.rowindex, frmInstall.enum_InstallStatus.success);
            if (v.url.Length > 0)
            {
              Utils.openURL(v.url);
            }
          }
        }

      }
      return true;
    }

    public bool setupVoice()
    {
      pf_showMessage("ボイスをセットアップします", 1);
      // bouyomi用のsettingsをコピーする必要がある
      // TODO どうやってinstall.htmlで指定するか？
      // どうやってコピーするか?


      foreach (voiceDefines v in voiceurls.Values)
      {
        string setup = v.setup.Trim(Environment.NewLine.ToCharArray()).Trim();
        List<string> lines = new List<string>(setup.Replace("\r\n", "\n").Split(new[] { '\n', '\r' }));
        foreach (string line in lines)
        {
          if (line.Contains("filecopy") == true)
          {
            // filecopyはinstallで行っているので無視
            continue;
          }
          if (line.Contains("replacestring") == true)
          {
            string[] ary = line.Split(',');
            string file = ary[1];
            string repstr = ary[2];
            // ぼうよみように特殊処理
            // TODO 本来であればinstall.htmlに処理を記述するべき
            if (v.id != "Bouyomi_plugin")
            {
              continue;
            }
            string destdir = voiceurls["Bouyomi"].destdir;
            string sourcedir = "BouyomiChan\\*.*"; // exeのsubdir配下

            string option = "/C xcopy " + sourcedir + " " + destdir + " /Y /Q /S /E";
            Utils.exeshellcmd("cmd.exe", option);

            string settingfile = destdir + "\\" + file;
            if (Utils.Files.exist(settingfile) == false)
            {
              continue;
            }
            string alltext = System.IO.File.ReadAllText(settingfile);
            ary = repstr.Split('=');
            string repsrc = ary[0];
            string repdestkey = ary[1];
            repdestkey = repdestkey.Replace("%", "");
            string repdest = directorys[repdestkey].path;
            alltext = alltext.Replace(repsrc, repdest);

            Utils.Files.delete(settingfile);
            System.IO.File.WriteAllText(settingfile, alltext);


          }
        }

      }


      // TODO voice wav,txtをどこに保存するか？
      // 固定でaviutl workspaceのvoiceフォルダ
      // ここでsetting fileを変更する必要があるか？
      // かんしくんで移動をセットしておけば問題なさそう
      // aviutlのかんしくんの配下のsettingsを変更する必要がある
      // なので、かんしくんのtmpフォルダをセットする必要がある
      // ちょっとナンセンスだな
      // この後、かんしくんの設定も必要 -> これはキャラ設定exeでまとめて行う

      // 追加でscriptを走らせる必要がある
      // windows script hostでなんとかできそうだけど、、、
      // 固定で設定したほうがいいね
      // voiceを保存するフォルダか、、、
      // 設定になるから、キャラ設定と同じようにcharasetting.exeで実行したいんだよねー
      // でも、キャラとは関係ないよね
      pf_showMessage("セットアップが終了しました", 100);

      return true;
    }

    public bool createPGShortcut()
    {
      // string destdir = Utils.getDesktopdir();

      // shortcutを作成するのは、install.html上でexefileが定義されているもののみ
      foreach (program pg in programs)
      {
        if (pg.existfile == false)
        {
          continue;
        }
        Shortcut.createshortcut(pg.exefullfilename);
        // Shortcut.Create(scfile, destdir, pg.exefile);

      }

      return true;
    }
    public bool createVoiceShortcut()
    {
      // string destdir = Utils.getDesktopdir();

      foreach (voiceDefines vo in voiceurls.Values)
      {
        if (vo.existfile == false)
        {
          continue;
        }
        Shortcut.createshortcut(vo.exefile);


      }

      return true;
    }
  }
}
