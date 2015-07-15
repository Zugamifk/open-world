using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GenericAnimators : MonoBehaviour {

	// Should this instance set itself to be the static instance?
	public bool IsStatic = false;

	/** A function taking an argument ranging [0,1], to be used for animations */
	public delegate void ParameterizedFunction(float t);

	/** A function taking a vector argument, to be used for animations */
	public delegate void VectorFunction(Vector3 v);

	/** A conditional function */
	public delegate bool TestFunction();

	public static TestFunction Always = () => true;
	public static TestFunction Never = () => false;
	public static TestFunction Randomly = () => UnityEngine.Random.value > 0.5f;
	public static TestFunction NotInteracting = () => !InputManager.AnyInput;
	public static TestFunction Interacting = () => InputManager.AnyInput;

	public static TestFunction Not(TestFunction test) {
		return () => !test();
	}

	public static TestFunction And(TestFunction a, TestFunction b) {
		return () => a() && b();
	}

	public static TestFunction Or(TestFunction a, TestFunction b) {
		return () => a() || b();
	}

	/** A function of time that generates a vector */
	public delegate Vector3 VectorGenerator(float t);

	/** A single encapsulated looping sound */
	public class Sound {
		public GameObject source;
        private GenericAnimators animator;
        private bool looping;
		private AudioClipList clipList;
		private TestFunction test;
		private float delay = 0;
		public float Volume {
			get { return source.audio.volume; }
			set { source.audio.volume = value; }
		}
		public bool IsPlaying {
			get { return source.audio.isPlaying; }
		}
		private Sound() {
			source = new GameObject();
            source.name = "GenericAnimators Sound";
            source.AddComponent<AudioSource>();
            animator = source.AddComponent<GenericAnimators>();
        }
		public Sound(AudioClip sound)
		: this()
		{
			source.audio.clip = sound;
		}
		public Sound(AudioClip sound, Transform parent)
		: this(sound)
		{
			source.transform.parent = parent;
		}
		public Sound(AudioClipList soundList)
		: this(){
			clipList = soundList;
			source.audio.clip = clipList.RandomClip;
		}
		public Sound(AudioClipList soundList, Transform parent)
		: this(soundList){
			source.transform.parent = parent;
		}
		public void SetTest(TestFunction test) {
			this.test = test;
		}
		public void SetDelay(float d) {
			delay = d;
		}
		public void Start(float fade = -1) {
			if(source == null) return;
			if(test == null) test = Always;
			source.audio.Play();
			if (fade > 0) {
				animator.StartCoroutine(FadeIn(source.audio, fade, Mathf.SmoothStep, source.audio.volume));
			}
			if (looping)
				animator.StartCoroutine(_Loop ());
		}
		public void Loop(float fade = -1) {
            looping = true;
            Start(fade);
        }
		public void Stop(float fade = -1) {
			if(source == null) return;
			animator.StartCoroutine(GenericAnimators.FadeOut (source.audio, 0.5f, Mathf.SmoothStep));
			looping = false;
		}
		public void Destroy(float time = 0) {
			if(source == null) return;
            DestroyObject(source, time);
        }
		private IEnumerator _Loop() {
            while(looping) {
				if (source == null || source.audio == null){
					yield break;
				}
				if(!source.audio.isPlaying) {
					if (clipList!=null) {
						source.audio.clip = clipList.RandomClip;
					}
					source.audio.Stop();
					source.audio.Play();
                }
				yield return 1;
				yield return new WaitForSeconds(delay);
				while(!test() && looping) {
					yield return 1;
				}
            }

			yield return animator.StartCoroutine(GenericAnimators.FadeOut (source.audio, 0.5f, Mathf.SmoothStep));
		}
	}

	public static IEnumerator FadeIn(Renderer renderer, float time, Mathfx.ErpFunc interpolationFunction, float alphaTo = -1 ) {
		bool isSprite = renderer is SpriteRenderer;
		Color color = isSprite?
			(renderer as SpriteRenderer).color :
				renderer.material.color;
		var alpha = alphaTo>0?alphaTo:color.a;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			var c = color;
			c.a=interpolationFunction(0,alpha,t);
			if (isSprite) (renderer as SpriteRenderer).color = c;
			else renderer.material.color = c;
			yield return 1;
		}
		color.a = interpolationFunction(0,alpha,1);
		if (isSprite) (renderer as SpriteRenderer).color = color;
		else renderer.material.color = color;
	}

	// Fade out a GUI graphic
	public static IEnumerator FadeIn(Graphic graphic, float time, Mathfx.ErpFunc interpolationFunction, float alphaTo = -1 ) {
		Color color = graphic.color;
		var alpha = alphaTo>0?alphaTo:color.a;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			var c = color;
			c.a=interpolationFunction(0,alpha,t);
			graphic.color = c;
			yield return 1;
		}
		color.a = interpolationFunction(0,alpha,1);
	}

	public static IEnumerator FadeOut(Renderer renderer, float time, Mathfx.ErpFunc interpolationFunction, bool hideAndReset = false) {
		Color color = Color.white;
		Action<Color> setColor = c => {};
		if (renderer is SpriteRenderer) {
			color = (renderer as SpriteRenderer).color;
			setColor = c => (renderer as SpriteRenderer).color = c;
		} else
		{
			color = renderer.material.color;
			setColor = c=>renderer.material.color=c;
		}
		var oldCol = color;
		var alpha = color.a;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			var c = color;
			c.a=interpolationFunction(alpha,0,t);
			setColor(c);
			yield return 1;
		}
		color.a = interpolationFunction(0,1,0);

		if (hideAndReset) {
			setColor(oldCol);
			renderer.enabled = false;
		} else {
			setColor(color);
		}
	}

	// Fade out a GUI graphic
	public static IEnumerator FadeOut(Graphic graphic, float time, Mathfx.ErpFunc interpolationFunction, bool hideAndReset = false) {
		Color color = graphic.color;
		var oldCol = color;
		var alpha = color.a;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			var c = color;
			c.a=interpolationFunction(alpha,0,t);
			graphic.color = c;
			yield return 1;
		}
		color.a = interpolationFunction(0,1,0);

		if (hideAndReset) {
			graphic.color = oldCol;
			graphic.enabled = false;
		} else {
			graphic.color = color;
		}
	}

	/** Interpolates renderer From into renderer To over Time seconds. If Delay is negative, From fades out first, otherwise To fades in first.
	 If enableObjects is true, renderers faded in are enabled first and renderers faded out are disabled after*/
	public static IEnumerator CrossFade(Renderer from, Renderer to, float time, float delay,
	                                    Mathfx.ErpFunc interpolationFuncitonA,
	                                    Mathfx.ErpFunc interpolationFuncitonB,
	                                    bool enableObjects = false, bool isSprite = false) {
		if (delay < 0) {
			Instance.StartCoroutine(FadeOut(from, time, interpolationFuncitonA));
			yield return new WaitForSeconds(-delay);
			if (enableObjects) {
				from.enabled = false;
				to.enabled = true;
			}
			yield return Instance.StartCoroutine(FadeIn(to, time, interpolationFuncitonB));
		} else {
			if (enableObjects) to.enabled = true;
			if (Instance == null){
				yield break;
			}
			Instance.StartCoroutine(FadeIn(to, time, interpolationFuncitonB));
			yield return new WaitForSeconds(delay);
			yield return Instance.StartCoroutine(FadeOut(from, time, interpolationFuncitonA));
			if (enableObjects) from.enabled = false;
		}
	}

	/** Oscillate to and from direction*amplitude at frequency */
	public static IEnumerator Oscillate(Transform transform, Vector3 direction, float amplitude, float frequency) {
		var pos = transform.localPosition;
		while(transform.gameObject.activeInHierarchy) {
			for(float t=0;t<1;t+=Time.deltaTime/frequency) {
				transform.localPosition = pos + direction*Mathfx.CosNU(t)*amplitude;
				yield return 1;
			}
		}
	}

	/** Oscillate between two points with a given interpolation function */
	 public static IEnumerator Oscillate(Transform transform, Vector3 destinationOffset, Mathfx.ErpFunc interpolation, float time) {
		var pos = transform.localPosition;
		var to = pos + destinationOffset;
		while(transform.gameObject.activeInHierarchy) {
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				transform.localPosition = Mathfx.Interpolate(pos, to, t, interpolation);
				yield return 1;
			}
		}
	}

	/** Rotate around an axis at a set rate */
	public static IEnumerator Rotate(Transform transform, Vector3 axis, float degreesPerSecond, Space relativeTo = Space.World) {
		var ang = transform.rotation;
		while(transform.gameObject.activeInHierarchy) {
			for(float t=0;t<1;t+=Time.deltaTime) {
				transform.Rotate(axis, Time.deltaTime*degreesPerSecond,relativeTo);
				yield return 1;
			}
		}
		transform.rotation = ang;
	}

	/** Oscillate between two points with a given interpolation function */
	public static IEnumerator Rotate(Transform transform, Vector3 axis, Mathfx.ErpFunc angles, float time) {
		var ang = transform.rotation;
		while(transform.gameObject.activeInHierarchy) {
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				transform.Rotate(axis, Time.deltaTime*angles(0,1,t)*360);
				yield return 1;
			}
		}
		transform.rotation = ang;
	}

	/** Interpolates between two transforms. Copies transform data before interpolating, so OBJ's transform as a start or end point is safe. */
	public static IEnumerator InterpolateTransforms(Transform obj, Transform from, Transform to, float time, Mathfx.ErpFunc interpolation, bool inWorldSpace=false) {
		if (inWorldSpace) {
			var fpos = from.position;
			var fscl = from.lossyScale.InverseScale(obj.parent.lossyScale);
			var frot = from.transform.rotation;
			var tpos = to.position;
			var tscl = to.lossyScale.InverseScale(obj.parent.lossyScale);
  			var trot = to.rotation;

//			var offset = Vector3.zero;
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				obj.position = Mathfx.Interpolate(fpos, tpos, t, interpolation);
				obj.localScale = Mathfx.Interpolate(fscl, tscl, t, interpolation);
				obj.rotation = Quaternion.Slerp(frot, trot, interpolation(0,1,t));
				yield return 1;
			}
			obj.position = tpos;
			obj.localScale = tscl;
			obj.rotation = trot;
		} else {
			var pos = from.localPosition;
			var scl = from.localScale;
			var rot = from.localRotation;
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				obj.localPosition = Mathfx.Interpolate(pos, to.localPosition, t, interpolation);
				obj.localScale = Mathfx.Interpolate(scl, to.localScale, t, interpolation);
				obj.localRotation = Quaternion.Slerp(rot, to.localRotation, interpolation(0,1,t));
				yield return 1;
			}
			obj.localPosition = to.localPosition;
			obj.localScale = to.localScale;
			obj.localRotation = to.localRotation;
		}
		yield break;
	}

	/** Move a transform to a position */
	public static IEnumerator MoveTo(Transform obj, Vector3 destination, float time, Mathfx.ErpFunc interpolation, bool inWorldSpace=false) {
		if (inWorldSpace) {

			var pos = obj.position;
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				obj.position = Mathfx.Interpolate(pos, destination, t, interpolation);
				yield return 1;
			}
			obj.position = destination;
		} else {
			var pos = obj.localPosition;
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				obj.localPosition = Mathfx.Interpolate(pos, destination, t, interpolation);
				yield return 1;
			}
			obj.localPosition = destination;
		}
		yield break;
	}

	/** Translate a transform by a vector */
	public static IEnumerator Translate(Transform obj, Vector3 translation, float time, Mathfx.ErpFunc interpolation, bool inWorldSpace=false) {

		if (inWorldSpace) {
			var pos = obj.position;
			for(float t=0;t<1;t+=Time.deltaTime/time) {
                obj.position = Mathfx.Interpolate(pos, pos+translation, t, interpolation);
				yield return 1;
			}
		} else {
			var pos = obj.localPosition;
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				Debug.DrawRay(pos, translation,Colorx.orange, 5);
				obj.localPosition = Mathfx.Interpolate(pos, pos+translation, t, interpolation);
				yield return 1;
			}
		}
		yield break;
	}

	/**Delay an animation function*/
	public static IEnumerator DelayedAnimation(GameObject go, float delay, System.Action<GameObject> anim) {
		yield return new WaitForSeconds(delay);
		anim(go);
	}
	public static IEnumerator DelayedAnimation(float delay, System.Action anim) {
		yield return new WaitForSeconds(delay);
		anim();
	}

	/**Play an animation when a condition is true*/
	public static IEnumerator AnimateWhen(System.Action animation, System.Func<bool> condition) {
		while (!condition()) {
			yield return 1;
		}
		animation();
	}

	/**Play an animation as long as a condition is true*/
	public static IEnumerator AnimateWhile(ParameterizedFunction animation, TestFunction condition) {
		float t = Time.time;
		while (condition()) {
			animation(Time.time-t);
			yield return 1;
		}
	}

	/**Play an animation every INTERVAL seconds */
	public static IEnumerator AnimateEvery(ParameterizedFunction animation, float interval) {
		float t = Time.time;
		while(true) {
			animation(Time.time-t);
			yield return new WaitForSeconds(interval);
		}
	}

	/**Play a frame-by-frame sprite animation*/
	public static IEnumerator SpriteLoop(SpriteRenderer sprite, Sprite[] frames, float frameTime) {
		while(true) {
			foreach(var f in frames) {
				sprite.sprite = f;
				yield return new WaitForSeconds(frameTime);
			}
			yield return 1;
		}
	}

	/**Lerp in a textureblend shaded renderer*/
	public static IEnumerator LerpInTextureBlend(MeshRenderer mesh, Mathfx.ErpFunc interpolation, float time) {
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			mesh.material.SetFloat("_Lerp", interpolation(0, 1, t));
			yield return 1;
		}
		mesh.material.SetFloat("_Lerp", interpolation(0,1,1));
	}

	/** Animate a shader float */
	public static IEnumerator AnimateShaderFloat(Material material, string floatName, Mathfx.ErpFunc interpolation, float time, float lo = 0, float hi = 1) {
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			material.SetFloat(floatName, interpolation(lo, hi, t));
			yield return 1;
		}
		material.SetFloat(floatName, hi);
	}

	/** Scroll UV coordinates */
	public static IEnumerator UVScroll(Material material, Mathfx.ErpFunc interpolationX, Mathfx.ErpFunc interpolationY, float time, Vector2 lo, Vector2 hi, bool oneShot = false) {
		Vector2 uv = lo;
		while(!oneShot) {
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				uv.x = interpolationX(lo.x, hi.x, t);
				uv.y = interpolationY(lo.y, hi.y, t);
				material.mainTextureOffset = uv;
				yield return 1;
			}
		}
		material.mainTextureOffset = hi;
	}

	public static IEnumerator UVScroll(Material material, string UVstring, Mathfx.ErpFunc interpolationX, Mathfx.ErpFunc interpolationY, float time, Vector2 lo, Vector2 hi, bool oneShot = false) {
		Vector2 uv = lo;
		while(!oneShot) {
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				uv.x = interpolationX(lo.x, hi.x, t);
				uv.y = interpolationY(lo.y, hi.y, t);
				material.SetTextureOffset(UVstring, uv);
				yield return 1;
			}
		}
		material.SetTextureOffset(UVstring, hi);
	}

	/**Play a particle effect for a set amount of time*/
	public static IEnumerator TimedEmit(ParticleSystem ps, float time) {
		ps.Play ();
		yield return new WaitForSeconds(time);
		ps.Stop ();
	}

	/**Play a particle effect a fixed number of times*/
	public static IEnumerator CountingEmit(ParticleSystem ps, int num) {
		while(num-->0) {
			ps.Play ();
			yield return new WaitForSeconds(ps.duration);
		}
		ps.Stop ();
	}

	/**Grow a line over another line*/
	public static IEnumerator ReplaceLine(LineRenderer a, LineRenderer b, float time, Mathfx.ErpFunc interpolation, Vector3 startPosition, Vector3 endPosition, bool toggleRenderers = false) {
		if(toggleRenderers) b.enabled=true;
		b.SetPosition(0,startPosition);
		b.SetPosition(1,startPosition);
		for (float t=0;t<1;t+=Time.deltaTime/time) {
			var progress = Mathfx.Interpolate(startPosition,endPosition,t,interpolation);
			a.SetPosition(0, progress);
			b.SetPosition(1, progress);
			yield return 1;
		}
		a.SetPosition(0, endPosition);
		b.SetPosition(1, endPosition);
		if(toggleRenderers) a.enabled=false;
	}

	/** Grow a line. If reverse is true, it will animate with the start position and reverse the interpolation */
	public static IEnumerator GrowLine(LineRenderer line, float time, Mathfx.ErpFunc interpolation, Vector3 startPosition, Vector3 endPosition, bool reverse = false) {
		line.SetPosition(0,startPosition);
		line.SetPosition(1,endPosition);
		int animatedEndIndex = reverse?0:1;

		line.SetPosition(animatedEndIndex,Mathfx.Interpolate(startPosition,endPosition,0,interpolation));

		for (float t=0;t<1;t+=Time.deltaTime/time) {
			var progress = Mathfx.Interpolate(startPosition,endPosition,t,interpolation);
			line.SetPosition(animatedEndIndex, progress);
			yield return 1;
		}
		line.SetPosition(animatedEndIndex,Mathfx.Interpolate(startPosition,endPosition,1,interpolation));
	}

	/** Lerp in a transform's localScale from 0 according to a curve over time*/
	public static IEnumerator ScaleIn(Transform tf, Mathfx.ErpFunc interpolation, float time, bool setAtEnd = true) {
		var scl = tf.localScale;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			tf.localScale = Mathfx.Interpolate(Vector3.zero, scl, t, interpolation);
			yield return 1;
		}
		if (setAtEnd) tf.localScale = scl;
	}

	/** Lerp in a transform's localScale from 0 according to a curve over time*/
	public static IEnumerator ScaleInAtLocation(Transform tf, Vector3 location, Mathfx.ErpFunc interpolation, float time, bool setAtEnd = true) {
		var scl = tf.localScale;
		var pos = tf.position;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			tf.localScale = Mathfx.Interpolate(Vector3.zero, scl, t, interpolation);
			tf.position = Mathfx.Interpolate(location, pos, t, interpolation);
			yield return 1;
		}
		if (setAtEnd) {
			tf.localScale = scl;
			tf.position = pos;
		}
	}

	/** Lerp into a different scale */
	public static IEnumerator ScaleTo(Transform tf, Vector3 scaleTo, Mathfx.ErpFunc interpolation, float time, bool setAtEnd = true) {
		var scl = tf.localScale;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			tf.localScale = Mathfx.Interpolate(scl, scaleTo, t, interpolation);
			yield return 1;
		}
		if(setAtEnd) tf.localScale = Mathfx.Interpolate(scl, scaleTo, 1, interpolation);
	}

	/** Multiply a scale over time */
	public static IEnumerator ScaleTo(Transform tf, float scaleMultiplier, Mathfx.ErpFunc interpolation, float time, bool setAtEnd = true) {
		var scl = tf.localScale;
		var to = scl*scaleMultiplier;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			tf.localScale = Mathfx.Interpolate(scl, to, t, interpolation);
			yield return 1;
		}
		if(setAtEnd) tf.localScale = Mathfx.Interpolate(scl, to, 1, interpolation);
	}

	/** General scale lerp between two scales */
	public static IEnumerator Scale(Transform tf, Vector3 scaleFrom, Vector3 scaleTo, Mathfx.ErpFunc interpolation, float time, bool setAtEnd = true) {
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			tf.localScale = Mathfx.Interpolate(scaleFrom, scaleTo, t, interpolation);
			yield return 1;
		}
		if(setAtEnd) tf.localScale = scaleTo;
	}

    /** Scale along a specific axis */
    public static IEnumerator ScaleAlong(Transform tf, Vector3 axis, Mathfx.ErpFunc interpolation, float time, bool setAtEnd = true)
    {
        var scl = tf.localScale;
        var from = scl-axis * Vector3.Dot(axis, scl);
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			tf.localScale = Mathfx.Interpolate(from, scl, t, interpolation);
			yield return 1;
		}
		if(setAtEnd) tf.localScale = scl;
    }

    /** Shakes an object */
    public static IEnumerator Shake(Transform transform, Mathfx.ErpFunc falloff, float magnitude, float time) {
		var pos = transform.position;
		for (float t=0;t<1;t+=Time.deltaTime/time) {
			transform.position = pos + UnityEngine.Random.onUnitSphere * magnitude * falloff(0,1,t);
			yield return 1;
		}
		transform.position = pos;
	}

	/** Shakes an object via rotation around an axis */
	public static IEnumerator Shake(Transform transform, Mathfx.ErpFunc falloff, float magnitude, Vector3 axis, float time, bool inWorldSpace = true) {
		if(inWorldSpace) {
			var rot = transform.rotation;
			for (float t=0;t<1;t+=Time.deltaTime/time) {
				transform.rotation = Quaternion.AngleAxis(Mathfx.Random11()*magnitude*falloff(0,1,t), axis)*rot;
				yield return 1;
			}
			transform.rotation = rot;
		} else {
			var rot = transform.localRotation;
			for (float t=0;t<1;t+=Time.deltaTime/time) {
				transform.localRotation = Quaternion.AngleAxis(Mathfx.Random11()*magnitude*falloff(0,1,t), axis)*rot;
				yield return 1;
			}
			transform.localRotation = rot;
		}
	}

	/** Spawn and play a particle system */
	public static IEnumerator OneShotParticle(ParticleSystem systemPrefab, Vector3 position) {
		var ps = (ParticleSystem)Instantiate(systemPrefab);
		ps.transform.position = position;
		ps.Play();
		while(ps.isPlaying) {
			yield return 1;
		}
		DestroyObject(ps.gameObject);
		yield break;
	}

	public static IEnumerator OneShotParticle(ParticleSystem systemPrefab, Vector3 position, Quaternion rotation) {
		var ps = (ParticleSystem)Instantiate(systemPrefab);
		ps.transform.position = position;
		ps.transform.rotation = rotation;
		ps.Play();
		while(ps.isPlaying) {
			yield return 1;
		}
		DestroyObject(ps.gameObject);
		yield break;
	}

	public static IEnumerator OneShotParticle(ParticleSystem systemPrefab, Transform parent, bool makeChild = true) {
		var ps = (ParticleSystem)Instantiate(systemPrefab);
		ps.transform.parent = parent;
		ps.transform.localPosition = Vector3.zero;
		ps.transform.localRotation = Quaternion.identity;
		ps.transform.localScale = Vector3.one;
		if (!makeChild) ps.transform.parent = null;
		ps.Play();
		while(ps.isPlaying) {
			yield return 1;
		}
		DestroyObject(ps.gameObject);
		yield break;
	}

	/** Rotate a transform along a curve */
	public static IEnumerator RotateWith(Transform transform, Vector3 axis, Mathfx.ErpFunc curve, float time, float magnitude = 1) {
		var rot = transform.localRotation;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			transform.localRotation = rot*Quaternion.AngleAxis(curve(0,1,t)*magnitude, axis);
			yield return 1;
		}
		transform.localRotation = rot*Quaternion.AngleAxis(curve(0,1,1)*magnitude, axis);
		yield return 0;
	}

	/** Move an abject according to a force-inducing function */
	public static IEnumerator Push(Transform transform, Vector3 initialVelocity, VectorGenerator vg, float time, Space space = Space.World) {
		if(space == Space.World) {
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				initialVelocity += vg(t)*Time.deltaTime;
				transform.position += initialVelocity * Time.deltaTime;
				yield return 1;
			}
		} else {
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				initialVelocity += vg(t)*Time.deltaTime;
				transform.localPosition += initialVelocity * Time.deltaTime;
				yield return 1;
			}
		}
	}

	/** Lerp a camera between projections */
	public static IEnumerator InterpolateProjections(Camera camera, Mathfx.ErpFunc curve, float time, bool changeProjectionModes = false) {
		var isOrtho = camera.orthographic;
//		if (changeProjectionModes) {
//			camera.orthographic = !isOrtho;
//			camera.ResetProjectionMatrix();
//		}
		Matrix4x4 orthoMat, persMat;
		if(isOrtho)	{
			orthoMat = camera.projectionMatrix;
			camera.orthographic = false;
			camera.ResetProjectionMatrix();
			persMat = camera.projectionMatrix;
		} else {
			persMat = camera.projectionMatrix;
			camera.orthographic = true;
			camera.ResetProjectionMatrix();
			orthoMat = camera.projectionMatrix;
		}
		camera.orthographic = isOrtho;

		for(float t=0;t<1;t+=Time.deltaTime/time) {
			camera.projectionMatrix = Mathfx.Interpolate4x4(orthoMat, persMat, t, curve);
			yield return 1;
		}
	//	camera.orthographic = !isOrtho;
	//	camera.ResetProjectionMatrix();
	}

	public static IEnumerator Glow(Renderer renderer, float period, Mathfx.ErpFunc curve) {
		float t0 = Time.time;
		if(renderer is SpriteRenderer) {
			var sprite = renderer as SpriteRenderer;
			while(renderer.enabled) {
				sprite.color = sprite.color.SetAlpha(curve(0,1,((Time.time-t0)/period)%1));
				yield return 1;
			}
		} else {
			while(renderer.enabled) {
				renderer.material.color = renderer.material.color.SetAlpha(curve(0,1,((Time.time-t0)/period)%1));
				yield return 1;
			}
		}
	}

	public static IEnumerator InterpolateCameraViews(Camera camera, Transform toView, float toOrtho, float viewDistance, float time, Mathfx.ErpFunc interpolation) {

		var startPos = camera.transform.position;
		var startOrtho = camera.orthographicSize;
		var startRot = camera.transform.rotation;

		float p = 0;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			p = interpolation(0,1,t);
			camera.transform.position = Vector3.Lerp(
				startPos,
				toView.position - toView.forward * viewDistance,
				p
				);
			camera.orthographicSize = Mathf.Lerp(
				startOrtho,
				toOrtho,
				p
				);

			camera.transform.LookAt(
				toView.position,
				toView.up
				);


			var cameraToViewingTransform = toView.position - camera.transform.position;
			var lookAtViewintTransformRotation = Quaternion.LookRotation(cameraToViewingTransform, toView.up);

			camera.transform.rotation = Quaternion.Slerp(
				startRot,
				lookAtViewintTransformRotation,
				p
				);

			yield return 1;
		}
	}

	/** Fade in an audiosource */
	public static IEnumerator FadeIn(AudioSource audio, float time, Mathfx.ErpFunc interpolation, float volume = 1f, bool fromCurrent = false) {
		var from = fromCurrent?audio.volume:0;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			if (audio != null){
				audio.volume = interpolation(from,volume,t);
			}
			yield return 1;
		}
	}

	/** Fade out an audiosource */
	public static IEnumerator FadeOut(AudioSource audio, float time, Mathfx.ErpFunc interpolation, float volume = 1f) {
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			audio.volume = interpolation(1,0,t)*volume;
			yield return 1;
		}
	}

	/** Play a sound for a fixed amount of time */
	public static IEnumerator PlayForTime(AudioClip sound, float time, Vector3 position, float fadeOutTime = 0.4f) {
		var source = new GameObject();
		source.transform.position = position;
		source.AddComponent<AudioSource>();
		source.audio.clip = sound;
		source.audio.Play();
		for(float t=0;t<time;t+=Time.deltaTime/time) {
			yield return 1;
		}
		yield return Instance.StartCoroutine(FadeOut(source.audio, fadeOutTime, Mathf.SmoothStep, source.audio.volume));
		Destroy(source);
	}

	public static IEnumerator PlayForTime(AudioClip sound, float time, Transform parent) {
		var source = new GameObject();
		source.transform.parent = parent;
		source.AddComponent<AudioSource>();
		source.audio.clip = sound;
		source.audio.Play();
		for(float t=0;t<time;t+=Time.deltaTime/time) {
			yield return 1;
		}
		yield return Instance.StartCoroutine(FadeOut(source.audio, 0.4f, Mathf.SmoothStep, source.audio.volume));
		Destroy(source);
	}

	/** Loops a sound a fixed number of times, witha no ptional condition to trigger plays*/
	public static IEnumerator LoopForCount(AudioClip sound, int num, TestFunction test = null) {
		test = test??Always;

		var clip = new Sound(sound);
		while(num-->0) {
			while(!test() || clip.IsPlaying) {
				yield return 1;
			}
			clip.Start();
			yield return 1;
		}

		yield break;
	}

	/** Change a renderer's colour according to a gradient */
	public static IEnumerator GradeColor(Renderer renderer, Gradient gradient, float time) {
		if(renderer is SpriteRenderer) {
			var sprite = renderer as SpriteRenderer;
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				sprite.color = gradient.Evaluate(t);
				yield return 1;
			}
		} else {
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				renderer.material.color = gradient.Evaluate(t);
				yield return 1;
			}
		}
	}

	/** Lerp a renderer's color between two */
	public static IEnumerator GradeColor(Renderer renderer, Color a, Color b, Mathfx.ErpFunc interpolation, float time) {
		if(renderer is SpriteRenderer) {
			var sprite = renderer as SpriteRenderer;
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				sprite.color = Mathfx.Interpolate(a, b, t, interpolation);
				yield return 1;
			}
		} else {
			for(float t=0;t<1;t+=Time.deltaTime/time) {
				renderer.material.color = Mathfx.Interpolate(a, b, t, interpolation);
				yield return 1;
			}
		}
	}

	/** Lerp a material's color between two */
	public static IEnumerator GradeColor(Material material, Color a, Color b, Mathfx.ErpFunc interpolation, float time) {
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			material.color = Mathfx.Interpolate(a, b, t, interpolation);
			yield return 1;
		}
	}

	/** move a transform through a spline path */
	public static IEnumerator MoveOnPath(Transform tf, Spline spline, float time, Mathfx.ErpFunc interpolation, bool look,
	                                     ParameterizedFunction pFunc = null, VectorFunction velocityFunction = null, bool debug = false) {
		var op = tf.position;
//		var speed = 1/time;
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			spline.Update();

			var p = interpolation(0,1,t);

			op = tf.position;
			tf.position = spline.EvaluateNormalized(p);

			if(look) tf.LookAt(tf.position-op, Vector3.up);
			if(pFunc!=null) pFunc(t);
			if(velocityFunction!=null) {
				var dt = (tf.position-op);
				velocityFunction(dt);
			}
            if (debug)
            {
                spline.DebugDraw(0.1f, true);
				spline.DebugDrawPos(t);
            }

            yield return 1;
		}
	}

	/** Animate some given f(t) over time */
	public static IEnumerator AnimateForTime(ParameterizedFunction f, float time) {
		for(float t=0;t<1;t+=Time.deltaTime/time) {
			f(t);
			yield return 1;
		}
	}

	/** Yield until an event happens */
	public static IEnumerator WaitFor(TestFunction test) {
		while(!test()) {
			yield return 1;
		}
		yield break;
	}

	/** Yield until user inputs something */
	public static IEnumerator WaitForInput() {
		while(!Interacting()) {
			yield return 1;
		}
		yield break;
	}

	public static GenericAnimators Instance;

	void Awake() {
		if (IsStatic) Instance = this;
	}
}
