using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour {

    public Material spriteMaterial;

    public static ResourceManager Instance;

    void Awake()
    {
        this.SetInstanceOrKill(ref Instance);
    }
}
