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
      // TODO logserver���N�����Ă��Ȃ����write
      // �N�����Ă����send���s��
      // send�������ɋN�����邩�ǂ����A�A�A
      // win C#���ƋN������΂悢���Aweb���ƋN�����Ȃ������悢
      // �Ȃ̂ŁA�N���C�A���g���ɂ܂�����
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


      // TODO log4net���g���Ȃ����H

      // logserver l 
      // l.init

    }
  }
}
