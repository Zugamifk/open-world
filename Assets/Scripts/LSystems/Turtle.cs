using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Extensions;
using MeshGenerator;

namespace LSystems {

public class Turtle : MonoBehaviour
{

    public LSystem system = new LSystem();

    public int defaultIterations = 0;
    public float angleStep;
    public float step;
    public float[] unused;

    public static Turtle instance;

    #region bracket stack
    private struct State
    {
        public Vector3 position;
        public float angle;
        public int lastI;
    }

    protected const int STACK_MAX = 128;
    protected int StackIndex = 0;
    private State[] StateStack = new State[STACK_MAX];

    private void ResetStack()
    {
        StackIndex = 0;
    }

    private void PushState(Vector3 position, float angle, int i)
    {
        if (StackIndex >= STACK_MAX)
        {
            throw new System.InvalidOperationException("Stack exceeded capacity! Number of items: " + StackIndex);
        }
        StateStack[StackIndex].position = position;
        StateStack[StackIndex].angle = angle;
        StateStack[StackIndex].lastI = i;
        StackIndex++;
    }

    private State PopState()
    {
        if (StackIndex == 0)
        {
            throw new System.InvalidOperationException("Stack is empty! Nothing to pop");
        }
        StackIndex--;
        return StateStack[StackIndex];
    }
    #endregion bracket stack
    #region path generation
    public DirectedGraph<Vector2> Path(string derivation)
    {
        ResetStack();
        var angle = 0f;
        var pos = Vector2.zero;
        var points = new List<Vector2>();
        var connections = new List<int>();
        bool turned = true;
        int lastpt = -1;
        int currentpt = 0;

        if (derivation == null)
        {
            return null;
        }
        for (int i = 0; i < derivation.Count(); i++)
        {
            switch (derivation[i])
            {
                case '+':
                    {
                        angle += angleStep;
                        turned = true;
                    }
                    break;
                case '-':
                    {
                        angle -= angleStep;
                        turned = true;
                    }
                    break;
                case '[':
                    {
                        PushState(pos, angle, currentpt);
                    }
                    break;
                case ']':
                    {
                        points.Add(pos);
                        if (lastpt >= 0)
                        {
                            connections.Add(lastpt);
                            connections.Add(currentpt);
                        }
                        currentpt++;

                        var state = PopState();
                        pos = state.position;
                        angle = state.angle;
                        lastpt = state.lastI;
                    }
                    break;
                default:
                    {
                        if (derivation[i].IsUpper())
                        {
                            if (turned)
                            {
                                turned = false;
                                points.Add(pos);
                                if (lastpt >= 0)
                                {
                                    connections.Add(lastpt);
                                    connections.Add(currentpt);
                                }
                                lastpt = currentpt;
                                currentpt++;
                            }
                            pos += Vector2.up.Rotate(angle)*step;
                        }
                        else
                        {
                            Debug.Log("Bad character in string: \'" + derivation[i] + "\'");
                        }
                    }
                    break;
            }
        }
        points.Add(pos);
        if (lastpt >= 0)
        {
            connections.Add(lastpt);
            connections.Add(currentpt);
        }
        return new DirectedGraph<Vector2>(points, connections);
    }
    #endregion path generation
    void Awake()
    {
        this.SetInstanceOrKill(ref instance);
        StackIndex = 0;
        StateStack = new State[STACK_MAX];
    }

    // public Polygon DrawPolygon(int derivations)
    // {
    //     ResetStack();
    //     current = system.ElementAt(derivations);
    //     var heading = Vector2.up;
    //     var angle = 0f;
    //     var pos = Vector2.zero;
    //     var points = new List<Vector2>();
    //     bool turned = true;
    //
    //     if (current == null)
    //     {
    //         return null;
    //     }
    //     for (int i = 0; i < current.Count(); i++)
    //     {
    //         switch (current[i])
    //         {
    //             case '+':
    //                 {
    //                     angle += angleStep;
    //                     turned = true;
    //                 }
    //                 break;
    //             case '-':
    //                 {
    //                     angle -= angleStep;
    //                     turned = true;
    //                 }
    //                 break;
    //             default:
    //                 {
    //                     Debug.Log("Bad character in string: \'" + current[i] + "\'");
    //                 }
    //                 break;
    //         }
    //     }
    //     points.Add(pos);
    //     return new Polygon(points.ToArray());
    // }
    //
    // public class TurtleMesh : IMeshGenerator
    // {
    //     public Turtle turtle;
    //     public int depth;
    //     public Color colorStart;
    //     public Color colorEnd;
    //     public string Name
    //     {
    //         get { return "Turtle Path Mesh"; }
    //     }
    //     public Mesh Generate()
    //     {
    //         var path = turtle.DrawPolygon(depth);
    //         var mesh = path.GenerateMesh();
    //         mesh.SplitTriangles();
    //         mesh.ColorByVertexIndex(colorStart, colorEnd);
    //         turtle.unused = new float[path.Vertices.Count];
    //         MeshGenerator.Utils.PostGenerateMesh(mesh);
    //         return mesh;
    //     }
    // }
}
}
