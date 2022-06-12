using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

namespace YourNamespace
{
    public class Loom : MonoBehaviour
    {
        public class MainThreadTask
        {
            //何时运行该函数
            public System.DateTime time;
            public Action action;

        }




        private List<Thread> threads;
        private static Loom current;
        private static readonly object lockObj = new object();

        private void Awake()
        {
            current = this;
            threads = new List<Thread>();
        }




        public static MainThreadTask RunOnMainThread(Action action, float time = 0)
        {
            if (time < 0)
            {
                Debug.LogError("time参数错误 函数不会被执行");
                return new MainThreadTask();
            }
            if (time != 0)
            {
                lock (current.allDelayed)
                {
                    MainThreadTask task = new MainThreadTask { time = System.DateTime.Now.AddSeconds(time), action = action };
                    current.allDelayed.Add(task);
                    return task;
                }
            }
            else
            {
                lock (current.allNoDelayed)
                {
                    MainThreadTask task = new MainThreadTask { time = System.DateTime.Now, action = action };
                    current.allNoDelayed.Add(task);
                    return task;
                }
            }
        }

        public static bool TryRemoveMainThreadTask(MainThreadTask task)
        {
            lock (current.allDelayed)
            {
                if (current.allDelayed.Contains(task))
                {
                    task.action = null;
                    return true;
                }
            }
            lock (current.allNoDelayed)
            {
                if (current.allNoDelayed.Contains(task))
                {
                    task.action = null;
                    return true;
                }
            }
            return false;
        }
        private static Thread loomThread;
        public static Thread RunOnAsync(Action active)
        {
            loomThread = new Thread(new ParameterizedThreadStart(RunAction));
            loomThread.Name = "Loom线程:" + System.Guid.NewGuid().ToString();
            Debug.Log("创建任务线程:" + loomThread.Name);
            loomThread.Priority = System.Threading.ThreadPriority.Lowest;
            loomThread.Start(active);
            current.threads.Add(loomThread);
            return loomThread;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            finally
            {
                current.threads.Remove(Thread.CurrentThread);
            }

        }
        public static void Sleep(int time)
        {
            if (current == null)
                return;
            if (Thread.CurrentThread == loomThread)
            {
                Thread.Sleep(time);
            }
        }

        public static void StopCurrent()
        {
            lock (lockObj)
            {
                if (current == null || loomThread == null)
                {
                    Debug.Log("线程已经全部销毁或当前线程不存在");
                    return;
                }
                string name = loomThread.Name;
                try
                {
                    current.threads.Remove(loomThread);
                    loomThread.Abort();
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
                finally
                {
                    loomThread = null;
                    Debug.Log("当前线程已经销毁:" + name);
                }
            }

        }
        public static void DestoryLoom()
        {
            if (current == null)
            {
                Debug.Log("线程已经全部销毁");
                return;
            }
            lock (lockObj)
            {
                if (current == null)
                    return;
                for (int i = 0; i < current.threads.Count; i++)
                {
                    Thread t = current.threads[i];
                    string name = t.Name;
                    try
                    {
                        t.Abort();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.ToString());
                    }
                    finally
                    {
                        Debug.Log("销毁线程:" + name);
                        t = null;
                    }
                }
                current.threads.Clear();
                current.allDelayed.Clear();
                current.allNoDelayed.Clear();
                current = null;
                Debug.Log("线程已经全部销毁");
            }

        }
        //延时和不延时区分开来的目的是保证不延时的函数立即执行 且减少linq查询
        private List<MainThreadTask> allDelayed = new List<MainThreadTask>();
        private List<MainThreadTask> currentDelayed = new List<MainThreadTask>();

        private List<MainThreadTask> allNoDelayed = new List<MainThreadTask>();
        private List<MainThreadTask> currentNoDelayed = new List<MainThreadTask>();


        // 消费主线程任务
        private void Update()
        {
            
            if (allNoDelayed.Count > 0)
            {
                lock (allNoDelayed)
                {
                    currentNoDelayed.Clear();
                    currentNoDelayed.AddRange(allNoDelayed);
                    allNoDelayed.Clear();
                }
                for (int i = 0; i < currentNoDelayed.Count; i++)
                {
                    currentNoDelayed[i].action?.Invoke();
                }
            }

            if (allDelayed.Count > 0)
            {
                lock (allDelayed)
                {
                    currentDelayed.Clear();
                    currentDelayed.AddRange(allDelayed.Where(d => d.time <= System.DateTime.Now));
                    for (int i = 0; i < currentDelayed.Count; i++)
                    {
                        allDelayed.Remove(currentDelayed[i]);
                    }
                }

                for (int i = 0; i < currentDelayed.Count; i++)
                {
                    currentDelayed[i].action?.Invoke();
                }
            }
        }
        private void OnDestroy() 
        {
            threads.Clear();
            allDelayed.Clear();
            currentDelayed.Clear();
            allNoDelayed.Clear();
            currentNoDelayed.Clear();

            currentNoDelayed = null;
            currentDelayed = null;
            allDelayed = null;
            allNoDelayed = null;
            threads = null;
            current = null;
            loomThread = null;

        }
    }
}