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

        List<LoadJob> jobs;
        int totalSize;

		private struct LoadJob {
            public int weight;
            public System.Func<float> statusCB;
        }

        public void AddJob(int weight, System.Func<float> statusCB) {
            jobs.Add(new LoadJob{weight = weight, statusCB = statusCB});
			totalSize += weight;
        }

        void Awake() {
            m_statusText.text = "Loading";
            m_loadingBar.fillAmount = 0;
        }
	}
}
