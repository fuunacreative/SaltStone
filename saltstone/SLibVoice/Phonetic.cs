using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeCab;

// 名詞,一般,*,*  ,*,*,すもも,スモモ,スモモ
// 品詞,品詞細分類1,品詞細分類2,品詞細分類
// 活用形,活用型,原形,読み,発音
// きょうはとても良い日でした 今日はとても良い日でした
// https://www.a-quest.com/demo/
// アクセント辞書 acdic
//  https://accent.u-biq.org/ki.html
//  // https://www.gavo.t.u-tokyo.ac.jp/ojad/search/index/sortprefix:accent/narabi1:kata_asc/narabi2:accent_asc/narabi3:mola_asc/yure:visible/curve:invisible/details:invisible/limit:20/word:今日
//  https://www.gavo.t.u-tokyo.ac.jp/ojad/search
// by 棒読み  きょ'う+わ/とても/よい/ひ+でした。
// by aqkanji2koe キョ'ーワ/トテモ/ヨ'イヒデ_シタ。
// by mecab 
//   キョウ 名詞-普通名詞-副詞可能 by acdic キョ'ウ
//   ハ 助詞-係助詞 区切り /
//   トテモ 副詞 区切り /
//   ヨイ 形容詞-非自立可能 by acdic ヨ'イ
//   ヒ 名詞-普通名詞-副詞可能
//   デシ 助動詞 _は子音+母音のうち、母音を音にしないケース
//   タ 助動詞
// でした -> で_した 辞書登録 今回は実装しない サーバさいどが必要
// ex きょ'うは/とても/よ'いひでした/ 'はアクセント区切りの中でひとつしかいれられない
// ex きょ'うは/とても/よ'い/'ひ+でし+た。
// ex キョ'ーワ/トテモ/ヨ'イ/ヒ'デ_シタ。 これが一番、自然に聞こえる
// 名詞のアクセントはwebから取得 ( cache or db化)を検討
// よい 強調したい名詞
// 助詞の終わりは"/"
// 適度に文を区切る
// 最後は。をつける-> ユーザ側にまかせる
// 通常の文節区切りは"+"
// 名詞のアクセント取得
// https://www.gavo.t.u-tokyo.ac.jp/ojad/search/index/sortprefix:accent/narabi1:kata_asc/narabi2:accent_asc/narabi3:mola_asc/yure:visible/curve:invisible/details:invisible/limit:20/word:良い

// feature = "名詞,副詞可能,*,*,*,*,今日,キョウ,キョー"
// %f[1-6] 要素の品詞+付属情報 5個 , 書字形基本形(今日),語彙読み(キョウ),発音形(キョー)
// 発音形 をstrに追加していく
// 品詞であればアクセント辞書よりアクセント取得
// 文節の判断 助詞の最後、副詞の最後、形容詞の最後,助動詞の前？はいらないかも

// GET /ojad/search/index/ 
//  sortprefix:accent/narabi1:kata_asc/narabi2:accent_asc/narabi3:mola_asc/yure:visible/curve:invisible/details:invisible/limit:20 
// /word:%E8%89%AF%E3%81%84 HTTP/1.1
// https://www.gavo.t.u-tokyo.ac.jp/ojad/search/index/  
//  sortprefix:accent/narabi1:kata_asc/narabi2:accent_asc/narabi3:mola_asc/yure:visible/curve:invisible/details:invisible/limit:20/word:良い
namespace saltstone
{
  public interface Phonetic
  {
    // static宣言はできない C#8.0(.Net core)からしか使用できない
    string getPhonetic(string arg);
    // phonetic id (aq)などの指定
    // in text out phonetictext
    // mecab , kakashi,yahoo apiなどの読み仮名取得ライブラリ
    // イントネーション

  }

  // これをどのクラスにもたせるか？
  // aquestalkと関連しているのだから、aquestalk voice clasに持たせた方がよい
  public class Phonetic_aqmecab : Phonetic
  {
    public const string CONST_NGCHAR = "!！";
    public  string getPhonetic(string arg)
    {
      // mecabを利用して形態素解析を行い、
      // aquestalk用のイントネーション記号 "/+'"を使ってdllに渡す発声textを作る
      MeCabTagger mb = null;
      string phonetic = "";
      try
      {
        mb = MeCabTagger.Create();
        string[] cols;
        string part; // 名詞、動詞などの品詞
        int i;
        string buff;
        string csvn;
        foreach (MeCabNode n in mb.ParseToNodes(arg))
        {
          csvn = n.Feature;
          if(csvn.IndexOf("BOS") >= 0)
          {
            continue;
          }
          cols = csvn.Split(',');
          part = cols[0];
          // TODO 品詞の場合、アクセント辞書よりphoneticを取得
          // !はNG
          buff = cols[8];
          i = CONST_NGCHAR.IndexOf(buff);
          if (i >= 0)
          {
            continue;
          }

          phonetic += cols[8];
          
          // cols[0]= 品詞
          if(part == "助詞" || part == "副詞")
          {
            phonetic += "/";
          }



        }
      }
      catch (Exception e)
      {
        Logs.write(e);
      } finally
      {
        mb?.Dispose();
      }

      return phonetic;
    }
    
  }
}
