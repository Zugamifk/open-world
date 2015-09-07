using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class Plant : IMeshGenerator, IWorldObject {
    public IPLSystem structure;

    public Dictionary<string, IMeshGenerator> componentGenerators = new Dictionary<string, IMeshGenerator>();

    #region IMeshGenerator
    public abstract Mesh Generate();
    public abstract string Name{ get; }
    #endregion

    #region IWorldObject
    public Mesh mesh {
      get { return Generate(); }
    }

    public abstract void InitializeWithWorldObject(WorldObject obj);
    #endregion
}
