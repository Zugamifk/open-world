using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PTurtle : Turtle3D {

    new public PLSystem system = new PLSystem();
    private PLSystem.Word[] currentDerivation;
    public override DirectedGraph<Vector3> Path(int derivations)
	{
		ResetStack();
		currentDerivation = system.ElementAt(derivations);

        var turtleTransform = (new GameObject()).transform;
		var points = new List<Vector3>();
		var connections = new List<int>();
		bool turned = true;
		int lastpt = -1;
		int currentpt = 0;

		if (currentDerivation == null)
		{
			return null;
		}
		for (int i = 0; !currentDerivation[i].isTerminator; i++)
		{
            var current = currentDerivation[i];
            switch (currentDerivation[i].name[0])
			{
				case '+':
					{
						turtleTransform.Rotate(0,current.angle,0);
						turned = true;
					}
					break;
				case '-':
					{
						turtleTransform.Rotate(0,-current.angle,0);
						turned = true;
					}
					break;
				case '&':
					{
						turtleTransform.Rotate(current.angle,0,0);
						turned = true;
					}
					break;
				case '^':
					{
						turtleTransform.Rotate(-current.angle,0,0);
						turned = true;
					}
					break;
				case '<':
					{
						turtleTransform.Rotate(0,0,current.angle);
					}
					break;
				case '>':
					{
						turtleTransform.Rotate(0,0,-current.angle);
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
						if (current.name[0].IsUpper())
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
							turtleTransform.localPosition += turtleTransform.forward*current.distance;
						}
						else
						{
							Debug.Log("Bad word in string: \'" + current.name + "\'");
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

  public struct AugmentedVertex {
    public readonly Vector3 position;
    public PLSystem.Word word;
    public AugmentedVertex(Vector3 position, PLSystem.Word word) {
      this.position = position;
      this.word = word.Copy();
    }
  }

  public DirectedGraph<AugmentedVertex> PathAugmented(int derivations)
{
  ResetStack();
  currentDerivation = system.ElementAt(derivations);

      var turtleTransform = (new GameObject()).transform;
  var points = new List<AugmentedVertex>();
  var connections = new List<int>();
  int lastpt = -1;
  int currentpt = 0;

  if (currentDerivation == null)
  {
    return null;
  }
  for (int i = 0; !currentDerivation[i].isTerminator; i++)
  {
          var current = currentDerivation[i];
          switch (currentDerivation[i].name[0])
    {
      case '+':
        {
          turtleTransform.Rotate(0,current.angle,0);
        }
        break;
      case '-':
        {
          turtleTransform.Rotate(0,-current.angle,0);
        }
        break;
      case '&':
        {
          turtleTransform.Rotate(current.angle,0,0);
        }
        break;
      case '^':
        {
          turtleTransform.Rotate(-current.angle,0,0);
        }
        break;
      case '<':
        {
          turtleTransform.Rotate(0,0,current.angle);
        }
        break;
      case '>':
        {
          turtleTransform.Rotate(0,0,-current.angle);
        }
        break;
      case '|':
        {
          turtleTransform.Rotate(0,180,0);
        }
        break;
      case '[':
        {
          PushState(turtleTransform.localPosition, turtleTransform.localRotation, currentpt);
        }
        break;
      case ']':
        {
          points.Add(new AugmentedVertex(turtleTransform.localPosition, currentDerivation[i-1].Terminal()));
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
          if (current.name[0].IsUpper())
          {
            points.Add(new AugmentedVertex(turtleTransform.localPosition, currentDerivation[i]));
            if (lastpt >= 0)
            {
              connections.Add(lastpt);
              connections.Add(currentpt);
            }
            lastpt = currentpt;
            currentpt++;
            turtleTransform.localPosition += turtleTransform.forward*current.distance;
          }
          else
          {
            Debug.Log("Bad word in string: \'" + current.name + "\'");
          }
        }
        break;
    }
  }
  points.Add(new AugmentedVertex(turtleTransform.localPosition, points.LastOrDefault().word.Terminal()));
  if (lastpt >= 0)
  {
    connections.Add(lastpt);
    connections.Add(currentpt);
  }

  DestroyImmediate(turtleTransform.gameObject);
  return new DirectedGraph<AugmentedVertex>(points, connections);
}

    public override string ToString() {
        if(currentDerivation == null) return string.Empty;
        return string.Join(
            string.Empty,
            currentDerivation.TakeWhile(w => !w.isTerminator).Select(d => d.prettyName).ToArray());
    }

}
