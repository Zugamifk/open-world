using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Albedo.Graphics {
	public class LoadingBar : MonoBehaviour {

        [SerializeField]
        Text m_statusText;
        [SerializeField]
        Image m_loadingBar;

        Queue<LoadJob> jobs;
        LoadJob currentJob;
        int totalSize;
        int sizeCompleted;

		bool loading;
		IEnumerable<Graphic> renderers;

		static LoadingBar instance;

		private class LoadJob {
            public string status;
            public int weight;
            public System.Func<float> statusCB;
        }

        public delegate void PostLoadingAction();
        public static PostLoadingAction OnPostLoad;

        public static void AddJob(string status, int weight, System.Func<float> statusCB) {
            instance._AddJob(status, weight, statusCB);
        }

		void _AddJob(string status, int weight, System.Func<float> statusCB) {
            jobs.Enqueue(new LoadJob{status = status, weight = weight, statusCB = statusCB});
			totalSize += weight;
        }

		void UpdateStatus() {
            var jobstatus = currentJob.statusCB();
            m_loadingBar.fillAmount = (float)(sizeCompleted + (int)(jobstatus * (float)currentJob.weight)) / (float)totalSize;
            if(jobstatus>=1) {
				sizeCompleted += currentJob.weight;
                currentJob = null;
            }
            renderers = gameObject.GetAll<Graphic>();
        }

		void OnFinishedLoad() {
            loading = false;
            m_statusText.text = "Done!";
            StartCoroutine(FinishedAnimation());
        }

		IEnumerator FinishedAnimation() {

            BlackoutFade.FadeIn();

			yield return BlackoutFade.WaitForFade();

            renderers.ForEach(g => g.enabled = false);

			if(OnPostLoad!=null) OnPostLoad();

            BlackoutFade.FadeOut();

        }

        void Awake() {
            if (this.SetInstanceOrKill(ref instance))
            {
                jobs = new Queue<LoadJob>();
                m_statusText.text = "Loading";
                m_loadingBar.fillAmount = 0;
                loading = true;
            }
        }

		void Update() {
			if(!loading) return;
            if(currentJob == null) {
                if (jobs.Count == 0)
                {
                    OnFinishedLoad();
                    return;
                } else
                {
                    currentJob = jobs.Dequeue();
                    m_statusText.text = currentJob.status;
                }
            }
            UpdateStatus();
        }
	}
}
