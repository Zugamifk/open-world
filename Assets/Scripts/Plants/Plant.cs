using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class Plant : MonoBehaviour {
    public IPLSystem structure;
    public Dictionary<string, IMeshGenerator> componentGenerators = new Dictionary<string, IMeshGenerator>();
}
