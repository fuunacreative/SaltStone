using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace saltstone
{

  // キャラ画像の描画順を保持する
  // chardir\charadraworder.txtで定義されている
  public class CharaDraworder
  {
    // public string charaid; // r ,m
    //  public Chara pChar;
    public static CharaDraworder pCharadraworderdefault = null;

    public string ruleid;
    public string patternregexp;
    public string memo;
    // public bool defalutorder; // defaultとする描画順 [overlayorder] defaultの記述があるもの pCharadraworderdefaultに記載する
    public SortedList<int, CharaDraworderItem> orderitems;
    // public string outfname; // memoryの場合、共有メモリに書きだし　どうやって？ 
    // 共有メモリとファイルを同じように扱う必要がある　memory mapped fileとhdd内部の実ファイル
    // utils memoryfilesでbinaryreader,binarywriterを使う or streamreaderでfsをreadする
    // 描画順序を決めるクラスにc++ imageprocessへのcmdを含めてしまってもよいか？


    // -> draworderにもたせるのではなく、charaクラス（単体）にもたせるべき
    // pngを出力し、キャッシュさせ、formからはpngファイルのみアクセスするようにするべきだねー
    // こうすれば、出力ファイルもchara classでファイル名を編集できる
    // B00F00.pngなど

    // constructorで charaorderを初期する？

    #region memo
    //public static CharaDraworder defaultorder 
    //{
    //  get 
    //  {
    //    // private charaorderはnullのはずだが、なぜか値    w画はいっている
    //    // それに、下の設定ロジックを通らない
    //    if (pCharadraworderdefault != null)
    //    {
    //      return pCharadraworderdefault;
    //    }
    //    pCharadraworderdefault = new CharaDraworder();
    //    pCharadraworderdefault.patternregexp = "*";
    //    List<string> porder = new List<string>();
    //    porder.Add(@"後\*.png");
    //    porder.Add(@"体\*.png");
    //    porder.Add(@"顔\*.png");
    //    porder.Add(@"髪\*.png");
    //    porder.Add(@"眉\*.png");
    //    porder.Add(@"目\*.png");
    //    porder.Add(@"口\*.png");
    //    porder.Add(@"他\*.png");
    //    porder.Add(@"後\*.png");
    //    pCharadraworderdefault.orderitems = new SortedList<int, CharaDraworderItem>();
    //    CharaDraworderItem cdi;
    //    int i = 10;
    //    string buff;
    //    foreach (string item in porder)
    //    {
    //      cdi = new CharaDraworderItem();
    //      cdi.ruleorder = i;
    //      buff = Charas.partskeybydir[item.Substring(0, 1)];
    //      cdi.partsid = buff;
    //      cdi.filepattern = item;
    //      cdi.multiplyflag = false;

    //      pCharadraworderdefault.orderitems[i] = cdi;
    //      i++;
    //    }


    //    return pCharadraworderdefault;
    //  }
    //}
    #endregion


    public void Dispose()
    {
      if (orderitems != null)
      {
        orderitems.Clear();
        orderitems = null;
      }
      if (pCharadraworderdefault != null)
      {
        if(pCharadraworderdefault.orderitems != null)
        {
          pCharadraworderdefault.orderitems.Clear();
          pCharadraworderdefault.orderitems = null;

        }
      }
    }



    public bool readDraworderItems()
    {
      bool fret = false;

      if (orderitems == null)
      {
        orderitems = new SortedList<int, CharaDraworderItem>();
      }

      DB.Query q = new DB.Query();
      DB.DBRecord rec;
      
      q.table = "drawruleitem";
      q.select = "ruleorder,ruleno,dirname,partsid,filepattern,multiplyflag,memo,partsubid,command";
      q.where("ruleid", ruleid);
      q.orderby = "ruleorder";
      bool ret = Charas.charadb.getrecord_noread(q, out rec);
      int i;
      q.clear();
      if (ret == false)
      {
        return fret;
      }
      CharaDraworderItem cdoi;
      while (rec.Read() == true)
      {
        i = rec.getnum(0);
        cdoi = new CharaDraworderItem();
        orderitems.Add(i, cdoi);
        cdoi.ruleorder = i;
        cdoi.ruleno = rec.getstring(1);
        cdoi.partsdir = rec.getstring(2);
        cdoi.partsid = rec.getstring(3);
        cdoi.filepattern = rec.getstring(4);
        i = rec.getnum(5);
        cdoi.multiplyflag = false;
        if(i == 1)
        {
          cdoi.multiplyflag = true;
        }
        cdoi.memo = rec.getstring(6);
        cdoi.partsubid = rec.getstring(7);
        cdoi.command = rec.getstring(8);
        // 全 A01だとparts sub idはnull 
        // このままだと、subidが空白の場合はpartsidで判定しないといけない
        // 他のoverlayorder ruletypeがsubid前提にしているので、それにあわせる


        
      }



      return true;
    }

    
  }

  public class CharaDraworderItem
  {
    public int ruleorder;
    public string ruleno;
    public string partsid;
    public string partsubid; // FA/FBの統合のため、追加
    public string partsdir;
    public string filepattern;
    public bool multiplyflag;
    public string memo;
    public int alpha; // レイヤーによりalpha指定のものがある
    public string command;
  }
}

/*
size=400x320 // width x height
base=C:\Users\fuuna\Videos\charas\れいむ
体\00.png
顔\00a.png
髪\00.png
眉\11.png
目\00.png
口\11.png
髪\00.png
// 明るさ、コントラスト、彩度、色相をどーするか？ -> 計算式で変更できる
 */

