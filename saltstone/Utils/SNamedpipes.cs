using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  /// <summary>
  /// namedpipeをとりまとめるクラス
  /// </summary>
  public static class SNamedpipes
  {
    private static Dictionary<string, SNamedpipeServer> _servers;
    private static Dictionary<string, SNamespipeClient> _clients;

    public static bool getServer(string pipename,out SNamedpipeServer spipe)
    {
      spipe = null;
      if(_servers == null)
      {
        _servers = new Dictionary<string, SNamedpipeServer>();
      }
      if(_servers.ContainsKey(pipename) == true )
      {
        spipe = _servers[pipename];
        return true;
      }
      _servers[pipename] = new SNamedpipeServer(pipename);
      spipe = _servers[pipename];
      return true;
    }
    public static bool getClient(string pipename, out SNamespipeClient cpipe)
    {
      cpipe = null;
      if (_clients == null)
      {
        _clients = new Dictionary<string, SNamespipeClient>();
      }
      if(_clients.ContainsKey(pipename) == true)
      {
        cpipe = _clients[pipename];
        return true;
      }
      _clients[pipename] = new SNamespipeClient(pipename);
      cpipe = _clients[pipename];
      return true;
    }

  }
}
