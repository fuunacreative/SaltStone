using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;

namespace saltstone
{
  public class Zip
  {
    private string zipfile;

    public Zip(string arg)
    {
      zipfile = arg;
    }

    public bool extract()
    {
      // current directoryに展開
      return true;
    }
    public bool extract(string extractpath)
    {
      ReadOptions opt = new ReadOptions();
      //opt.StatusMessageWriter =
      opt.Encoding = System.Text.Encoding.GetEncoding(932);

      ZipFile zipf = ZipFile.Read(zipfile, opt);
      zipf.ExtractAll(zipfile, ExtractExistingFileAction.OverwriteSilently);
      zipf.Dispose();
      zipf = null;


      return true;
      
    }
  }
}
