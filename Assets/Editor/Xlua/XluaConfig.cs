using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XLua;
using UnityEditor;

public static class XluaConfig
{
    //lua中要使用到C#库的配置，比如C#标准库，或者Unity API，第三方库等。
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>() {
        
                //UGUI
                typeof(UnityEngine.UI.Text),
                typeof(UnityEngine.UI.Button),
                typeof(UnityEngine.UI.Slider),
                typeof(UnityEngine.RectTransformUtility),
                typeof(UnityEngine.RectTransform),
                typeof(UnityEngine.Canvas),

                //c#
                typeof(System.Object),
                typeof(System.IO.Path),
                typeof(System.Collections.Generic.List<int>),
                typeof(Action<string>),
                typeof(System.Action<float>),

                typeof(UnityEngine.Object),

                typeof(Vector2),
                typeof(Vector3),
                typeof(Vector4),
                typeof(Quaternion),

                typeof(Color),
               
                typeof(Time),
                typeof(UnityEngine.Application),
                typeof(UnityEngine.RuntimePlatform),
                typeof(UnityEngine.Debug),

                typeof(GameObject),
                typeof(Component),
                typeof(Behaviour),
                typeof(Transform),
                typeof(MonoBehaviour),
                typeof(LayerMask),

                //animation
                typeof(AnimationClip),
                typeof(Animation),
                typeof(RuntimeAnimatorController),
                
                //render
                typeof(ParticleSystem),
                typeof(SkinnedMeshRenderer),
                typeof(MeshRenderer),
                typeof(Renderer),
                typeof(Light),

                typeof(Mathf),

                

                typeof(AudioSource),
                typeof(UnityEngine.AudioClip),

                typeof(UnityEngine.SpriteMask),

                
                typeof(UnityEngine.RenderSettings),
                typeof(UnityEngine.Rendering.AmbientMode),
                typeof(UnityEngine.Resources),

                
                
                //物理
                typeof(UnityEngine.Physics),
                typeof(UnityEngine.RaycastHit),
                typeof(UnityEngine.Collider),
                typeof(UnityEngine.BoxCollider),
                typeof(Ray),
                typeof(Bounds),
                //事件
                typeof(UnityEngine.EventSystems.EventTrigger),
                typeof(UnityEngine.EventSystems.EventTriggerType),
                typeof(UnityEngine.EventSystems.Physics2DRaycaster),
                typeof(UnityEngine.EventSystems.PhysicsRaycaster),

                


                // TTweening
                // typeof(DG.Tweening.Ease),
                // typeof(DG.Tweening.Tween),
                // typeof(DG.Tweening.TweenSettingsExtensions),
                // typeof(DG.Tweening.ShortcutExtensions),
                // typeof(DG.Tweening.TweenExtensions),

                typeof(YourNamespace.CSBehaviour),
                typeof(YourNamespace.LuaLoadAsset),
                typeof(YourNamespace.UnityHelper),
                typeof(YourNamespace.TimeHelp),

    

            };

    //C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface
    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>() {
                typeof(Action),
                typeof(Func<double, double, double>),
                typeof(Action<string>),
                typeof(Action<double>),
                typeof(UnityEngine.Events.UnityAction),
                typeof(System.Collections.IEnumerator),

            };

    // //热修复
    [Hotfix]
    public static List<Type> MyHotfix = new List<Type>() {
                
                
            };

    //黑名单
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
                new List<string>(){"System.Xml.XmlNodeList", "ItemOf"},
                new List<string>(){"UnityEngine.WWW", "movie"},
    #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
                new List<string>(){"UnityEngine.WWW", "MovieTexture"},
                new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
                new List<string>(){"UnityEngine.Input", "IsJoystickPreconfigured","System.String"},
                new List<string>(){"UnityEngine.Physics", "Raycast","UnityEngine.Ray"},


                new List<string>(){"UnityEngine.Light", "shadowRadius"},
                new List<string>(){"UnityEngine.Light", "SetLightDirty"},
                new List<string>(){"UnityEngine.Light", "shadowAngle"},
                new List<string>(){"UnityEngine.MeshRenderer", "scaleInLightmap"},
                new List<string>(){"UnityEngine.MeshRenderer", "receiveGI"},
                new List<string>(){"UnityEngine.MeshRenderer", "stitchLightmapSeams"},
                new List<string>() {"UnityEngine.UI.Text", "OnRebuildRequested"}, 
            };

}
