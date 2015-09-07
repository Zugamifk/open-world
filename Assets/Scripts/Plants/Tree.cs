using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tree : Plant {
    [InspectorButton("Initialize", "Initialize", "Derive", "boop")]
    public string derivation = "Not Initialized";

    private class DefaultTree : IPLSystem {

        public class TrunkWord : PLSystem.Word {
          public float radius;
          public TrunkWord(string name, float distance, float radius)
          : base (name, distance) {
            this.radius = radius;
          }
          public static PLSystem.WordScheme TrunkRadiusFunc(System.Func<float, float> func) {
            return word => {
              var trunk = word as TrunkWord;
              trunk.radius = func(trunk.radius);
              return trunk;
            };
          }
          public override string prettyName {
            get {
              return name + "("+distance+", "+radius+")";
            }
          }
          public override PLSystem.Word Copy() {
            return new TrunkWord( name, distance, radius );
          }
          public override PLSystem.Word Terminal() {
            return new TrunkWord(name, 0,0);
          }
        }

        public PLSystem System {
            get; private set;
        }
        public void InitializeSystem() {
            var system = new PLSystem();

            system.SetAxiom(new PLSystem.Word("^", 90f), new TrunkWord("T", .02f, .01f));
            system.AddProduction(
                "T",
                new PLSystem.WordScheme[] {
                    TrunkWord.TrunkRadiusFunc(r=>r*0.75f),
                    PLSystem.ConstScheme(new PLSystem.Word("[")),
                    PLSystem.ConstScheme(new PLSystem.Word("+", 30f)),
                    PLSystem.ParamFuncScheme(f=>f+0.01f),
                    PLSystem.ConstScheme(new PLSystem.Word("]")),
                    PLSystem.ParamFuncScheme(f=>f*2)
                }
            );

            this.System = system;
        }
    }

#region IMeshGenerator implementation
    public override string Name {
      get {
        return "Tree";
      }
    }

    private void GetPathsRecurse(
      Graph<PTurtle.AugmentedVertex>.Vertex<PTurtle.AugmentedVertex> current,
      List<PTurtle.AugmentedVertex> currentPath,
      List<List<PTurtle.AugmentedVertex>> collection) {
      currentPath.Add(current.value);
      int i=0;
      foreach(var b in current.Outgoing) {
        if(i==0) {
          GetPathsRecurse(b.to, currentPath, collection);
        } else {
          var newPath = new List<PTurtle.AugmentedVertex>();
          newPath.Add(current.value);
          collection.Add(newPath);
          GetPathsRecurse(b.to, newPath, collection);
        }
        i++;
      }
    }

    public List<List<PTurtle.AugmentedVertex>> GetPaths() {
      if(path==null) return null;

      var paths = new List<List<PTurtle.AugmentedVertex>>();
      var currentPath = new List<PTurtle.AugmentedVertex>();
      paths.Add(currentPath);
      var current = path.Vertices[0];
      GetPathsRecurse(current, currentPath, paths);
      return paths;
    }
    public override Mesh Generate() {
      var branchPaths = GetPaths();

      var generator = new TubeGenerator();
      var mesh = new Mesh();
      var combiners = new CombineInstance[branchPaths.Count];
      for(int i=0;i<branchPaths.Count;i++) {
        generator.Clear();
        branchPaths[i].ForEach(pt=>generator.AddPoint(pt.position, ((DefaultTree.TrunkWord)pt.word).radius));
        combiners[i].mesh = generator.Generate();
      }
      mesh.CombineMeshes(combiners, true, false);

      MeshGenerator.Utils.PostGenerateMesh(mesh);

      mesh.name = Name;

      return mesh;
    }
#endregion

#region IWorldObject implementation
  public override void InitializeWithWorldObject(WorldObject obj) {
    structure = new DefaultTree();
    structure.InitializeSystem();
    SetLevel(4);
  }
#endregion

  public DirectedGraph<PTurtle.AugmentedVertex> path = null;
  private int currentLevel = 0;
    string Initialize() {
        structure = new DefaultTree();
        structure.InitializeSystem();
        currentLevel = 0;
        var derivation = structure.System.First();
        path = PTurtle.PathAugmented(derivation);
        return PLSystem.DerivationToString(derivation);
    }

    string Derive() {
      currentLevel++;
      var derivation = structure.System.ElementAt(currentLevel);
      path = PTurtle.PathAugmented(derivation);
      return PLSystem.DerivationToString(derivation);
    }

    public void SetLevel(int level) {
      currentLevel = level;
      var derivation = structure.System.ElementAt(currentLevel);
      path = PTurtle.PathAugmented(derivation);
    }
}
