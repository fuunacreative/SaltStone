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
      // TODO logserver���N�����Ă��Ȃ����write
      // �N�����Ă����send���s��
      // send�������ɋN�����邩�ǂ����A�A�A
      // win C#���ƋN������΂悢���Aweb���ƋN�����Ȃ������悢
      // �Ȃ̂ŁA�N���C�A���g���ɂ܂�����
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
        // send��namedpie���g�p���� logserver��log���M���s��
        // �Ȃ�named pipe�������Ȃ����������Ă���
        // program���I�����Adispose�������� log server���ŃC�x���g����������
        // Logs.send(ex);
      }


      // log4net���g���Ȃ����H
      // �����e���I�����Ă��邵�A�g��Ȃ�

      // logserver l 
      // l.init

    }
  }
}
