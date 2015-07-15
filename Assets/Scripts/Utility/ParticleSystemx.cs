using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemx : MonoBehaviour {

	public delegate ParticleSystem.Particle ParticleFunction(ParticleSystem.Particle p);
	public delegate void ParticleSystemFunction(ParticleSystem ps);
	private List<ParticleFunction> particleFunctions;
	private List<ParticleSystemFunction> particleSystemFunctions;
	private ParticleSystem system;
	private ParticleSystem.Particle[] particles;
	private bool initialized = false;

	public void InitIfNecessary() {
		if (!initialized) {
			particleFunctions = new List<ParticleFunction>();
			particleSystemFunctions = new List<ParticleSystemFunction>();
			system = GetComponent<ParticleSystem>();
			particles = new ParticleSystem.Particle[system.maxParticles];
			initialized = true;
		}
	}

	// public void Spiral(float speed, Vector3 axis) {
	// 	InitIfNecessary();
	// 	particleFunctions.Add( p => {
	// 		p.position += Vector3.Cross(axis, p.position.normalized)*Time.deltaTime*Mathf.Pow((10-p.position.xy().magnitude)/10, 2);
	// 			return p;
	// 		});
	// }

	public void AddFunction(ParticleFunction function) {
		particleFunctions.Add(function);
	}

	public void AddFunction(ParticleSystemFunction function) {
		particleSystemFunctions.Add(function);
	}

	public void Clear() {
		particleFunctions.Clear();
		particles = new ParticleSystem.Particle[system.maxParticles];
	}

	// Use this for initialization
	void Awake () {
		InitIfNecessary();
	}

	// Update is called once per frame
	void Update () {
		if(GetComponent<ParticleSystem>().IsAlive()) {
			foreach(var f in particleSystemFunctions) {
				f(system);
			}

			int num = system.GetParticles(particles);
			for(int i=0;i<num;i++) {
				foreach(var f in particleFunctions) {
					particles[i] = f(particles[i]);
				}
			}
			system.SetParticles(particles, num);
		}
	}
}

public static class ParticleSystemExtensions {
	public static ParticleSystemx Ext(this ParticleSystem p) {
		return p.GetComponent<ParticleSystemx>();
	}
}

//
//
//#if UNITY_EDITOR
//[CustomEditor(typeof(ParticleSystemx))]
//public class ParticleSystemxEditor : Editor {
//	bool hasSpiralFunc = false;
//	public override void OnInspectorGUI() {
//		var psx = target as ParticleSystemx;
//		var spiral = GUILayout.Toggle(hasSpiralFunc, "Spiral");
//		if(spiral!=hasSpiralFunc) {
//			hasSpiralFunc = spiral;
//			if(hasSpiralFunc) {
//
//			}
//		}
//	}
//}
//#endif
