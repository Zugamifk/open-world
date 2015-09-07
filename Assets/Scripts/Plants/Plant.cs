using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class Plant : MonoBehaviour, IMeshGenerator {
    public IPLSystem structure;
    public PTurtle turtle;
    
    public Dictionary<string, IMeshGenerator> componentGenerators = new Dictionary<string, IMeshGenerator>();

    public abstract Mesh Generate();
    public abstract string Name{ get; }
}
