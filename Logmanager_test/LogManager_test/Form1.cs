using saltstone;
using System.ComponentModel.Design.Serialization;
using System.Text.Json;

namespace LogManager_test
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void cmdLogWrite_Click(object sender, EventArgs e)
    {
      //Logs l = new Logs();
      //l.message = "testmsg";
      //l.write();
      // TODO logserverが起動していなければwrite
      // 起動していればsendを行う
      // sendした時に起動するかどうか、、、
      // win C#だと起動すればよいが、webだと起動しない方がよい
      // なので、クライアント側にまかせる
      // exception test

      // string buff = l.data();
      try
      {
        throw new Exception("testexception");
      }
      catch (Exception ex)
      {
        //Logs l = new Logs(ex);
        Logs.write(ex);
        // sendはnamedpieを使用して logserverにlog送信を行う
        // なぜnamed pipeが動かないか調査しておく
        // programを終了し、disposeが動くと log server側でイベントが発生する
        // Logs.send(ex);
      }


      // log4netを使えないか？
      // メンテも終了しているし、使わない

      // logserver l 
      // l.init

    }
  }
}
