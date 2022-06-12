
//build之前对原始资源的处理


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Build;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;
namespace YourNamespace.Tool
{

    public class RemoveAssetsWhenBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        //打包前将这两个目录从工程挪走，打包完成后再挪回来

        private readonly string[] removeAssetPathList =
        {
            "GameAsset"
        };

        public void OnPostprocessBuild(BuildReport report)
        {

            foreach (var source in removeAssetPathList)
            {
                if (!Directory.Exists(Path.Combine(System.Environment.CurrentDirectory, source)))
                    throw new System.Exception("拷贝文件夹异常，文件夹不存在 " + Path.Combine(System.Environment.CurrentDirectory, source));
                if (Directory.Exists(Path.Combine(Application.dataPath, source)))
                    throw new System.Exception("拷贝文件夹异常，目标已经存在该文件夹 " + Path.Combine(Application.dataPath, source));
                Directory.Move(Path.Combine(System.Environment.CurrentDirectory, source), Path.Combine(Application.dataPath, source));
                AssetDatabase.Refresh();
            }


        }

        public void OnPreprocessBuild(BuildReport report)
        {

            foreach (var source in removeAssetPathList)
            {
                if (!Directory.Exists(Path.Combine(Application.dataPath, source)))
                    throw new System.Exception("拷贝文件夹异常，文件夹不存在 " + Path.Combine(Application.dataPath, source));
                if (Directory.Exists(Path.Combine(System.Environment.CurrentDirectory, source)))
                    throw new System.Exception("拷贝文件夹异常，目标已经存在该文件夹 " + Path.Combine(System.Environment.CurrentDirectory, source));
                Directory.Move(Path.Combine(Application.dataPath, source), Path.Combine(System.Environment.CurrentDirectory, source));
                AssetDatabase.Refresh();
            }


        }
    }

}