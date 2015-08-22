using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Turtle3D : Turtle	 {

	new public static Turtle3D instance;

	#region bracket stack
	private struct State
	{
		public Vector3 position;
		public Quaternion rotation;
		public int lastI;
	}

	private State[] StateStack = new State[STACK_MAX];

	private void ResetStack()
	{
		StackIndex = 0;
	}

	private void PushState(Vector3 position, Quaternion rotation, int i)
	{
		if (StackIndex >= STACK_MAX)
		{
			throw new System.InvalidOperationException("Stack exceeded capacity! Number of items: " + StackIndex);
		}
		StateStack[StackIndex].position = position;
		StateStack[StackIndex].rotation = rotation;
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
	new public DirectedGraph<Vector3> Path(int derivations)
	{
		ResetStack();
		current = system.ElementAt(derivations);

		var turtleTransform = (new GameObject()).transform;
		var points = new List<Vector3>();
		var connections = new List<int>();
		bool turned = true;
		int lastpt = -1;
		int currentpt = 0;

		var left = Quaternion.AngleAxis(angleStep, Vector3.up);
		var right = Quaternion.AngleAxis(-angleStep, Vector3.up);
		var up = Quaternion.AngleAxis(angleStep, Vector3.right);
		var down = Quaternion.AngleAxis(-angleStep, Vector3.right);
		var rollLeft = Quaternion.AngleAxis(angleStep, Vector3.forward);
		var rollRight = Quaternion.AngleAxis(-angleStep, Vector3.forward);
		var reverse = Quaternion.AngleAxis(180, Vector3.up);

		if (current == null)
		{
			return null;
		}
		for (int i = 0; i < current.Count(); i++)
		{
			switch (current[i])
			{
				case '+':
					{
						turtleTransform.Rotate(0,angleStep,0);
						turned = true;
					}
					break;
				case '-':
					{
						turtleTransform.Rotate(0,-angleStep,0);
						turned = true;
					}
					break;
				case '&':
					{
						turtleTransform.Rotate(angleStep,0,0);
						turned = true;
					}
					break;
				case '^':
					{
						turtleTransform.Rotate(-angleStep,0,0);
						turned = true;
					}
					break;
				case '<':
					{
						turtleTransform.Rotate(0,0,angleStep);
					}
					break;
				case '>':
					{
						turtleTransform.Rotate(0,0,-angleStep);
					}
					break;
				case '|':
					{
						turtleTransform.Rotate(0,180,0);
						turned = true;
					}
					break;
				case '[':
					{
						PushState(turtleTransform.localPosition, turtleTransform.localRotation, currentpt);
					}
					break;
				case ']':
					{
						points.Add(turtleTransform.localPosition);
						if (lastpt >= 0)
						{
							connections.Add(lastpt);
							connections.Add(currentpt);
						}
						currentpt++;

						var state = PopState();
						turtleTransform.localPosition = state.position;
						turtleTransform.localRotation = state.rotation;
						lastpt = state.lastI;
					}
					break;
				default:
					{
						if (current[i].IsUpper())
						{
							if (turned)
							{
								turned = false;
								points.Add(turtleTransform.localPosition);
								if (lastpt >= 0)
								{
									connections.Add(lastpt);
									connections.Add(currentpt);
								}
								lastpt = currentpt;
								currentpt++;
							}
							turtleTransform.localPosition += turtleTransform.forward*step;
						}
						else
						{
							Debug.Log("Bad character in string: \'" + current[i] + "\'");
						}
					}
					break;
			}
		}
		points.Add(turtleTransform.localPosition);
		if (lastpt >= 0)
		{
			connections.Add(lastpt);
			connections.Add(currentpt);
		}

		DestroyImmediate(turtleTransform.gameObject);
		return new DirectedGraph<Vector3>(points, connections);
	}
	#endregion path generation
	void Awake()
	{
		this.SetInstanceOrKill(ref instance);
		StackIndex = 0;
		StateStack = new State[STACK_MAX];
	}

}
