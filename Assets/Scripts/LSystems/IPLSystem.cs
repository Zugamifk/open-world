using UnityEngine;
using System.Collections;

public interface IPLSystem {
	PLSystem System { get; }

    void InitializeSystem();
}
