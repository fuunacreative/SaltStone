using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  [Serializable]
  public class AqcmdMemstrucure
  {
    public string command;
    public string jobid;
    public string voiceid;
    public int speed;
    public string phonetictext; // ひらがなである必要がある
    public string outwavfile;
    public string resultcode;
    public DateTime orderdate;
    public DateTime resultdate;
  }
}

/*
   jobid=10 // namedpipeのjobidと一致しているかどうか判定 -> resultへjobid ok/ngを返すのにも使用する
   voiceid=AQF1
   speed=100
   text="きょうもいいてんきでした。"
   wavfile=C:\Users\fuuna\a.wav
 */
