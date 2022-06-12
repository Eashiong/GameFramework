
//辅助协程管理

using System.Collections;
namespace YourNamespace
{


    public class CoroutineMgr : MonoSingleton<CoroutineMgr> { }
    public static class __CoroutineMgrHelper
    {
        public static UnityEngine.Coroutine Start(this IEnumerator ator)
        {
            return CoroutineMgr.Ins.StartCoroutine(ator);
        }
        // public static void Stop(this IEnumerator ator)
        // {
        //     CoroutineMgr.Ins.StopCoroutine(ator);
        // }
        public static void Stop(this UnityEngine.Coroutine cor)
        {
            CoroutineMgr.Ins.StopCoroutine(cor);
        }
    }
}



//示例
// public class Test
// {
//     private void Fun()
//     {

//         //开始协程
//         var task = Wait().Start();

//         //停止
//         if(task!=null) task.Stop();

//         //或
//         Wait().Start();
//         Wait().Stop();

//     }

//     System.Collections.IEnumerator Wait()
//     {
//         yield return null;
//     }
// }