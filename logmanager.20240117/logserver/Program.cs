using saltstone;
using System;
using System.Threading;


namespace logserver
{
  /*
  internal static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      // To customize application configuration such as set high DPI settings or default font,
      // see https://aka.ms/applicationconfiguration.
      ApplicationConfiguration.Initialize();
      Application.Run(new Form1());
    }
  }�@
  */
  static class Program
  {
    // private static string _taskid;

    /// <summary>
    /// �A�v���P�[�V�����̃��C�� �G���g�� �|�C���g�ł��B
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      try
      {
        // exe singleton
        // mutex���g�����A�A�A getprocessbyname���g�����A�A�A
        // �ЂƂ����s�����ꍇ�Agetprosessbyname�͂ЂƂ��q�b�g����

        // ���ł�logserver�����s����Ă��邩�m�F����
        string exename = Utils.Sysinfo.getExeName();
        int cnt = Utils.checkrunexec(exename);
        Logs? lbuff = null;

        #region argument ����
        if (args.Length > 0)
        {
          lbuff = new Logs();
          lbuff.logdate = Utils.getNowDatetime();
          // �N��������
          // message , exename , logtype, method, sourceline, trace,
          // string , string , (info,warn,debug,error,fatal) , 
          // exename��� exe ver���擾����
          // log date��current_date
          // args�͎��sexe�͊܂܂Ȃ�
          for (int i = 0; i < args.Length; i++)
          {
            switch (i)
            {
              case 1:
                lbuff.message = args[i];
                break;
              case 2:
                lbuff.exename = args[2];
                break;
              case 3:
                // logtype
                switch (args[3])
                {
                  case "debug":
                    lbuff.logtype = Logs.Logtype.debug;
                    break;
                  case "info":
                    lbuff.logtype = Logs.Logtype.info;
                    break;
                  case "warn":
                    lbuff.logtype = Logs.Logtype.warn;
                    break;
                  case "error":
                    lbuff.logtype = Logs.Logtype.error;
                    break;
                  case "fatal":
                    lbuff.logtype = Logs.Logtype.fatal;
                    break;
                }
                break;
              case 4:
                lbuff.method = args[4];
                break;
              case 5:
                int j;
                bool ret = int.TryParse(args[5], out j);
                if (ret == true)
                {
                  lbuff.sourceline = j;
                }
                break;
              case 6:
                lbuff.trace = args[6];
                break;
            }
          }
        }
        #endregion


        #region program argument proc
        //���łɎ��s����exe������̂ŁA���������Ƃ�pipe��send���s��
        if (cnt > 1)
        {
          // ���s���̂��̂����� => ���������Ƃ�log�o�͂��s��
          // class logs��g�ݗ���

          // logserver���s���Ȃ̂ŁApipe�ő��M����
          if( lbuff != null)
          {
            lbuff.send();
          }

          // ���O�o�͌�A�I��
          // exe count��1�ȏ゠��̂ŁA���C���ƂȂ�logserver�͓����Ă���
          // ���̂��� log send�������s���΂悢

          // if arg is pass -> log send by ipc
          // arg1 -> message only
          // arg2 -> error , message
          // arg3 -> exe,error,message
          // arg4 -> exe,error,message,stacktrace ? 
          // arg1 and file.json -> json parse and send log by ipc

          // return��finally�����s�����
          return;
        }
        #endregion
        // ���������� + exe�������Ă��� -> ���O�o�� + ��ʕ\��
        // process�������Ă���ꍇ��logs.send���s����B
        // �����ł́A�������w�肳��Ă���ꍇ�Alogserver���N����
        // �����̃t�@�C������������
        if( lbuff != null)
        {
          lbuff.write();
        }


        // logserver���N�� namedpipeno�������[�v�ɓ���
        // �I���͉�ʂ�close�{�^�����������܂�
        // ���������win form�̓���ƂȂ�


        // 
        #region logserver loop
        // logmanager�̃��O��t���[�v�����s

        // ���ɋN�����Ă���ꍇ��log������������
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new frmLogView());

        // global init���K�v���A�A�A�A
        Globals.init();
        // Logs.init();

        // revloop�͕K�v�Ȃ����� -> event rise�����̂ł́H
        // _taskid = Tasks.createTask(revloop);
        // Logs.IPCReieveLog();
        using (LogServer ls = LogServer.getInstance())
        {
          // ls.addevent();
          ls.initServer();
          // ipc��send rev�����܂������Ă��Ȃ��݂���


        }

        // logserver��close����
        // ls.Dispose();
        #endregion

      }
      catch (Exception e)
      {
        Logs.write(e);
        // logs��quque���m���ɏ����o��
        // global dispose -> logs dispose -> ququq��empty�ɂȂ�܂ŏ����o���͂�
      }
      finally
      {
        Globals.Dispose();
      }
    }
    /*
    public static void revloop()
    {
      while (true)
      {
        IPCLog l =  Logs.IPCReieveLog();
        // Ipclogjob_recieveevent�@������write�������s��
        Utils.sleep(500); // 0.5s�Ɉ�x�Alow level log��queue����������
      }
    }
    */

  }

}


