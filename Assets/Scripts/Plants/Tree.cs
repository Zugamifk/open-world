using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LSystems;
using MeshGenerator;

public class Tree : Plant
{
    [InspectorButton("Initialize", "Initialize", "Derive", "boop")]
    public string derivation = "Not Initialized";

    private class DefaultTree : IPLSystem
    {

        public class TrunkWord : PLSystem.Word
        {
            public float radius;
            public TrunkWord(string name, float distance, float radius)
            : base(name, distance)
            {
                this.radius = radius;
            }
            public static PLSystem.WordScheme TrunkRadiusFunc(System.Func<float, float> func)
            {
                return word =>
                {
                    var trunk = word as TrunkWord;
                    trunk.radius = func(trunk.radius);
                    return trunk;
                };
            }
            public override string prettyName
            {
                get
                {
                    return name + "(" + distance + ", " + radius + ")";
                }
            }
            public override PLSystem.Word Copy()
            {
                return new TrunkWord(name, distance, radius);
            }
            public override PLSystem.Word Terminal()
            {
                return new TrunkWord(name, 0, 0);
            }
        }

        public class LeafWord : PLSystem.Word
        {
            public float development;
            public LeafWord(string name, float development) : base(name) {
                this.development = development;
            }
            public static PLSystem.WordScheme DevelopmentFunc(System.Func<float, float> func)
            {
                return word =>
                {
                    var leaf = word as LeafWord;
                    leaf.development = func(leaf.development);
                    return leaf;
                };
            }
            public override string prettyName
            {
                get
                {
                    return name + "(" + development+")";
                }
            }
            public override PLSystem.Word Copy()
            {
                return new LeafWord(name, development);
            }
            public override PLSystem.Word Terminal()
            {
                return new LeafWord(name, 0);
            }
            public Mesh Generate() {
                var mesh = new Mesh();
                var verts = new Vector3[4];
                verts[0] = Vector3.zero;
                verts[1] = new Vector3(-.3f,0.4f,0)*development;
                verts[2] = Vector3.up*development;
                verts[3] = new Vector3(.3f, 0.4f,0)*development;
                var tris = Utils.GetFacePoints(0,1,2,3).ToArray();
                mesh.vertices = verts;
                mesh.triangles = Utils.GenerateBackfaces(tris);
                return mesh;
            }
        }

        public PLSystem System
        {
            get; private set;
        }
        public void InitializeSystem()
        {
            var system = new PLSystem();

            var leafPrototype = new LeafWord("L", 1);

            system.SetAxiom(new PLSystem.Word("^", 90f), new TrunkWord("T", .02f, .01f));
            system.AddProduction(
                "T",
                new PLSystem.WordScheme[] {
                    TrunkWord.TrunkRadiusFunc(r=>r*0.75f),
                    PLSystem.ConstScheme(leafPrototype),
                    PLSystem.ConstScheme(new PLSystem.Word("<", 45f)),
                    PLSystem.ConstScheme(new PLSystem.Word("[")),
                    PLSystem.ConstScheme(new PLSystem.Word("+", 30f)),
                    PLSystem.ParamFuncScheme(f=>f+0.01f),
                    PLSystem.ConstScheme(new PLSystem.Word("]")),
                    PLSystem.ParamFuncScheme(f=>f*2)
                }
            );
            system.AddProduction(
                "L",
                LeafWord.DevelopmentFunc(d => d + 0.25f)
            );

            this.System = system;
        }
    }

    #region IMeshGenerator implementation
    public override string Name
    {
        get
        {
            return "Tree";
        }
    }

    private void GetPathsRecurse(
      Graph<PTurtle.AugmentedVertex>.Vertex<PTurtle.AugmentedVertex> current,
      List<PTurtle.AugmentedVertex> currentPath,
      List<List<PTurtle.AugmentedVertex>> collection)
    {
        currentPath.Add(current.value);
        int i = 0;
        foreach (var b in current.Outgoing)
        {
            if (i == 0)
            {
                GetPathsRecurse(b.to, currentPath, collection);
            }
            else
            {
                var newPath = new List<PTurtle.AugmentedVertex>();
                newPath.Add(current.value);
                collection.Add(newPath);
                GetPathsRecurse(b.to, newPath, collection);
            }
            i++;
        }
    }

    public List<List<PTurtle.AugmentedVertex>> GetPaths()
    {
        if (path == null) return null;

        var paths = new List<List<PTurtle.AugmentedVertex>>();
        var currentPath = new List<PTurtle.AugmentedVertex>();
        paths.Add(currentPath);
        var current = path.Vertices[0];
        GetPathsRecurse(current, currentPath, paths);
        return paths;
    }
    public override Mesh Generate()
    {
        var branchPaths = GetPaths();

        if (branchPaths == null)
        {
            Debug.LogError("Error generating branch paths!");
            return new Mesh();
        }

        var leafs = path.Vertices.Where(v => v.value.word is DefaultTree.LeafWord);

        var generator = new TubeGenerator();
        var mesh = new Mesh();
        var combiners = new CombineInstance[branchPaths.Count + leafs.Count()];
        int i = 0;
        for (; i < branchPaths.Count; i++)
        {
            generator.Clear();
            branchPaths[i].Where(pt=>pt.word is DefaultTree.TrunkWord).ForEach(pt => generator.AddPoint(pt.position, ((DefaultTree.TrunkWord)pt.word).radius));
            combiners[i].mesh = generator.Generate();
        }
        foreach(var l in leafs) {
            var mat = Matrix4x4.TRS(l.value.position, l.value.rotation, Vector3.one*0.1f);
            var leaf = ((DefaultTree.LeafWord)l.value.word).Generate();
            leaf.TransformMesh(mat);
            combiners[i].mesh = leaf;
            i++;
        }
        mesh.CombineMeshes(combiners, true, false);

        Utils.PostGenerateMesh(mesh);

        mesh.name = Name;

        return mesh;
    }
    #endregion

    #region IWorldObject implementation
    public override void InitializeWithWorldObject(WorldObject obj)
    {
        structure = new DefaultTree();
        structure.InitializeSystem();
        SetLevel(4);
    }
    #endregion

    public DirectedGraph<PTurtle.AugmentedVertex> path = null;
    private int currentLevel = 0;
    string Initialize()
    {
        structure = new DefaultTree();
        structure.InitializeSystem();
        currentLevel = 0;
        var derivation = structure.System.First();
        path = PTurtle.PathAugmented(derivation);
        return PLSystem.DerivationToString(derivation);
    }

    string Derive()
    {
        currentLevel++;
        var derivation = structure.System.ElementAt(currentLevel);
        path = PTurtle.PathAugmented(derivation);
        return PLSystem.DerivationToString(derivation);
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
        var derivation = structure.System.ElementAt(currentLevel);
        path = PTurtle.PathAugmented(derivation);
    }
}
