
//打包工具

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
namespace YourNamespace.Tool
{
    public class AutoAssetBundle
    {
        //源资源文件路径
        private static string sourceAssetPath;
        //AB包输出目录
        private static string baseOutputPath;
        //后缀名
        private static string suffix = ".ab";
        //只保留.ab文件
        private static bool abFileOnly = false;

        //只拷贝这个平台的AB包到应用程序路径
        //打包会生成IOS 和安卓 2个平台AB包，这个变量决定拷贝哪个平台的AB包到程序目录进行调试
        private static BuildTarget copyBuild = BuildTarget.Android;
        private static BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.IgnoreTypeTreeChanges | BuildAssetBundleOptions.None;

        public static System.Action<string> LogHander;

        [MenuItem("AssetBundle/全量打包")]
        public static int BuildAll()
        {
            sourceAssetPath = "Assets/GameAsset/Resources/";
            baseOutputPath = "Assets/StreamingAssets/Games/";

            //删除包名
            ClearAllABName();
            //重新设置包名
            SetAllABName(sourceAssetPath);
            //查找重复冗余资源 并移到到一个共有包
            MoveDuplicates(sourceAssetPath);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            int len = -1;
            try
            {
                BuildTarget[] buildTarget = new BuildTarget[] { BuildTarget.iOS,BuildTarget.Android};
                for (int i = 0; i < buildTarget.Length; i++)
                {
                    int _len = RunBuild(buildTarget[i], OnBuildOK);
                    if(len != -1 && len != _len)
                    {
                        Debug.LogError("打包出错 不同平台生成AB数不一致");
                        LogHander?.Invoke("打包出错 不同平台生成AB数不一致");
                    }
                    len = _len;

                }

            }
            catch (System.Exception e)
            {
                Debug.LogError("打包出错:" + e.ToString());
                LogHander?.Invoke("打包失败 详情看见控制台");
            }
            return len;
        }
        [MenuItem("AssetBundle/设置包名")]
        public static void SetAllABName()
        {
            sourceAssetPath = "Assets/GameAsset/Resources/";
            baseOutputPath = "Assets/StreamingAssets/Games/";
            ClearAllABName();
            SetAllABName(sourceAssetPath);

        }


        [MenuItem("AssetBundle/增量打包")]
        public static void BuildSelect()
        {
            ClearAllABName();
            SetSelectABName();
            try
            {
                BuildTarget[] buildTarget = new BuildTarget[] { BuildTarget.iOS, BuildTarget.Android };
                for (int i = 0; i < buildTarget.Length; i++)
                {
                    RunBuild(buildTarget[i], OnBuildOK);

                }
                ClearAllABName();

            }
            catch (System.Exception e)
            {
                Debug.LogError("打包出错:" + e.ToString());
                LogHander?.Invoke("打包失败 详情看见控制台");
            }

        }

        // 删除无用文件 拷贝到应用程序路径的后续操作
        private static void OnBuildOK(BuildTarget buildTarget)
        {
            string path = Path.Combine(baseOutputPath, buildTarget.ToString());
            File.Copy(Path.Combine(path, buildTarget.ToString()), Path.Combine(path, buildTarget.ToString() + suffix), true);
            File.Delete(Path.Combine(path, buildTarget.ToString()));
            if (abFileOnly) OnlySaveABFile(path);
            if (buildTarget == copyBuild)
            {
                //Copy2Local(source:path,target:Path.Combine(Application.persistentDataPath,"AssetBundle/AB"));
            }
            CreateVersionJSON(path);
            LogHander?.Invoke(buildTarget.ToString() + "版本文件生成成功");
        }


        private static int RunBuild(BuildTarget buildTarget, System.Action<BuildTarget> buildOK)
        {
            string outputPath = CreateOutputPath(buildTarget);
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, buildOptions, buildTarget);
            if (manifest == null)
            {
                Debug.LogError(buildTarget + "平台打包失败");
                LogHander?.Invoke("打包失败 详情看见控制台");
                return 0;
            }
            buildOK?.Invoke(buildTarget);
            int len = manifest.GetAllAssetBundles().Length;
            Debug.Log(buildTarget + "平台打包完成,生成AB数：" + len);
            LogHander?.Invoke("打包失败 详情看见控制台");
            AssetDatabase.Refresh();
            return len;

        }



        private static void SetAllABName(string source)
        {
            try
            {

            
                DirectoryInfo folder = new DirectoryInfo(source);
                FileSystemInfo[] files = folder.GetFileSystemInfos();
                int length = files.Length;
                for (int i = 0; i < length; i++)
                {
                    if (files[i] is DirectoryInfo)
                    {
                        SetAllABName(files[i].FullName);
                    }
                    else
                    {
                        if (!files[i].Name.EndsWith(".meta"))
                        {
                            SetABName(files[i].FullName);
                        }
                    }
                }
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.ToString());
                Debug.LogError(source);
            }

            void SetABName(string file)
            {
                
                string _source = file.Replace("\\", "/"); ;
                string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);
                string _assetPath2 = _source.Substring(Application.dataPath.Length + 1);
                AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
                string assetName = _assetPath2.Split('/')[3].ToLower() + suffix;
                if (assetImporter != null)
                {
                    assetImporter.assetBundleName = assetName;
                    assetImporter.assetBundleVariant = null;
                }
            }
        }


        private static void MoveDuplicates(string source)
        {
            List<string> rootAssets = new List<string>();
            FindRootAsset(source,rootAssets);
            List<string> allAssets = new List<string>();

            Dictionary<string,int> duplicatesAssetDic = new Dictionary<string, int>();
            foreach(var root in rootAssets)
            {   
               
                if(root.EndsWith(".DS_Store")) continue;
                Debug.Log("大资源:" + root);
                string[] subAssets = AssetDatabase.GetDependencies(root,false);
                allAssets.AddRange(subAssets.ToList());
            }
            for(int i = 0;i < allAssets.Count;i++)
            {
                for(int j = i + 1;j < allAssets.Count;j++)
                {
                    if(allAssets[i].EndsWith(".cs")
                    || allAssets[i].EndsWith(".lua") 
                    || allAssets[i].EndsWith(".DS_Store"))
                    {
                        continue;
                    } 

                    if(allAssets[i] == allAssets[j])
                    {
                        if(duplicatesAssetDic.ContainsKey(allAssets[i]))
                            duplicatesAssetDic[allAssets[i]] ++;
                        else
                            duplicatesAssetDic.Add(allAssets[i],1);
                        
                    }
                }
            }
            //冗余的资源被单独分离打包
            foreach(var kv in duplicatesAssetDic)
            {
                Debug.Log("重复项:" + kv.Key +  " " + kv.Value + "次");
                // AssetImporter assetImporter = AssetImporter.GetAtPath(kv.Key);
                // if (assetImporter != null)
                // {
                //     assetImporter.assetBundleName = "duplicates" + suffix;
                //     assetImporter.assetBundleVariant = string.Empty;
                // }
            }
            
        }
        private static void FindRootAsset(string source,List<string> objs)
        {
            try
            {
                DirectoryInfo folder = new DirectoryInfo(source);
                FileSystemInfo[] files = folder.GetFileSystemInfos();
                int length = files.Length;
                for (int i = 0; i < length; i++)
                {
                    if (files[i] is DirectoryInfo)
                    {
                        FindRootAsset(files[i].FullName,objs);
                    }
                    else
                    {
                        if (!files[i].Name.EndsWith(".meta"))
                        {
                            string _source = files[i].FullName.Replace("\\", "/"); ;
                            string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);
                            objs.Add(_assetPath);
                        }
                    }
                }
                
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.ToString());
                Debug.LogError(source);
            }

        }
        [MenuItem("AssetBundle/删除所有AB名")]

        private static void ClearAllABName()
        {
            string[] oldAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            int length = oldAssetBundleNames.Length;

            for (int j = 0; j < oldAssetBundleNames.Length; j++)
            {
                AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
            }
            AssetDatabase.Refresh();
        }


        private static void SetSelectABName()
        {
            try
            {

                int deep = 2;
                Object[] asset = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
                for (int i = 0; i < asset.Length; i++)
                {
                    string path = AssetDatabase.GetAssetPath(asset[i]);
                    if (asset[i].GetType() != typeof(DefaultAsset))
                    {
                        path = AssetDatabase.GetAssetPath(asset[i]);
                        string[] _path = path.Split('/');
                        string abName = _path[deep].ToLower() + suffix;
                        AssetImporter ai = AssetImporter.GetAtPath(path);
                        ai.assetBundleName = abName;
                    }
                }
            }
            catch
            {
                Debug.LogError("设置包名失败");
                LogHander?.Invoke("设置包名失败");
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }

        private static void DelSelectABName()
        {
            try
            {

                Object[] asset = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
                for (int i = 0; i < asset.Length; i++)
                {

                    if (asset[i].GetType() != typeof(DefaultAsset))
                    {
                        string path = AssetDatabase.GetAssetPath(asset[i]);
                        AssetImporter ai = AssetImporter.GetAtPath(path);
                        ai.assetBundleName = string.Empty;

                    }
                }
            }
            catch
            {
                Debug.LogError("清空包名失败");
                LogHander?.Invoke("清空包名失败");
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }

        private static string CreateOutputPath(BuildTarget buildTarget)
        {
            string outputPath = Path.Combine(baseOutputPath, buildTarget.ToString());
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);
            Directory.CreateDirectory(outputPath);
            //Debug.Log(buildTarget.ToString() + "AB输出路径:" + outputPath);
            return outputPath;

        }

        private static void Copy2Local(string source, string target)
        {
            try
            {
                Debug.Log("拷贝AB到程序持久存储目录," + "只拷贝.ab文件：" + abFileOnly);
                string[] files = Directory.GetFiles(source);
                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = Path.GetFileName(files[i]);
                    string targetFileName = Path.Combine(target, fileName);
                    File.Copy(files[i], targetFileName, overwrite: true);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("拷贝失败");
                Debug.LogError(e.ToString());
            }

        }
        private static void OnlySaveABFile(string source)
        {
            Debug.Log("删除非.ab格式文件");
            string[] files = Directory.GetFiles(source);
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                if (!Path.GetExtension(file).Equals(".ab"))
                {
                    //Debug.Log("del..." + file);
                    File.Delete(file);
                }
            }
        }
        private static void CreateVersionJSON(string path)
        {
            //选择所有需要打包的资源类型
            string[] files = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(t => (t.EndsWith(".ab"))).ToArray();
            AssetBundleDownloadInfoList infos = new AssetBundleDownloadInfoList();
            //为每一个文件生成相应的xml
            foreach (var file in files)
            {

                FileInfo fileInfo = new FileInfo(file);
                
                infos.infos.Add(new AssetBundleDownloadInfo()
                {
                    name = fileInfo.Name,
                    md5 = MD5File(file),
                    size = fileInfo.Length

                });
            }
            Debug.Log(JsonUtility.ToJson(infos));
            File.WriteAllText(Path.Combine(path, "version.json"), JsonUtility.ToJson(infos));
        }
        /// <summary>
        /// 生成version.xml文件
        /// </summary>
        /// <param name="xmlPaths"></param>
        /// 
        private static void CreateVersionXml(string path)
        {

            //xml声明
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
            //生成根节点
            xml.AppendChild(xml.CreateElement("Root"));
            //生成大版本号
            XmlNode root = xml.SelectSingleNode("Root");
            XmlElement verNode = xml.CreateElement("version");

            //版本号这里写死，如有需要，自由赋值
            verNode.InnerText = "2.0";

            root.AppendChild(verNode);
            //创建res节点
            XmlElement resNode = xml.CreateElement("res");
            root.AppendChild(resNode);
            //选择所有需要打包的资源类型
            string[] files = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(t => (t.EndsWith(".ab"))).ToArray();

            //为每一个文件生成相应的xml
            foreach (var file in files)
            {
                Debug.Log(file);
                FileInfo fileInfo = new FileInfo(file);

                AddNodeToXML(xml, fileInfo.Name, MD5File(file), fileInfo.Length);
            }

            //将xml文件写入res目录下
            xml.Save(path + @"/version.xml");

            //System.Diagnostics.Process.Start(path, path);
        }
        /// <summary>
        /// 计算md5码
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string MD5File(string file)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("md5file() fail, error:" + ex.Message);
            }
        }
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        /// <param name="md5Str"></param>
        /// <param name="length"></param>
        private static void AddNodeToXML(XmlDocument xml, string name, string md5Str, long size)
        {
            //获取根节点  
            XmlNode resNode = xml.SelectSingleNode("Root/res");
            //添加元素  
            XmlElement element = xml.CreateElement("res");
            element.SetAttribute("name", name);
            element.SetAttribute("size", size.ToString());
            element.InnerText = md5Str;
            resNode.AppendChild(element);
        }


    }



    // [System.Serializable]
    // public class AssetBundleDownloadInfos
    // {
    //     public List<AssetBundleDownloadInfo> infos = new List<AssetBundleDownloadInfo>();
    // }
    // [System.Serializable]
    // public class AssetBundleDownloadInfo
    // {
    //     public string name;
    //     public long size;
    //     public string md5;
    // }
}

