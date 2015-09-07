using UnityEngine;
using System.Collections;

public class PLSystemTest : MonoBehaviour, IPLSystem {

    private PLSystem system = new PLSystem();
    public PLSystem System {
		get { return system; }
	}

    public void InitializeSystem() {
        system = new PLSystem();
        system.SetAxiom(new PLSystem.Word("F", 1f));
        system.AddProduction(
            "F",
            new PLSystem.WordScheme[] {
                PLSystem.IdentityScheme,
                PLSystem.ConstScheme(new PLSystem.Word("[")),
                PLSystem.ConstScheme(new PLSystem.Word("+", 30f)),
                PLSystem.ParamFuncScheme(f=>f+1),
                PLSystem.ConstScheme(new PLSystem.Word("]")),
                PLSystem.ParamFuncScheme(f=>f*2)
            });
    }

    void Awake() {
        InitializeSystem();
    }
}
