using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.International;
using Microsoft.International.Collections;
using System.Runtime.InteropServices;


namespace saltstone
{
    class Yomigana
    {
        private static object objForLock = new object();

        public static string getYomigana(string arg)
        {
            string buff = "";
            // TextSegments segments = new HanIdeographSequenceBoundaries("今日は良い天気。");
            // TODO 今日は良い -> きょうはりょういになり良が"りょう"と変換される
            TextSegments segments = new HanIdeographSequenceBoundaries(arg);
            StringAnnotations stringAnnotations = new StringAnnotations(segments);
            int index = 0;

            // Expected: 
            //    今日(きょう)
            //    は
            //    良(よ)
            //    い
            //    天気(てんき)
            //    。
            // Console.WriteLine("AutoAssignYomiganaSample: ");
            foreach (string source in segments)
            {
                stringAnnotations[index++] = GetYomiByIME(source, NativeMethods.FELANG_CMODE_HIRAGANAOUT);
            }

            if (segments.Count == stringAnnotations.Count)
            {
                index = 0;
                int count = segments.Count;
                while (index < count)
                {
                    if (stringAnnotations[index] == String.Empty)
                    {
                        //Console.WriteLine(segments[index]); 
                        buff += segments[index];
                    }
                    else
                    {
                        //Console.WriteLine("{0}({1})", segments[index], stringAnnotations[index]);
                        buff += stringAnnotations[index];
                    }
                    index++;
                }
            }


            return buff;
           
        }

        public static string GetYomiByIME(string text, int yomiMode)
        {
            IntPtr ppResult_Yomi = IntPtr.Zero;
            string yomi = string.Empty;
            int hResult_Yomi;
            IFEInterface ifeInterface = null;


            lock (objForLock)
            {
                Guid clsid = new Guid();
                int hr = NativeMethods.CLSIDFromString("MSIME.Japan", ref clsid);
                if (hr != NativeMethods.S_OK)
                    throw Marshal.GetExceptionForHR(hr);
                Object temp;

                Guid iid = new Guid("21164102-C24A-11d1-851A-00C04FCC6B14");//IFELanguage2


                hr = NativeMethods.CoCreateInstance(ref clsid, IntPtr.Zero, NativeMethods.CLSCST, ref iid, out temp);
                if (hr != NativeMethods.S_OK)
                {
                    return text;
                }

                ifeInterface = (IFEInterface)temp;

                ifeInterface.Open();

                hResult_Yomi = 0;
                try
                {
                    hResult_Yomi = ifeInterface.GetJMorphResult(NativeMethods.FELANG_REQ_REV,
                                                          yomiMode,
                                                          text == null ? 0 : text.Length,
                                                          text,
                                                          IntPtr.Zero,
                                                          out ppResult_Yomi);
                    if (hResult_Yomi == NativeMethods.S_OK)
                    {
                        NativeMethods.MORRSLT morrslt = (NativeMethods.MORRSLT)Marshal.PtrToStructure(ppResult_Yomi, typeof(NativeMethods.MORRSLT));

                        // yomi = MORRSLTPtrToString(ppResult_Yomi);
                        yomi = Marshal.PtrToStringUni(morrslt.pwchOutput, morrslt.cchOutput);

                    }
                }
                finally
                {
                    if (ppResult_Yomi != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(ppResult_Yomi);
                    }
                }

                ifeInterface.Close();
            }

            return yomi;
        }
    }

}

