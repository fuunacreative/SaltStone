using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vocalization
{
    public static class Voices
    {
    // cevio,openjtalk
    // voicevox, ai voice
    // ゆずきゆかり、ずんだもん、
    // aquestalk れいむ・まりさ（ゆっくりvoice) speed,tone,volume
    // voicevox ずんだもん、四国めたん、春日部つむぎ、雨晴はう、波音りつ
    //   話速,音高,抑揚,音量
    //   httpd による rest apiのサポートあり
    // CeVIO 佐藤ささら
    //   大きさ、速さ、高さ、声質、（元気、怒り、哀しみ）
    //   C#独自のプロセス間通信により連携可能
    //   https://w.atwiki.jp/ceviouser/pages/70.html#id_1129cdd5
    // COEIROINK
    //   httpdによる rest apiのサポートあり
    // VOCALOID 結月ゆかり、紲星あかり、弦巻マキ、東北ずん子、東北きりたん、琴葉 茜・葵
    //   AssistantSeika 複数のボイスソフトに対しコマンドラインでの発声を実行する
    //   2とか５とかいろいろある
    //   基本 api は提供されておらず、sendmessage,uws で操作する必要がある

    // インターフェースとしては
    // input 発音記号 ,テキスト文字
    // 処理：自然言語処理による形態素解析
    // 　ひらながよみの発音記号を解析する
    //   指定されたボイスソフトを使用して wavを出力
    // output:wav , 口パクアニメ用のphoneticテキスト(aiuoe)
    // 

    // 問題は、、、よみの取得をここでやるか？どうやってやるか？
    
    // saltstoneで管理しているボイスソフト全部 aquestalk,cevioなど
    // aquestalkなら、"AQF1"などのIDで登録する
    public static Dictionary<string, voiceinterface> pVoices;

    public static void init()
    {
      pVoices = new Dictionary<string, voiceinterface>();
      

      // とりま、aquestalkから
    }
  }
}
