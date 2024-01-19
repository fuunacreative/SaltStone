using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace saltstone
{
  public partial class frmSenario : Form
  {
    public frmSenario()
    {
      InitializeComponent();
    }

    private void cmdPhoneticPlay_Click(object sender, EventArgs e)
    {
      pFunc_PhoneticPlay(sender);
    }
    private void pFunc_PhoneticPlay(object sender)
    {
      // saltstonevoiceのc# libを作成し、ここで各種音声合成再生ライブラリにアクセスし、wavを作成して音声再生を行う
      //vocalization.Aquestalk_f1 f = new vocalization.Aquestalk_f1();

      //f.createwav("あいうえお", @"C:\Users\fuuna\Videos\voice\a.wav");
      Voicessoft vs = VoiceGlobal.voices;
      // Voicessoft vs = new Voicessoft();
      // vs.init();
      Voics v = vs.getVoice("AQF1"); // <- 現在選択中のcharaで定義されているvoiceid
      // slibvoiceに対して
      // text,param,outfを渡す
      // aqmemstructureのようなclassで渡すか,,,,
      string argtext = "今日はとても良い日でした！";
      VoiceText vtext = new VoiceText();
      vtext.speed = 110;
      Phonetic_aqmecab vp = new Phonetic_aqmecab();
      string testtext = vp.getPhonetic(argtext);

      vtext.text = argtext;
        
      vtext.phonetic = v.phoneticcnv.getPhonetic(argtext);
      string outf = @"C:\Users\fuuna\a.wav";
      vtext.outwavefile = outf;
      // outfをどう渡すか？
      //v.setParam("")
      bool ret = v.makeWave(vtext);
      if (ret == false)
      {
        return;
      }
      Utils.Sound.play(outf);

      // wav作成をどこで行うか？
      // scene , quote ?
      // voice , phonetic?
      // Voicetextに必要なspeed,tone,volなどのパラメータも必要


        
    }

    private void frmSenario_Load(object sender, EventArgs e)
    {
      Globals.init();

    }
  }
}
