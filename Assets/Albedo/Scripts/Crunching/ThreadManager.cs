using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Albedo.Crunching {
	public class ThreadManager : MonoBehaviour {

        [SerializeField]
        int threadCount;

		private struct ThreadTask {
			public Action task;
            public Action onComplete;
        }

        readonly object _lock = new object();
        Thread[] threads;
		Queue<ThreadTask> tasks;
        Queue<Action> completeActions;

        static ThreadManager instance;

		public static void QueueTask(Action task) {
            instance._QueueTask(task, null);
        }

		public static void QueueTask(Action task, Action onComplete) {
            instance._QueueTask(task, onComplete);
        }

		void _QueueTask(Action task, Action onComplete) {
			lock(_lock) {
                tasks.Enqueue(new ThreadTask{task = task, onComplete = onComplete});
                Monitor.Pulse(_lock);
            }
		}

		void ThreadWorker() {
			ThreadTask task;
            while (true)
            {
                lock (_lock)
                {
                    while (tasks.Count == 0)
                    {
                        Monitor.Wait(_lock);
                    }
                    task = tasks.Dequeue();
                }
                task.task();
				if(task.onComplete!=null)
                	completeActions.Enqueue(task.onComplete);
            }
        }

		void Awake() {
            if (this.SetInstanceOrKill(ref instance))
            {
                threads = new Thread[threadCount];
                tasks = new Queue<ThreadTask>();
                completeActions = new Queue<Action>();

                for (int i = 0; i < threadCount; i++)
                {
                    threads[i] = new Thread(ThreadWorker);
                    threads[i].Start();
                }
            }
        }

		void Update() {
			while(completeActions.Count>0) {
                completeActions.Dequeue()();
            }
		}

		void OnApplicationQuit() {
			for (int i = 0; i < threadCount; i++)
			{
				threads[i].Abort();
			}
		}
	}
}
