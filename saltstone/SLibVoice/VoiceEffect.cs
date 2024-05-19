using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// tone変更
// https://www.baku-dreameater.net/entry/2015/06/07/%25e3%2580%2590aquestalk%25e3%2580%2591%25e3%2582%2586%25e3%2581%25a3%25e3%2581%258f%25e3%2582%258a%25e3%2583%259c%25e3%2582%25a4%25e3%2582%25b9%25e3%2581%25ae%25e3%2583%2594%25e3%2583%2583%25ehttps://www.baku-dreameater.net/entry/2015/06/07/%25e3%2580%2590aquestalk%25e3%2580%2591%25e3%2582%2586%25e3%2581%25a3%25e3%2581%258f%25e3%2582%258a%25e3%2583%259c%25e3%2582%25a4%25e3%2582%25b9%25e3%2581%25ae%25e3%2583%2594%25e3%2583%2583%25e

// aqtalkの声のspeed,tone
// https://ytxty1.hatenablog.com/entry/ar1254993


namespace saltstone
{
    // セリフ毎につける音声エフェクトを管理するクラス
    // Quote内で保持される

    // base config内に保持されているエフェクトのリストを管理する
    public class VoiceEffects 
    {
        // singletonにする必要がある
        private Dictionary<string, VoiceEffect> _voedics;
        private static VoiceEffects _voes;

        public static VoiceEffects getInstance()
        {
            if(_voes ==  null)
            {
                _voes = new VoiceEffects();
                _voes._voedics = new Dictionary<string, VoiceEffect>();
            }
            return _voes;
        }
        public static void close()
        {
            if(_voes._voedics != null)
            {
                _voes._voedics.Clear();
                _voes._voedics = null;
            }
            _voes = null;
        }

        public static bool add(string key , VoiceEffect val)
        {
            if (_voes == null)
            {
                getInstance();
            }
            if (_voes._voedics.ContainsKey(key))
            {
                return true;
            }
            _voes._voedics.Add(key, val);
            return true;
        }
        public static VoiceEffect get(string key)
        {
            if (_voes == null)
            {
                getInstance();
            }
            if (_voes._voedics.ContainsKey(key) == false)
            {
                return null;
            }
            return _voes._voedics[key];
        }
        public static string removevoecode(string message)
        {
            string buff = message;
            if (_voes == null)
            {
                getInstance();
            }
            // 本来であれば、ぼうよみちゃんで登録されているエコー）についてもこっちで処理したい
            // でも、最終的にはaquestalkのdllを直接コールし、リバーブは自作する予定なので
            // とりあえずこれで問題ない
            // 制限としてはメッセージ中に音声効果記号は１つしかいれられない
            foreach (string k in _voes._voedics.Keys)
            {
                if(buff.Contains(k) == true)
                {
                    buff = buff.Replace(k, "");
                }
            }
            return buff;
        }


    }

    public class VoiceEffect
    {
        // public string key;
        public int volume;
        public int speed;
        public int tone;
        // reverbも再現したいが、自前でwav処理が必要
        // ぼうよみちゃんでは自前で処理を作成している
        // 変調もできると声の高低を上下に震動させるエフェクトがつけられる
        // NAudioでやれるみたいだが、、、いやできないな
        // directxを使うSharpDXを使うとできるみたいだが、、、
        // (Reverb a b c) … 残響(a:余韻時間、b:反復間隔、c:増幅率)
        // vibrato ヴィブラート その音の見かけの音高を保ちながら、その音の特に高さを揺らすことである。
        // unityではあるみたい

        // とりあえず、セリフに対して、上記の３つ（音量、速度、音程）を変化できればいいや
        // volumeは
        // -> 												bWavData = WaveUtility.ToStereo(bWavData, volumeL, volumeR);
        // dllからの出力を100とし、それに対して音byteの音量を調整している
        // sample 16bit ２byte
        // aquestalkのdllではbyteデータを出力している
        // 効果にあわせてwavデータを変更し、つなぎ合わせてwavに保存することは可能
        // 発声文字の途中で効果記号をいれることも可能ってことだ
        // ということは、発声効果データは実際のaquestalkのラッパーで使うことになる
        // sceneやquoteに持たせることはあまりよくない




    }
}
