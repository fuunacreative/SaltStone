using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace saltstone
{
  /// <summary>
  /// jsonファイルに保存する log message情報
  /// </summary>
  public class Logs
  {
    public enum Logtype
    {
      dispwarn = 1,
      dispinfo = 2,
      disperror = 3,
      debug = 10,
      info = 11,
      warn = 12,
      error = 13,
      fatal = 14
    }

    public string logdate {
      get; set;
    } // yyyymmdd_hhmmss
    public string exename {
      get; set;
    }
    public string message {
      get; set;
    }

    public string trace {
      get; set;
    }

    public Logs.Logtype logtype;


    public static bool send(string arg)
    {
      // logmanagerへ送信
      return true;
    }

    public static bool send(Exception ex)
    {
      // logmanagerへ送信
      return true;
    }

    public static bool write(string arg)
    {
      // fileへ書き込む
      // logmanagerへの画面表示は行わない -> logserver側で処理する
      return true;
    }

    public static bool write(Exception ex)
    {
      // fileへ書き込む
      // logmanagerへの画面表示は行わない -> logserver側で処理する
      return true;
    }

    public static void Dispose()
    {
    
    }
  }
}
