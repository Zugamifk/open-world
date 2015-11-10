using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//TODO: track how many tasks are running and limit them
namespace Albedo.Crunching {
	public class CrunchManager : MonoBehaviour {

        public delegate IEnumerator CrunchingRoutine();

        private enum ConditionStatus {
			NotReady,
			Ready,
			Error
		}

		private struct CrunchTask {
            public string[] conditions;
            public CrunchingRoutine task;
			public CrunchTask(CrunchingRoutine task, params string[] conditions) {
				this.task = task;
				this.conditions = conditions;
			}
        }

		private Dictionary<string, ConditionStatus> conditionStatuses;
        private Queue<CrunchTask> tasks;
        private static CrunchManager instance;

        void Awake() {
            if(this.SetInstanceOrKill(ref instance)) {
                conditionStatuses = new Dictionary<string, ConditionStatus>();
                tasks = new Queue<CrunchTask>();

				// Set up some conditions
				RegisterCondition(Constants.PlayerControlInitialized);
            }
        }

		void Update() {
            var maxCheck = tasks.Count;
            for (int i = 0; i < maxCheck;i++) {
                var task = tasks.Dequeue();
				if(task.conditions.All(ConditionReady)) {
                    StartCoroutine(task.task());
                } else {
					tasks.Enqueue(task);
				}
			}
        }

		bool ConditionReady(string condition) {
            ConditionStatus status;
			if(conditionStatuses.TryGetValue(condition, out status)) {
                return status == ConditionStatus.Ready;
            } else {
				Debug.LogErrorFormat("Condition \"{0}\" is not being tracked!", condition);
                return false;
            }
		}

		public static void RegisterCondition(string conditionName) {
            ConditionStatus status;
			if(instance.conditionStatuses.TryGetValue(conditionName, out status)) {
                Debug.LogErrorFormat("Condition \"{0}\" is already being tracked!", conditionName);
                return;
            }
			instance.conditionStatuses.Add(conditionName, ConditionStatus.NotReady);
        }

		public static void UpdateStatus(string conditionName, bool success, string error = null) {
            ConditionStatus status;
			if(instance.conditionStatuses.TryGetValue(conditionName, out status)) {
				if(success) {
                    instance.conditionStatuses[conditionName] = ConditionStatus.Ready;
                } else {
                    instance.conditionStatuses[conditionName] = ConditionStatus.Error;
					if(!string.IsNullOrEmpty(error)) {
						Debug.LogError(error);
					}
                }
			} else {
                Debug.LogErrorFormat("Condition \"{0}\" is not being tracked!", conditionName);
            }
        }

		public static void AddRoutine(CrunchingRoutine routine, params string[] conditions) {
			instance.tasks.Enqueue(new CrunchTask(routine, conditions));
		}
	}
}
