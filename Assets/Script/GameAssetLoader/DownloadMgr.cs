
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace YourNamespace
{
    /// <summary>
    /// 异步下载
    /// </summary>
    public class DownloadAsync
    {
        /// <summary>
        /// 下载贴图
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<Texture> DownLoadTexture(string path)
        {
            Uri uri = URIHelper.Builder(path);
            using (UnityWebRequest unityWebRequest = new UnityWebRequest(uri))
            {
                DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
                unityWebRequest.downloadHandler = texDl;
                await unityWebRequest.SendWebRequest();
                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                    return null;
                return texDl.texture;  
            }
        }
        /// <summary>
        /// 下载文本
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> DownLoadText(string path)
        {
            Uri uri = URIHelper.Builder(path);
            using (UnityWebRequest unityWebRequest = new UnityWebRequest(uri))
            {
                unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                await unityWebRequest.SendWebRequest();
                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                {
                    Debug.LogError(unityWebRequest.error + "URI:" + path);
                    return null;
                }
                    
                return unityWebRequest.downloadHandler.text;  
            }
        }
        /// <summary>
        /// 下载文件到本地
        /// </summary>
        /// <param name="remoteurl"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public static async Task<string> DownloadFile(string remoteurl, string localPath)
        {
            Uri uri = URIHelper.Builder(remoteurl);
            using (var webRequest = UnityWebRequest.Get(uri))
            {
                webRequest.downloadHandler = new DownloadHandlerFile(localPath);
                await webRequest.SendWebRequest();
                if (webRequest.isHttpError || webRequest.isNetworkError || !string.IsNullOrEmpty(webRequest.error))
                {
                    YourNamespace.Log.Red("asset", string.Format("远程文件下载失败,\n服务器文件:{0}\n本地保存位置:{1}\n本地保存位置:{2}", remoteurl, localPath,webRequest.error));
                    return null;
                }
                else
                {
                    YourNamespace.Log.White("asset", string.Format("远程文件下载成功,\n服务器文件:{0}\n本地保存位置:{1}", remoteurl, localPath));
                    
                    return localPath;
                }
            }
        }
    }

    public static class ExtensionMethods
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }

    }

}
