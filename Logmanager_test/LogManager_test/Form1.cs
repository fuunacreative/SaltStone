using saltstone;
using System.ComponentModel.Design.Serialization;

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

      Logs.init();

      try
      {
        throw new Exception("testexception");
      }
      catch (Exception ex)
      {
        // Logs.write(ex);
        Logs.send(ex);
      }


      // TODO log4netを使えないか？

      // logserver l 
      // l.init

    }
  }
}
