using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//TODO: Filter messages based on instances flagged with DebugFeedAttribute
public class DebugFeed : EditorWindow{

    static DebugFeed feed;

    class Message
    {
        public string name;
        public string formatMessage;
        public System.Func<object>[] paramGetters;
        public Object target;

        public MessageType type {
            get {
                if (target == null)
                {
                    return MessageType.Error;
                }
                else
                {
                    return MessageType.None;
                }
            }
        }

        string _message;
        public string message { 
            get{
                switch (type)
                {
                    case MessageType.Error: return "Object reference missing!\n"+_message;
                    default: return _message;
                }
            }
            private set
            {
                _message = value;
            }
        }
        public void Update()
        {
            string[] args = new string[paramGetters.Length];
            for (int i = 0; i < paramGetters.Length;i++ )
            {
                args[i] = paramGetters[i].Invoke().ToString();
            }
            message = string.Format(formatMessage,args);
        }
    }

    List<Message> messages = new List<Message>();

    public static void AddMessage(Object target, string format, params System.Func<object>[] args)
    {
        if (feed != null)
        {
            var msg = new Message();
            msg.target = target;
            msg.name = target.name;
            msg.formatMessage = format;
            msg.paramGetters = args;
            feed.messages.Add(msg);
        }
    }

    public static void RemoveMessage(string name)
    {
        Message tr = null;
        for (int i = 0; i < feed.messages.Count; i++)
        {
            if (feed.messages[i].name == name)
            {
                tr = feed.messages[i];
                break;
            }
        }
        feed.messages.Remove(tr);
    }

    [MenuItem("Window/Debug Feed")]
    public static void Init()
    {
        feed = (DebugFeed)EditorWindow.GetWindow(typeof(DebugFeed));
        feed.Show();
    }

    void OnEnable()
    {
        feed = this;
        GetMessages();
        EditorApplication.playmodeStateChanged += GetMessages;
    }

    void OnDestroy()
    {
        EditorApplication.playmodeStateChanged -= GetMessages;
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Messages", "Count: "+messages.Count);
        for (int i = 0; i < messages.Count; i++)
        {
            if (!string.IsNullOrEmpty(messages[i].name))
            {
                EditorGUILayout.LabelField(messages[i].name);
            }
            EditorGUILayout.HelpBox(messages[i].message, messages[i].type, true);
        }
    }

    void OnInspectorUpdate()
    {
        for (int i = 0; i < messages.Count; i++)
        {
            messages[i].Update();
            //Debug.Log(messages[i].paramGetters[0]() + " : " + (messages[i].target as Shrines.PhysicsBody).position);
        }
        Repaint();
    }

    void GetMessages()
    {

        messages.Clear();

        string format;
        System.Func<object>[] args;

        var objs = FindObjectsOfType<MonoBehaviour>();
        foreach (var o in objs.Where(o=>o is IDebuggable))
        {
            var d = o as IDebuggable;
            if (d != null)
            {
                d.GetDebugMessageArgs(out format, out args);
                AddMessage(o, format, args);
                messages.Last().target = o;
            }
        }
    }
}
