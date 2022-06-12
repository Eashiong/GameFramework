

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace YourNamespace
{
    public static class UnityHelper
    {
        /// <summary>
        /// 获取动画剪辑长度
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="clipName"></param>
        public static float GetAllClipLen(this Animator animator)
        {
            float length = 0;
            try
            {
                AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
                foreach (AnimationClip clip in clips)
                {
                    length += clip.length;
                }
            }
            catch (System.NullReferenceException)
            {
                Log.Yellow("Comm", "动画控制器或动画剪辑为null");
                length = 0;
            }
            return length;
        }

        public static void SetLayer(GameObject root, int layer)
        {
            root.layer = layer;
            var ts = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < ts.Length; i++)
            {
                ts[i].gameObject.layer = layer;
            }
        }
    }
}
