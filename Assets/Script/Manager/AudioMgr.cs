//播放声音

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace YourNamespace
{


    [System.Serializable]
    public class AudioConfig
    {
        public List<Config> items;
        [System.Serializable]
        public class Config
        {
            public string name;
            public float volume;
            public bool loop;
        }


    }
    public class AudioMgr : MonoSingleton<AudioMgr>
    {
        // 所有音效对应的声音数据
        private Dictionary<string,AudioConfig.Config> configs;


        //音频源 背景专用
        private AudioSource audioBGM;

        public float BGMVolume { get; set; } = 1;
        public float SoundVolume { get; set; } = 1;

        protected override void OnInit()
        {
            //指定背景音乐的音频源
            this.audioBGM = gameObject.AddComponent<AudioSource>();
            //读取音频资源包
            InitAudioData();

        }
        private void InitAudioData()
        {
            TextAsset audioStr = AssetMgr.Ins.Load<TextAsset>("Config/audioConfig");

            Log.White("comm", "Audio配置读取成功 !!!" + audioStr);
            var audioConfig = JsonUtility.FromJson<AudioConfig>(audioStr.text);
            configs = new Dictionary<string, AudioConfig.Config>();
            for(int i = 0;i<audioConfig.items.Count;i++)
            {
                configs.Add(audioConfig.items[i].name,audioConfig.items[i]);
            }
        }
        //check资源
        private AudioClip LoadRes(string acName)
        {
            if (string.IsNullOrEmpty(acName))
            {
                Log.Red("comm", "音频名字错误 ======= ");
                return null;
            }

            return AssetMgr.Ins.Load<AudioClip>(acName);

        }


        //播放特效音乐函数：
        public void PlaySound(string clipName)
        {
            Debug.Log(clipName);
            AudioClip ac = this.LoadRes(clipName);
            if (ac == null) return;
            AudioSource audioSource;

            //遍历当前持有的AudioSource组件
            var audioSources = gameObject.GetComponents<AudioSource>();
            //audioSources[0]被BGM的播放占用，因此从[1]开始
            for (int i = 1; i < audioSources.Length; i++)
            {
                audioSource = audioSources[i];
                //当有音频源空闲时，则用其播放
                if (!audioSource.isPlaying)
                {
                    audioSource.loop = false;
                    audioSource.clip = ac;
                    audioSource.loop = configs.ContainsKey(clipName) && configs[clipName].loop;
                    audioSource.volume = SoundVolume * (configs.ContainsKey(clipName) ? configs[clipName].volume:1);
                    audioSource.Play();
                    return;
                }
            }

            //当没有多余的音频源空闲时，则创建新的音频源
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = configs.ContainsKey(clipName) && configs[clipName].loop;         
            audioSource.volume = SoundVolume * (configs.ContainsKey(clipName) ? configs[clipName].volume:1);
            audioSource.clip = ac;
            audioSource.Play();

        }
        public void StopSound(string clipName)
        {
            string assetName;
            try
            {
                string[] s = clipName.Split('/');
                assetName = s[1];
            }
            catch
            {
                assetName = null;
            }

            var audioSources = gameObject.GetComponents<AudioSource>();
            for (int i = 1; i < audioSources.Length; i++)
            {
                var audioSource = audioSources[i];
                if(audioSource.isPlaying && audioSource.clip.name == assetName)
                {
                    audioSource.Stop();
                    audioSource.loop = false;
                    audioSource.clip = null;
                }
            }
        }
        //播放BGM函数：
        public void PlayBGM(string acName)
        {
            AudioClip ac = this.LoadRes(acName);
            if (ac != null)
            {
                this.audioBGM.clip = ac;
                this.audioBGM.loop = !configs.ContainsKey(acName) || configs[acName].loop;
                this.audioBGM.volume = BGMVolume * (configs.ContainsKey(acName) ? configs[acName].volume:1);
                this.audioBGM.Play();
            }

        }
        
        //停止当前BGM的播放函数：
        public void StopBGM()
        {
            this.audioBGM.Stop();
            this.audioBGM.clip = null;
        }

    }

}
