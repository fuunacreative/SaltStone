using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// IPCじゃないから、たぶん不要 dllを使うだけならね

namespace Utils
{
  public class SMutex : IDisposable
  {
    // private static Dictionary<string, System.Threading.Mutex> mutexes;
    // mutexを一元管理する仕組みが必要か？ -> おそらく不要

    private string mutexname;
    private System.Threading.Mutex _mutex;
    private bool lockflag;


    public SMutex(string arg)
    {
      // Dispose();
      mutexname = arg;
      // owner false -> 主導権を握るのはc++側
      _mutex = new System.Threading.Mutex(false, mutexname);
      lockflag = false;
    }

    public void Dispose()
    {
      if (_mutex != null)
      {
        try
        {
          if(lockflag == true)
          {
            _mutex.ReleaseMutex();

          }
        }
        catch (Exception e)
        {
          Logs.write(e);
        } finally
        {

        }
        _mutex.Dispose();
        _mutex.Close();
        _mutex = null;
      }
    }

    public enum enum_mutexwaitmode
    {
      wait5s,
      waitever
    }

    public bool lockmutex(enum_mutexwaitmode lockmode = enum_mutexwaitmode.wait5s)
    {
      bool ret = false;
      if (lockmode ==
        enum_mutexwaitmode.waitever)
      {
        ret = _mutex.WaitOne();
        lockflag = true;
        return ret;
      }
      if (lockflag == true)
      {
        return false;
      }
      try
      {
        lock (_mutex)
        {
          ret = _mutex.WaitOne(5000);
          if (ret == true)
          {
            lockflag = true;
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      return ret;
    }

    public bool releasemutex()
    {
      // 何度もreleaseがcalできるようにする
      if (lockflag == false)
      {
        return true;
      }
      lockflag = false;
      // releaseしたものに対して再度releaseするとエラーが発生する
      try
      {
        _mutex.ReleaseMutex();
      }
      catch (Exception ex)
      {
        Logs.write(ex);
        // throw new Exception(ex.Message);
      }
      return true;
    }
      

  }
}
