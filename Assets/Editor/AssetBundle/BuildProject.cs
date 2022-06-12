using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;
using BuildResult = UnityEditor.Build.Reporting.BuildResult;
public class BuildProject : Editor
{
    private static readonly string ProjectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
    private static readonly string apkPath = Path.Combine(ProjectPath, "Builds/" + Application.productName + ".apk");
    private static readonly string androidExportPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../android/unityLibrary"));
    private static readonly string iosExportPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../ios/UnityLibrary"));


    public static void BuildDebug()
    {
        #if UNITY_ANDROID
        BuildAndroidDebug();
        #elif UNITY_IOS
        BuildIOSDebug();
        #endif
    }
    private static void BuildAndroidDebug()
    {
        PlayerSettings.SetIl2CppCompilerConfiguration(
                            BuildTargetGroup.Android,
                            Il2CppCompilerConfiguration.Debug);
        if (Directory.Exists(apkPath))
            Directory.Delete(apkPath, true);

        if (Directory.Exists(androidExportPath))
            Directory.Delete(androidExportPath, true);

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        BuildOptions options = BuildOptions.Development | BuildOptions.ConnectWithProfiler;
        var report = BuildPipeline.BuildPlayer(
            GetEnabledScenes(),
            apkPath,
            BuildTarget.Android,
            options
        );

        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Build failed");


    }

    private static void BuildIOSDebug()
    {
        if (Directory.Exists(iosExportPath))
            Directory.Delete(iosExportPath, true);
        BuildOptions options = BuildOptions.Development | BuildOptions.ConnectWithProfiler;
        var report = BuildPipeline.BuildPlayer(
            GetEnabledScenes(),
            iosExportPath,
            BuildTarget.iOS,
            options
        );

        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Build failed");
    }
    static string[] GetEnabledScenes()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Where(s => !string.IsNullOrEmpty(s.path))
            .Select(s => s.path)
            .ToArray();

        return scenes;
    }
}