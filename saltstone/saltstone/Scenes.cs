using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Threading.Tasks;

namespace saltstone
{



  public class Scenes
  {
    public enum CharaDrawMode
    {
      BySerif,
      AllMovie
    }

    public int id; // おそらくtreeの追加順
    public int parentid;
    public string scenename;
    public List<Quote> messages;
    public static int counter; // sceneを追加するたびにincrement
    public CharaDrawMode charadrawmode;

    public string getCharadrawMode_disp()
    {
      string buff = "デフォルト";
      switch (charadrawmode)
      {
        case CharaDrawMode.BySerif:
          buff = "セリフ毎";
          break;
        case CharaDrawMode.AllMovie:
          buff = "動画全体";
          break;
      }
      return buff;
    }

    public static void Clear()
    {
      counter = 0;
    }
    

    public Scenes()
    {
      id = counter;
      counter += 1;
      messages = new List<Quote>();
      charadrawmode = CharaDrawMode.AllMovie;
    }

    public bool play(Globals.Filesavemode mode = Globals.Filesavemode.nosave)
    {
      foreach (Quote q in messages)
      {
        // Logs.write(Globals.batchstop.ToString());
        if (Globals.batchstop == true)
        {
          Globals.batchstop = false;
          return false;
        }
        q.play(Globals.ePlaywait.wait);
      }

      return true;

    }
    public bool make(Globals.Makemode arg = Globals.Makemode.noplay)
    {
      foreach (Quote q in messages)
      {
        if (Globals.batchstop == true)
        {
          Globals.batchstop = false;
          return false;
        }
        q.make(arg);
      }
      return true;

    }

    public int length {
      get {
        int count = 0;
        foreach (Quote q in messages)
        {
          count += q.length;
        }
        return count;
      }


    }

  }


}
