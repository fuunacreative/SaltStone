using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Net;
using System.ComponentModel;

namespace saltstone 
{
  // : IEnumerator<HtmlAgilityPack>
  public class html : IDisposable
  {
    public string url;
    public HtmlAgilityPack.HtmlDocument htmldoc;
    // public HtmlAgilityPack.HtmlNodeCollection nodes;
    // getnodeで選択されたノードのコレクションを保持する
    // foreachでnodeをまわしたいがienumerator interfaceを実装する必要がある
    // ienumratableも存在する

    // progressbarの根本を変更する
    // main threadからここのpublic 変数にアクセスする
    public int downloadpercent;

    // progressbarの進捗更新
    public delegate int fn_progress(int per);
    // public fn_progress pf_progress;

    public delegate bool fn_setCurrentinstall(int rowid);

    public html(string url)
    {

      if (url.Length == 0)
      {
        return;
      }
      System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

      this.url = url;
      WebClient web = null;
      Stream fs = null;
      htmldoc = null;
      try
      {
        web = new WebClient();
        web.Encoding = System.Text.Encoding.UTF8;
        fs = web.OpenRead(url);
        htmldoc = new HtmlDocument();
        htmldoc.Load(fs, System.Text.Encoding.UTF8);
      }
      finally
      {
        if (web != null)
        {
          web.Dispose();
          web = null;
        }
        if (fs != null)
        {
          fs.Close();
          fs.Dispose();
          fs = null;
        }
      }
    }

    public void Dispose()
    {
      if (htmldoc != null)
      {
        htmldoc = null;
      }
    }

    public HtmlNodeCollection getNodes(string xpath)
    {
      // なぜか、読み込み時はtableが３つあるが、２回目に呼び出すと1個しかなくなる
      // retで返したnodeをさわるとおかしくなる
      if (htmldoc == null)
      {
        return null;
      }
      HtmlNodeCollection nodes = htmldoc.DocumentNode.SelectNodes(xpath);
      downloadpercent = 0;
      return nodes;
    }

    public string getNode(string xpath)
    {
      if (htmldoc == null)
      {
        return null;
      }
      HtmlNode n = htmldoc.DocumentNode.SelectSingleNode(xpath);
      return n.InnerText;
    }
    public string getAttribute(string xpath, string attribute)
    {
      if (htmldoc == null)
      {
        return null;
      }
      HtmlNode n = htmldoc.DocumentNode.SelectSingleNode(xpath);
      if (n == null)
      {
        return "";
      }
      return n.Attributes[attribute].Value;

    }

    public string getText(HtmlAgilityPack.HtmlNode node, string xpath)
    {
      string buff = "";
      if (node == null)
      {
        return buff;
      }
      HtmlNode n = node.SelectSingleNode(xpath);
      if (n == null)
      {
        return buff;
      }
      return n.InnerText.Trim().Trim(Environment.NewLine.ToArray());

    }

    // web download asyncを使用するために内部フラグを使う
    private bool downloadstart = false;
    // private WebClient webclient;
    private bool downloadend = false;
    System.Threading.AutoResetEvent downloadwait;
    private WebClient webcli;

    public string webDownloadFile(string url, string filename)
    {

      string dwfile = "";
      WebClient web = null;
      try
      {

        // web.DownloadFile(url, filename);
        // asyncを使う方法に変更したがうまく動かない
        // eventがraiseしない
        // threadを作らないとけいないのでは？



        downloadend = false;
        downloadstart = true;
        downloadpercent = 0;
        object syncobj = new object();
        downloadwait = new System.Threading.AutoResetEvent(false);

        System.Threading.Thread thread = new System.Threading.Thread(() => {
          webcli = new WebClient();
          // webcli = web;
          // asyncじゃないので、進捗は表示できない
          // asyncを使うと、download中かどうかの判定が難しい
          webcli.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webDownloadProgress);
          webcli.DownloadFileCompleted += new AsyncCompletedEventHandler(webDownloadEnd);
          // web.DownloadFileCompleted += new DownloadDataCompletedEventHandler(webDownloadEnd);
          webcli.DownloadFileAsync(new Uri(url), filename, syncobj);
          // System.Threading.Monitor.Wait(syncobj);
          /*
          object syncobj = new object();
          lock (syncobj)
          {
            web.DownloadFileAsync(new Uri(url), filename, syncobj);
            System.Threading.Monitor.Wait(syncobj);
          }
          */
        });
        thread.Start();
        do
        {
          Utils.sleep(50);
          /*
          if (thread.IsAlive == false)
          {
            break;
          }*/
          // System.Threading.Monitor.Wait(syncobj, 100);
          downloadwait.WaitOne(50);
          // ここでwaitするとform timerが動作しない

          // application.runを含んでいないため
          // form timerが動作しない
          // System.Windows.Forms.Application.Run();

          // 問題はend eventがraiseしないこと
          // やはりtimerが動作しない
        } while (downloadend == false);
        /*
        object syncobj = new object();
        lock (syncobj)
        {
          web.DownloadFileAsync(new Uri(u), filename,syncobj);
          // ここでメインスレッドがlockする
          do
          {
            System.Threading.Monitor.Wait(syncobj, 50);
            if (downloadend == true)
            {
              break;
            }
            Utils.sleep(100);
          } while (downloadend == false);
        }
        */
        /*
        int i = 0;
        do
        {

          Utils.sleep(100); // 100ms
          i++;
        } while (i < 100 || downloadend == false);
        */

        // filenameが固定なのがなー dwfileで取得できるから
        // 一時ファイルとして保存しておき、copy deleteするほうがいいかもね
        // content-dispositionを返さない responseもある
        WebHeaderCollection res = webcli.ResponseHeaders;
        if (res == null)
        {
          throw new Exception("no web response");
        }
        string cond = res["Content-Disposition"];
        if (cond == null)
        {
          throw new Exception("no web response cond");
        }
        // webcli.Dispose(); finallyに移動
        // webcli = null;
        // dispositionは必須ではない
        // dwfile = web.ResponseHeaders["Content-Disposition".Substring(web.ResponseHeaders["Content-Disposition"].IndexOf("filename=") + 9).Replace("\"", "");
        dwfile = cond.Substring(cond.IndexOf("filename=") + 9).Replace("\"", "");
      }
      catch (Exception e)
      {
        string buff = e.Message;
        return dwfile;
      }
      finally
      {
        if (web != null)
        {
          web.Dispose();
          web = null;
        }
        if (webcli != null)
        {
          webcli.Dispose();
          webcli = null;
        }
      }
      return dwfile;
    }

    public void webDownloadProgress(object sender, DownloadProgressChangedEventArgs arg)
    {
      if (downloadstart == false)
      {
        return;
      }
      /*
      if (this.pf_progress == null)
      {
        return;
      }*/

      // 100%のうち、?%まで進んだかをintで返す

      long totalbyte = arg.TotalBytesToReceive;
      long readbyte = arg.BytesReceived;
      double per = (double)readbyte / (double)totalbyte;
      int ret = (int)(per * 100);
      downloadpercent = ret;
      // pf_progress(ret);
    }

    private void webDownloadEnd(object sender, AsyncCompletedEventArgs arg)
    {
      if (downloadstart == false)
      {
        return;
      }
      /*
      lock (arg.UserState)
      {
        System.Threading.Monitor.Pulse(arg.UserState);
      }
      */
      downloadpercent = 100;
      downloadwait.Set(); // thread end signal
      downloadend = true;

    }

    public static string getFilenameFromUrl(string url)
    {
      Uri uri = new Uri(url);
      string file = uri.AbsolutePath.Split('/').Last();
      bool isFile = file.Contains('.');

      //  Uri uri = new Uri(url);
      if (isFile == false)
      {
        return "";
      }
      return file;
    }



  }
}
