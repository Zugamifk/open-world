using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Extensions.Managers;

namespace Animations {
	public class LoadingBar : MonoBehaviour {

        [SerializeField]
        Text m_statusText;
        [SerializeField]
        GameObject m_graphicsRoot;
        [SerializeField]
        Image m_loadingBar;
        [SerializeField]
        Image m_blackout;
		[Header("Debug")]
        [SerializeField]
        bool debug;
        [SerializeField]
        float fadeDuration = 1;
        [SerializeField]
        float minWait = 1;

        Queue<LoadJob> jobs;
        LoadJob currentJob;
        int totalSize;
        int sizeCompleted;

        float enableTime;
		bool loading;
		IEnumerable<Graphic> renderers;

        System.Diagnostics.Stopwatch debugTimer;

        static LoadingBar instance;

		private class LoadJob {
            public string status;
            public int weight;
            public System.Func<float> statusCB;
        }

        public delegate void PostLoadingAction();
        public static PostLoadingAction OnPostLoad;

        bool animating;


        public static void AddJob(string status, int weight, System.Func<float> statusCB) {
            statusCB = statusCB ?? (() => 0);
            instance._AddJob(status, weight, statusCB);
        }

        public static void Enable()
        {
            instance.gameObject.SetActive(true);
        }

		void _AddJob(string status, int weight, System.Func<float> statusCB) {
            statusCB = statusCB ?? (() => 0);
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
        }

		void OnFinishedLoad() {
            loading = false;
            m_statusText.text = "Done!";
            StartCoroutine(FinishedAnimation());
			if(debug) {
                Debug.Log("Loading time: " + debugTimer.Elapsed.TotalMilliseconds+" ms");
            }
        }

		IEnumerator FinishedAnimation() {

            animating = true;
            m_blackout.CrossFadeAlpha(1, fadeDuration, true);

            yield return new WaitForSeconds(fadeDuration);

            renderers.ForEach(g => g.enabled = false);

			if(OnPostLoad!=null) OnPostLoad();

            m_blackout.CrossFadeAlpha(0, fadeDuration, true);

            yield return new WaitForSeconds(fadeDuration);
            
            animating = false;
            m_blackout.enabled = false;
            gameObject.SetActive(false);

        }

        IEnumerator TransitionIn()
        {

            animating = true;
            m_blackout.enabled = true;
            m_blackout.CrossFadeAlpha(1, fadeDuration, true);

            yield return new WaitForSeconds(fadeDuration);

            renderers.ForEach(g => g.enabled = true);

            m_blackout.CrossFadeAlpha(0, fadeDuration, true);

            yield return new WaitForSeconds(fadeDuration);
            animating = false;

        }

        void Awake() {
            if (this.SetInstanceOrKill(ref instance))
            {
                jobs = new Queue<LoadJob>();
                renderers = m_graphicsRoot.GetAll<Graphic>();
            }
        }

        void OnEnable()
        {
            m_statusText.text = "Loading";
            m_loadingBar.fillAmount = 0;
            loading = true;
            enableTime = Time.unscaledTime;
            renderers.ForEach(g => g.enabled = false);
            m_blackout.canvasRenderer.SetAlpha(0);
            StartCoroutine(TransitionIn());
        }

		void Start() {
			if(debug) {
				debugTimer = new System.Diagnostics.Stopwatch();
                debugTimer.Start();
            }
		}

		void Update() {
			if(!loading) return;
            if(currentJob == null) {
                if (jobs.Count == 0)
                {
                    if (Time.unscaledTime - enableTime > minWait && !animating)
                    {
                        OnFinishedLoad();
                    }
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
