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

            system.axiom = new TrunkWord("T", 1f, 1f);
            system.AddProduction(
                system.axiom,
                new PLSystem.WordScheme[] {
                    TrunkWord.TrunkRadiusFunc(r=>r*0.75f),
                    PLSystem.ConstScheme(new PLSystem.Word("[")),
                    PLSystem.ConstScheme(new PLSystem.Word("+", 30f)),
                    PLSystem.ParamFuncScheme(f=>f+1),
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
      return mesh;
    }
#endregion

  private int currentLevel = 0;
    string Initialize() {
        structure = new DefaultTree();
        structure.InitializeSystem();
        turtle.system = structure.System;
        currentLevel = 0;
        path = turtle.PathAugmented(currentLevel);
        return turtle.ToString();
    }

    string Derive() {
      currentLevel++;
      path = turtle.PathAugmented(currentLevel);
      return turtle.ToString();
    }

  private const int maxGizmos = 2048;
  public DirectedGraph<PTurtle.AugmentedVertex> path = null;
  public void OnDrawGizmos()
  {
      if (path == null) return;

    var startCol = (ColorHSV)Colorx.FromHex(0xFF00FF);
    var endCol = (ColorHSV)Colorx.FromHex(0x00FF00);
    var len = Mathf.Min(path.Vertices.Count, maxGizmos);
    for (int i = 0; i < len; i++)
    {
      var col = ColorHSV.Lerp(startCol, endCol, (float)i / (float)len);
      Gizmos.color = col;
      var node = path.Vertices[i].value;
      var trunk = node.word as DefaultTree.TrunkWord;
      var nodeRadius = trunk!=null?trunk.radius:0.5f;
      Gizmos.DrawSphere(transform.position + node.position, nodeRadius);

      foreach (var e in path.Vertices[i].Outgoing)
      {
        Gizmos.DrawLine(
          transform.position + e.from.value.position,
          transform.position + e.to.value.position
        );
      }
    }
  }
}
