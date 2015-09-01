using UnityEngine;
using System.Collections;

public class Tree : Plant {
    [InspectorButton("Initialize", "Initialize", "Derive", "boop")]
    public string derivation = "Not Initialized";

    private class DefaultTree : IPLSystem {
        public PLSystem System {
            get; private set;
        }
        public void InitializeSystem() {
            this.System = new PLSystem();
        }
    }

    string Initialize() {
        structure = new DefaultTree();
        return "assaddasd";
    }

    string Derive() {
        return "boop";
    }
}
