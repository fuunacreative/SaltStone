using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace saltstone
{

  // 発声を保存するクラス
  // セリフと関係ずけされ、
  // 各シーンの各セリフを格納する
  public class Quote
  {

    public int id; // messageid
    public Scenes refsecene; // 相互参照になる。問題ないか？
    // public Voice voiceref;

    private string _voiceid; // slibvoiceのvoiceid
    private string _message;
    public string pronmessage;
    // public string charaexpression;
    public string othercommand;
    // public int speed; // メッセージ発声の個別パラメータ
    // public int tone;
    // public int volume;
    public int length;
    // セリフ毎の発声パラメータ（音量、音程、速度）を保持する // 追加でビブラートやリバーブなど
    // セリフの毎にvoiceEffectを持つ必要がある
    // しかもセリフの途中でもvoiceeffectを持つ
    // configの定義にある場合に設定することになるな
    // セリフの途中でエコーをかけたいときとかは？
    // 現状ではぼうよみちゃんしだい 途中でエコーはかけられるが、それ以降もすべてエコーになる
    // 改行をいれると元に戻る -> 必ず無音区間ができる
    // TODO aquestalkでぼうよみするにはどうすればいいか、わからない

    // public string characode; // 発声するキャラコード 変更する場合に使用
    public string charaid; // r,m
    public Chara chara; // r,mの実態
    public string charafacestr; // B00E00
    public Image charaimage; // 立ち絵 picture
    public VoiceEffect voiceeffect;
    public string wavfile; // 出力されたwavファイル

    // ここに立ち絵とwavファイルを集約する
    // 他からも作成できるように charapicture , voice classでメソッドを用意して
    // それをcallするか、、、
    // タイミングは、、、、ユーザまかせ（aviutlへの展開)にするか
    // wavfile nameをどうきめるか？
    // yyyymmdd_hhmiss_r_message(remove no file string).wavにするか
    public bool makeImage()
    {
      // charafacestrからimageを作成
      // charasのgetmergefileが使える
      return true;

    }

    // これって何に使うの？ -> すべてのQuoteにidを付与する
    // idはintでDatasで一元管理されている
    public static int counter;

    public Quote()
    {
      voiceeffect = new VoiceEffect();
      id = counter;
      counter += 1;
      Datas.quotes[id] = this;
    }

    ~Quote()
    {
      voiceeffect = null;
    }


    public int speed {
      get {
        return voiceeffect.speed;
      }
      set {
        voiceeffect.speed = value;
      }
    }

    public int tone {
      get {
        return voiceeffect.tone;
      }
      set {
        voiceeffect.tone = value;
      }
    }
    public int volume {
      get {
        return voiceeffect.volume;
      }
      set {
        voiceeffect.volume = value;
      }

    }


    public const int MaxMessage = 40; // メッセージの最大制限長 ２段ｘ２０文字
    public const int MaxMessageLine = 20; // メッセージ１行の最大長


    // messegeセット時に発音テキストも同時にセットする
    // message内に声の音響効果（エコー、トーン、速度）なども入れられるようにする
    public string message {
      get {
        return _message;
      }
      set {
        // エコー)
        // エコー）だけはぼうよみちゃん側で登録が必要なので排除する
        // そのほかはVoiceEffectsに登録されている
        string buff = value;

        if (pronmessage == null)
        {
          //pronmessage = buff;
          // cpronmessage = Yomigana.getYomigana(buff);
        }
        if (buff.Contains("エコー）") == true)
        {
          buff = buff.Replace("エコー）", "");
        }
        // これでエコー）は消えるはず
        buff = VoiceEffects.removevoecode(buff);
        _message = buff;

        // 実際は発音するpronmessageを使用するべき
        // TODO  だから、pronmeesageが設定されたときに計算するべき
        length = value.Length;
        // TODO 文字が長い場合、改行する
        // ファイル名がおかしいとaviu側で読み込みエラーとなるもよう
        // ファイル名を少し変更したほうがいいかも
        // TODO 自動で改行したい
        // 限界文字　"そうだねー富岡の南にある赤系工場では場所がない"
        // 23文字 ２０文字
        // ４０文字を超える場合、警告
        // ２段、２行
        // １行最大　２０文字、２段最大　４０文字
        if (buff.Length > Quote.MaxMessage)
        {
          Logs.write("字幕の最大長を超えています[" + buff.Substring(0, 5), Logs.Logtype.dispwarn);
        }
        if (buff.Length > Quote.MaxMessageLine)
        {
          // 改行処理を行う
          // コンマで改行処理を行う
          int i = buff.IndexOf("、");
          if (i > 0)
          {
            string[] ls = buff.Split(new string[] { "、" }, StringSplitOptions.RemoveEmptyEntries);
            // 
            if (ls.Length == 2)
            {
              // コンマがひとつの場合のみ、コンマの位置で改行処理を行う
              // コンマが含まれている場合のみ処理する
              // 長さが２０文字以上の場合、こんまを取り除き、改行を行う
              // 複数のコンマが入っている場合、どうするか？
              // 最初のコンマのみ処理する？　”あっ、、、”などで使う
              // 
              if (ls[0].Length < Quote.MaxMessageLine && ls[1].Length < Quote.MaxMessageLine)
              {
                // こんまは取り除く
                _message = buff.Substring(0, i) + "\r\n" + buff.Substring(i + 1);
              }
            }
            // こんまの位置が微妙な場合、どちらかに文字がかたよる
            // この場合は半分でぶつきりするか？
            // 打ち上げに必要なロケット燃料などの精油素材を
            // まとめて生産するぜ
            // 上記のような場合、どうするか？
            else if (ls[0].Length > Quote.MaxMessageLine)
            {
              // おにがへやのとをからでまで ２０文字周辺の格助詞を検索して、そこで改行する？
              // -> 何度も検索して改行位置を調整する必要がある
              // ロジック的に難しいし、現実的ではない
              // こんまいりで設定し、こんまの次に改行をいれて、aviutilで編集させる
              _message = buff.Substring(0, i + 1) + "\r\n" + buff.Substring(i + 1);

            }
          }
        }

      }
    }
    public string voicecode {
      get {
        return _voiceid;
      }
      set {
        _voiceid = value;
        //if (Globals.voicelist.ContainsKey(value) == false)
        //{
        //  Logs.write("voiceperson not defined voice list[" + value + "]");
        //  return;
        //}
        // SLibVoics v = Globals.voicelist[value];
        //SLibVoics v = 
        //// voiceref = v;
        //Chara c = Globals.charas.charabyid[value];
        //// speed = v.speed;
        //// tone = v.tone;
        //// volume = v.volume;
        //speed = c.default_speed;
        //tone = c.default_tone;
        //volume = c.default_volume;

      }
    }


    public static void Clear()
    {
      counter = 0;
    }

    // playはeffect経由で行ったほうがよい?
    // chara毎にvoiceが決まり、speed,toneが決まる
    // 保存されている phonetictext or text(message)のplay
    public bool play(Globals.ePlaywait mode = Globals.ePlaywait.wait)
    {
      //if (voiceref == null)
      //{
      //  voiceref = Globals.voicelist[voicecode];
      //}
      //// 発音文字が設定されてる場合はそれを使う
      //// 設定されていない場合はテキストをそのまま使う

      //string buff = pronmessage == "" ? message : pronmessage;

      //// buffの中に発声効果記号がないか判定
      //// 途中で効果が変更になる場合は、現状のところ対応できない？
      //// 改行をいれればいいのかな？
      //// いや、スピードや音声などはipcで設定できるが、
      //// デフォルト値を渡してwavを出力させている
      //// 自前化ができるまでは無理か
      //// List<VoiceEffect> ve = VoiceEffects.getEffect()とかして
      //// 発声文字より効果記号を判定し、効果音を設定する
      //// んで、それをvoiceref.playに引数としてそのまま渡す
      //// 自前化した場合は、play内で効果記号を判定して都度、aquestalkに効果を設定する
      //// wavへ出力する処理はどうなってるんだろ
      //// 別で呼び出しているはず　ipcのsaveだ

      //voiceref.play(buff);
      //if (mode == Globals.ePlaywait.wait)
      //{
      //  if (Globals.boyomi.NowPlaying == false)
      //  {
      //    return true;
      //  }
      //  do
      //  {

      //  } while (Globals.boyomi.NowPlaying == true);
      //}


      return true;

    }

    private const int Default_filemessagelen = 5;

    // wavファイルの作成 & soud play
    public bool make(Globals.Makemode arg = Globals.Makemode.noplay)
    {
      //if (voiceref == null)
      //{
      //  voiceref = Globals.voicelist[voicecode];
      //}
      //if (pronmessage == null)
      //{
      //  pronmessage = message;
      //}

      //// 音を再生せず、棒読みちゃんへ保存指示を行ってかんしくんでaviutilに連携
      //// さらに表情を連携させたい
      //// かんしくんでexoファイルをどのように作ってるか調査する必要がある
      //// ファイル名 210602_221010_女性２_もうだめかとおもっ…

      //string vname = voiceref.sourcename;
      //// Logs.write(vname);
      //DateTime dt = DateTime.Now;
      //string timesp = dt.ToString("yyyyMMdd_HHmmss");

      //// TODO ファイル名が不正でavi側で読み込みエラーがおきてるのかもしれない
      //// ファイル名の正規化、文字数を短くする

      //// ファイル名のメッセージ部として先頭１０文字から発音記号を削除したものを使用する
      //int i = message.Length;
      //if (i > Default_filemessagelen)
      //{
      //  i = Default_filemessagelen;
      //}

      //// TODO fmsgよりファイル名に使えない文字を削除
      //string fmsg = Voice.removeaqueschar(message.Replace("〜", "～").Replace("\r", "").Replace("\n", ""));
      //fmsg = fmsg.Replace("、", "").Replace("\"", "");
      //if (fmsg.Length > i)
      //{
      //  fmsg = fmsg.Substring(0, i);
      //}

      ////　エスケープ文字に"を追加
      //string fname = Globals.voicefolder + @"\" + timesp + "_" + voiceref.dispname + "_" + fmsg;
      //// Logs.write(fname);

      //// ファイル名に使えない/を削除
      //fname = fname.Replace("/", "");

      //voiceref.bouymi.wait();
      //Logs.write(voicecode + "," + message);


      //// wavとテキストを作成し、指定フォルダに保存
      //// 発声データをwavとしてfnameに保存
      //// messageをtxtとして保存
      //// System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      //// File.WriteAllText(fname + ".txt", message,System.Text.Encoding.GetEncoding("Shift_JIS"));
      //voiceref.save(pronmessage, fname + ".wav");
      //File.WriteAllText(fname + ".txt", message);
      //if (arg == Globals.Makemode.withplay)
      //{
      //  voiceref.play(pronmessage, arg);

      //}


      return true;
    }

  }
}
