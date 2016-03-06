using UnityEngine;
using System.Collections;
using Extensions.Managers;

namespace Shrines
{
    public class Player : MonoBehaviour
    {

        MovementControl controller;

        public Vector2 position;

        public delegate void PositionUpdateCallback(Vector2 position);
        public PositionUpdateCallback PositionUpdate;

        // Use this for initialization
        void Start()
        {
            controller = FindObjectOfType<MovementControl>();
            controller.RegisterVelocityUpdate(Move);

            position = new Vector2(50, 0);
        }

        public void Move(Vector2 velocity)
        {
            position += velocity * 10 * Time.deltaTime;
            if (PositionUpdate != null) PositionUpdate.Invoke(position);
        }
    }
}