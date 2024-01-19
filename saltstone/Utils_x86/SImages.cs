using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace saltstone
{
  public static class SImages
  {
    public static Dictionary<string, Image> pimages;

    public static void Dispose()
    {
      if (pimages != null)
      {
        foreach (Image img in pimages.Values)
        {
          img.Dispose();
        }
        pimages = null;
      }
    }

    public static Image getImage(string fname)
    {
      Image img = null;
      if (pimages == null)
      {
        pimages = new Dictionary<string, Image>();
      }
      if (pimages.ContainsKey(fname) == true)
      {
        return pimages[fname];
      }

      img = Image.FromFile(fname);
      if (img == null)
      {
        return null;
      }
      pimages[fname] = img;
      return img;

    }
  }
}
