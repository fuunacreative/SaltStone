using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{

  public class program
  {
    public enum enum_sourcetype
    {
      html,
      json
    }

    public enum_sourcetype sourcefiletype;

    public string name; // 表示用の名前
    public string url;
    public string downloadurl;
    public string xpath;
    public string file; // コピーするファイル 複数にはとりあえず対応しない
    public string subdir; // コピーするsubdir pluginsを想定 destdir
    public string openurl;
    // public bool install; // インストールを行うかどうか
    public string version;
    public string exefile; // shortcut用 + install.exe用
    public bool existfile;
    public bool dispflag; // guiに表示するかどうか

    // jsonか通常かを判断するenumは必要か？ -> 必要
    // install.htmlとc++.jsonは親子関係になる
    // これをどのようにclassに保存するか？
    // programsで全部のインストールするexeを管理したい
    // progress barの動きお正しく表示したいから
    // id
    // ここから json install用の定義
    // 上の定義は最終的には必要なくなる
    public string id;
    public string parentid;
    public string dispname;
    // public string dispflag;
    // public string url;
    // public string downloadurl;
    // public string xpath;
    // public string filename; // urlから取得できない場合がある
    // public List<string> setups; // このままでいい？
    // setupsにはexe filecopy createshortcutなどはいる予定
    // なので、classが必要
    public List<InstallMethod> installmethods;
    public string setups_str;
    // public string version;
    public bool installflag; // インストールを行うかどうか
    public string installflag_str;
    public string memo;
    public bool openurlflag; // openurlのstringをboolにしたもの
    public string openurlflag_str;
    public string dispflag_str;
    public bool createshortcut_flag;
    public bool install_success_flag; // 今回のsetupでinstalしたか
    public bool installed_already_flag; // setup前にインストール済みかどうか
    // checkする機構が必要

    // 画面表示用
    public int rowindex; // listでのrowid
    public string error; // install error 現在は未使用
    public int partentrowindex;
    public string partentname;
    // jsonなどの親子関係がある場合、子供が全部終わってから親を完了にする
    // そのために親のrowindexを保存しておく
    public List<program> childprogram;

    // インストール作業用
    public string exefullfilename;

  }

}
