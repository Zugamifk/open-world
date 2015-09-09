using UnityEngine;
using System.Collections;

namespace LSystems {
public interface IPLSystem {
	PLSystem System { get; }

    void InitializeSystem();
}
}
