using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Extensions.Managers
{
    public delegate void OnButtonDown();
    public delegate void OnButtonHeld();
    public delegate void OnButtonUp();
    public delegate void AxisUpdate(float value);

    public enum InputKey
    {
        NONE,
        MOVE_HORIZONTAL,
        MOVE_VERTICAL,
        LOOK_HORIZONTAL,
        LOOK_VERTICAL,
        BUTTON0,
        BUTTON1,
        BUTTON2,
        BUTTON3,
    }

    public class InputManager : MonoBehaviour {
        public Dictionary<string, OnButtonDown> ButtonDownCallbacks;
        public Dictionary<string, OnButtonUp> ButtonUpCallbacks;
        public Dictionary<string, OnButtonHeld> ButtonHeldCallbacks;
        public Dictionary<string, AxisUpdate> AxisUpdateCallbacks;

        public static string[] KeyStrings = new string[] {
            "None",
            "Horizontal",
            "Vertical",
            "Mouse X",
            "Mouse Y",
            "Submit"
        };

        private static InputManager instance;

        public static string GetKeyString(InputKey key) {
            var i = (int)key;
            if(i>=KeyStrings.Length)
                return KeyStrings[0];
            else
                return KeyStrings[i];
        }

        public static Vector3 MouseScreenPosition {
            get {
                return Input.mousePosition;
            }
        }

        public static void RegisterAxisUpdateCallback(InputKey axis, AxisUpdate callback) {
            var key = GetKeyString(axis);
            instance.AxisUpdateCallbacks[key] += callback;
        }

        public static void RegisterButtonDownCallback(InputKey button, OnButtonDown callback) {
            var key = GetKeyString(button);
            instance.ButtonDownCallbacks[key] += callback;
        }

        public static void RegisterButtonHeldCallback(InputKey button, OnButtonHeld callback) {
            var key = GetKeyString(button);
            instance.ButtonHeldCallbacks[key] += callback;
        }

        public static void RegisterButtonUpCallback(InputKey button, OnButtonUp callback) {
            var key = GetKeyString(button);
            instance.ButtonUpCallbacks[key] += callback;
        }

        void Awake() {
            this.SetInstanceOrKill(ref instance);
            ButtonDownCallbacks = new Dictionary<string, OnButtonDown>();
            KeyStrings.ForEach(key => ButtonDownCallbacks.Add(key, null));

            ButtonUpCallbacks = new Dictionary<string, OnButtonUp>();
            KeyStrings.ForEach(key => ButtonUpCallbacks.Add(key, null));

            ButtonHeldCallbacks = new Dictionary<string, OnButtonHeld>();
            KeyStrings.ForEach(key => ButtonHeldCallbacks.Add(key, null));

            AxisUpdateCallbacks = new Dictionary<string, AxisUpdate>();
            KeyStrings.ForEach(key => AxisUpdateCallbacks.Add(key, null));
        }

        void UpdateInput(InputKey key) {
            string strKey = GetKeyString(key);

            var downCB = ButtonDownCallbacks[strKey];
            if(downCB!=null &&
                Input.GetButtonDown(strKey))
            {
                downCB();
            }

            var heldCB = ButtonHeldCallbacks[strKey];
            if(heldCB!=null &&
                Input.GetButton(strKey))
            {
                heldCB();
            }

            var upCB = ButtonUpCallbacks[strKey];
            if(upCB!=null &&
                Input.GetButtonUp(strKey))
            {
                upCB();
            }

            var axisCB = AxisUpdateCallbacks[strKey];
            if(axisCB!=null) {
                var axisVal = Input.GetAxis(strKey);
                axisCB(
                    axisVal
                );
            }
        }

        void Update() {
            foreach (InputKey key in System.Enum.GetValues(typeof(InputKey)))
            {
                UpdateInput(key);
            }
        }
    }
}
