using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Albedo.Graphics {
	public class BlackoutFade : MonoBehaviour {

        [SerializeField]
        Image m_blackoutImage;

		static BlackoutFade instance;
        AnimatedValue<float> animatedAlpha;

        public static void FadeIn() {
            instance.animatedAlpha.Value = 1;
        }

		public static void FadeOut() {
            instance.animatedAlpha.Value = 0;
        }

		public static Coroutine WaitForFade() {
            return instance.animatedAlpha.WaitForAnimation();
        }

        void Awake() {
            this.SetInstanceOrKill(ref instance);
            animatedAlpha = new AnimatedValue<float>(0, Mathf.SmoothStep, 0.5f);
			animatedAlpha.Animator = this;
            animatedAlpha.SetCallback = t => m_blackoutImage.color = m_blackoutImage.color.SetAlpha(t);
        }
	}
}
