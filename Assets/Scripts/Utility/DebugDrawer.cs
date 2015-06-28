using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class DebugDrawer : MonoBehaviour {

    public delegate void DebugDrawCall();
    public static DebugDrawCall DrawCalls;

    public void OnPostRender() {
        if(DrawCalls!=null)
            DrawCalls();
    }

    public static DebugDrawer instance;
    // Use this for initialization
    void Start () {
        this.SetInstanceOrKill(ref instance, "Duplicate DebugDrawer object!");
    }

}
