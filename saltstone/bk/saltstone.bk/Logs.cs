using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;



namespace saltstone
{

    public static class Logs
    {
        public enum Logtype
        {
            dispwarn,
            dispinfo,
            error,
            info,
            debug
        }

        public static bool writelogflag = true;

        public static string exename = "";
        
        // 表示内容は、
        // exe,エラーソース、エラー行番号,type,メッセージ
        // テキストに書き込む場合は履歴管理が必要



        public static void write(string arg)
        {
            if (writelogflag == false) { return; }

            Console.WriteLine(arg);
        }

        public static void write(string mess ,Logtype lgtype = Logtype.info)
        {
            string exe = getexename();
        }

        private static string getexename()
        {
            if (exename == "")
            {
                exename = Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().Location);
            }
            return exename;
        }

        private static void getexetraceinfo(StackFrame sf , out string methodname , out int souceline)
        {
            System.Reflection.MethodBase callm = sf.GetMethod();
            methodname  = callm.Name;
            souceline  = sf.GetFileLineNumber(); // 呼び出し元の行番号を表示

        }

        public static void dispmessage()
        {
            string exe = getexename();
            const int findex = 2;
            StackFrame sf = new StackFrame(findex);
            // 呼び出し元のメソッド名を取得
        }






    }
}
