
//通知广播中心
using System;
using System.Collections;
using System.Collections.Generic;
namespace YourNamespace
{



    //接收者
    public class NotifyListener
    {

        //只带发送者的回调
        public Action<Object> callback { get; private set; } = null;
        //带float 和发送者的回调
        public Action<Object, float> callbackFloat { get; private set; } = null;
        //带一组参数和发送者的回调
        public Action<Object, object> callbackObj { get; private set; } = null;
        //带一组参数和发送者的回调
        public Action<Object, ArrayList> callbackArgs { get; private set; } = null;

        //要注册的通知类型和名字
        public string notifyType { get; private set; }

        public readonly int hashCode;

        //执行完后销毁
        public bool autoKill = false;
        public NotifyListener SetAutoKill()
        {
            autoKill = true;
            return this;
        }

        /// <summary>
        /// 生成一个通知接收对象 返回发送者信息
        /// </summary>
        public NotifyListener(NotifyType vmuseNotify, Action<object> callback)
        {
            this.notifyType = vmuseNotify.ToString();
            this.callback = callback;
            this.hashCode = callback.GetHashCode();
        }
        ///// <summary>
        ///// 生成一个通知接收对象 返回发送者信息 和一个float参数
        ///// </summary>
        //public NotifyListener(string notifyType, Action<object, float> callback)
        //{
        //    this.notifyType = notifyType;
        //    this.callbackFloat = callback;
        //}
        /// <summary>
        /// 生成一个通知接收对象 返回发送者信息 和一个object参数
        /// </summary>
        public NotifyListener(NotifyType vmuseNotify, Action<object, object> callback)
        {
            this.notifyType = vmuseNotify.ToString();
            this.callbackObj = callback;
            this.hashCode = callback.GetHashCode();
        }
        /// <summary>
        /// 生成一个通知接收对象 返回发送者信息 和一个ArrayList参数
        /// </summary>
        public NotifyListener(NotifyType vmuseNotify, Action<object, ArrayList> callback)
        {
            this.notifyType = vmuseNotify.ToString();
            this.callbackArgs = callback;
            this.hashCode = callback.GetHashCode();
        }
        public override string ToString()
        {
            if (this.callback != null)
                return "事件名:" + notifyType + "，方法名:" + this.callback.Method.ToString();
            if (this.callbackFloat != null)
                return "事件名:" + notifyType + "，方法名:" + this.callbackFloat.Method.ToString();
            if (this.callbackObj != null)
                return "事件名:" + notifyType + "，方法名:" + this.callbackObj.Method.ToString();
            if (this.callbackArgs != null)
                return "事件名:" + notifyType + "，方法名:" + this.callbackArgs.Method.ToString();
            return "事件名:" + notifyType + "，方法名 null";

        }


    }

    /// <summary>
    /// 广播通知中心
    /// </summary>
    public class NotifyMgr
    {
        private Dictionary<string, List<NotifyListener>> allListeners;
        //广播正在发出通知
        private string currentNotify = null;
        private static NotifyMgr _ins;
        public static NotifyMgr Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new NotifyMgr();
                }
                return _ins;
            }
        }

        private NotifyMgr()
        {
            allListeners = new Dictionary<string, List<NotifyListener>>();
            currentNotify = null;
        }
        //接收者注册
        public void Register(NotifyListener listener)
        {
            if (!string.IsNullOrEmpty(currentNotify))
            {
                Log.Yellow("Comm", "通知期间注册接收者不安全,notifyType:" + listener.notifyType);
                if (currentNotify.Equals(listener.notifyType))
                {
                    Log.Red("Comm", "通知期间无法注册同类型接收者,notifyType:" + listener.notifyType);
                    return;
                }
            }

            string key = listener.notifyType;
            if (allListeners.ContainsKey(key))
            {
                if (allListeners[key].Count > 0 && allListeners[key][0] != null)
                {
                    if (allListeners[key][0].hashCode == listener.hashCode)
                    {
                        Log.Red("Comm", "重复注册了方法:" + listener.ToString());
                    }
                }
                allListeners[key].Add(listener);
            }
            else
            {
                allListeners.Add(key, new List<NotifyListener>() { listener });
            }
        }
        //移除一个接收者
        public void Remove(NotifyListener listener)
        {
            if (!string.IsNullOrEmpty(currentNotify))
            {
                Log.Yellow("Comm", "通知期间移除接收者不安全,notifyType:" + listener.notifyType);
                if (currentNotify.Equals(listener.notifyType))
                {
                    Log.Red("Comm", "通知期间无法移除同类型接收者,notifyType:" + listener.notifyType);
                    return;
                }
            }
            try
            {
                string key = listener.notifyType;
                if (allListeners.ContainsKey(key))
                    if (allListeners[key].Contains(listener))
                        allListeners[key].Remove(listener);
                    else
                        Log.Yellow("Comm", "通知尝试移除一个不存在的接收者,notifyType:" + listener.notifyType);
                else
                    Log.Yellow("Comm", "通知尝试移除一个不存在的接收者,notifyType:" + listener.notifyType);
            }
            catch (System.Exception exc)
            {
                Log.Yellow("Comm", " 该监听事件不存在 " + exc);
            }
        }

        //带autokill集合
        private List<NotifyListener> autoRemoveList = new List<NotifyListener>();
        //移除自销毁接收者
        private void AutoRemove()
        {
            foreach (var listener in autoRemoveList)
            {
                string key = listener.notifyType;
                if (allListeners.ContainsKey(key))
                    if (allListeners[key].Contains(listener))
                        allListeners[key].Remove(listener);
                    else
                        Log.Yellow("Comm", "通知尝试移除一个不存在的接收者,notifyType:" + listener.notifyType);
                else
                    Log.Yellow("Comm", "通知尝试移除一个不存在的接收者,notifyType:" + listener.notifyType);
            }
            autoRemoveList.Clear();
        }

        //发出一个通知
        public void Notify(NotifyType vmuseNotify, object sender)
        {
            string notifyType = vmuseNotify.ToString();
            if (!string.IsNullOrEmpty(currentNotify))
            {
                Log.Yellow("Comm", "通知期间再次发出通知不安全,notifyType:" + notifyType);
                if (currentNotify.Equals(notifyType))
                {
                    Log.Red("Comm", "通知期间无法重复广播,notifyType:" + notifyType);
                    return;
                }
            }
            currentNotify = notifyType;
            if (allListeners.ContainsKey(notifyType))
            {
                List<NotifyListener> listeners = allListeners[notifyType];
                foreach (var listener in listeners)
                {
                    if (listener.callback != null)
                    {
                        Log.Blue("Comm", "发起通知,notifyType:" + notifyType);
                        listener.callback?.Invoke(sender);
                        if (listener.autoKill)
                        {
                            autoRemoveList.Add(listener);
                        }
                    }
                }
            }
            else
            {
                Log.Yellow("Comm", "没有收听者,notifyType:" + notifyType);
            }
            currentNotify = null;
            AutoRemove();
        }
        //发出一个通知 带float参数
        // public void Notify(string notifyType, object sender, float value)
        // {
        //     if (!string.IsNullOrEmpty(currentNotify))
        //     {
        //         Log.Yellow("Comm", "通知期间再次发出通知不安全,notifyType:" + notifyType);
        //         if (currentNotify.Equals(notifyType))
        //         {
        //             Log.Red("Comm", "通知期间禁止重复广播,notifyType:" + notifyType);
        //             return;
        //         }
        //     }
        //     currentNotify = notifyType;
        //     if (allListeners.ContainsKey(notifyType))
        //     {
        //         List<NotifyListener> listeners = allListeners[notifyType];
        //         foreach (var listener in listeners)
        //         {
        //             if (listener.callbackFloat != null)
        //             {
        //                 Log.Blue("Comm", "收到通知,notifyType:" + notifyType);
        //                 listener.callbackFloat?.Invoke(sender, value);
        //                 if (listener.autoKill)
        //                 {
        //                     autoRemoveList.Add(listener);
        //                 }
        //             }
        //         }
        //     }
        //     else
        //     {
        //         Log.Red("Comm", "没有收听者,notifyType:" + notifyType);
        //     }
        //     currentNotify = null;
        //     AutoRemove();
        // }
        //发出一个通知 带obj参数
        public void Notify(NotifyType vmuseNotify, object sender, object obj)
        {
            string notifyType = vmuseNotify.ToString();
            if (!string.IsNullOrEmpty(currentNotify))
            {
                Log.Yellow("Comm", "通知期间再次发出通知不安全,notifyType:" + notifyType);
                if (currentNotify.Equals(notifyType))
                {
                    Log.Red("Comm", "通知期间禁止重复广播,notifyType:" + notifyType);
                    return;
                }
            }
            currentNotify = notifyType;
            if (allListeners.ContainsKey(notifyType))
            {
                List<NotifyListener> listeners = allListeners[notifyType];
                foreach (var listener in listeners)
                {
                    if (listener.callbackObj != null)
                    {
                        //Log.Blue("Comm", "收到通知,notifyType:" + notifyType);
                        listener.callbackObj?.Invoke(sender, obj);
                        if (listener.autoKill)
                        {
                            autoRemoveList.Add(listener);
                        }
                    }
                }
            }
            else
            {
                Log.Yellow("Comm", "没有收听者,notifyType:" + notifyType);
            }
            currentNotify = null;
            AutoRemove();
        }
        //发出一个通知 带ArrayList参数
        public void Notify(NotifyType vmuseNotify, object sender, ArrayList args)
        {
            string notifyType = vmuseNotify.ToString();
            if (!string.IsNullOrEmpty(currentNotify))
            {
                Log.Yellow("Comm", "通知期间再次发出通知不安全,notifyType:" + notifyType);
                if (currentNotify.Equals(notifyType))
                {
                    Log.Red("Comm", "通知期间禁止重复广播,notifyType:" + notifyType);
                    return;
                }
            }
            currentNotify = notifyType;
            if (allListeners.ContainsKey(notifyType))
            {
                List<NotifyListener> listeners = allListeners[notifyType];
                foreach (var listener in listeners)
                {
                    if (listener.callbackArgs != null)
                    {
                        Log.Blue("Comm", "收到通知,notifyType:" + notifyType);
                        listener.callbackArgs?.Invoke(sender, args);
                        if (listener.autoKill)
                        {
                            autoRemoveList.Add(listener);
                        }
                    }
                }
            }
            else
            {
                Log.Red("Comm", "没有收听者,notifyType:" + notifyType);
            }
            currentNotify = null;
            AutoRemove();
        }
        public override string ToString()
        {
            string str = null;
            foreach (var types_listeners in allListeners)
            {
                string type = types_listeners.Key;
                str = type + ":";
                foreach (var listener in types_listeners.Value)
                {
                    string name = (listener.callback != null) ? listener.callback.Method.DeclaringType.Name + ":" + listener.callback.Method.ToString() :
                    (listener.callbackFloat != null) ? listener.callbackFloat.Method.DeclaringType.Name + ":" + listener.callbackFloat.Method.ToString() :
                    (listener.callbackObj != null) ? listener.callbackObj.Method.DeclaringType.Name + ":" + listener.callbackObj.Method.ToString() :
                    listener.callbackArgs.Method.DeclaringType.Name + ":" + listener.callbackArgs.Method.ToString();
                    str = str + "\n\t" + name;
                }
                str = str + "\n";
            }
            return str;
        }

        internal void Register(NotifyType test1, Manager manager, object test2)
        {
            throw new NotImplementedException();
        }
    }
}
