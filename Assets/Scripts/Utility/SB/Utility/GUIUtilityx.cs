using UnityEngine;
using System.Collections.Generic;

/** Utility class which helps manage the UnityGUI.
	Contains three main features: 
	1. An enabledness "stack"
	2. A GUI color stack.
	3. Tools for converting between UI spaces, calculating typical useful Rects, etc.
	The two stacks should only be used within OnGUI functions and if a stack
	is used it should be emptied before the OnGUI ends.
	
	The enabledness stack is used for conveniently handling nested GUI components
	where disabling one component disables all components nested within it. Once
	a False has been pushed to the stack the GUI remains disabled until that element
	is popped. The value of the top element of the stack is not of particular importance.
	Note that since the enabledness stack is implemented as a bitfield it has a 
	size limit of 32 entries.
	
	The color stack acts as a history of past colors -- the top of the stack is not
	the current color but the immediately previous color.
*/
public static class GUIUtilityx {
	
	private static int	m_enabledStack = ~0, // stack of bits, starts all 1's
							m_enabledTop = 0;
	private static Stack<Color> m_colorStack = new Stack<Color>();
	
	/** Push a GUI enabledness to the stack. The resulting enabledness state is applied immediately. */
	public static void PushEnabled(bool enabled) {
		if (enabled) {
			m_enabledStack |= (1 << ++m_enabledTop); //push a 1
		} else {
			m_enabledStack &= ~(1 << ++m_enabledTop); //push a 0
		}
		GUI.enabled = m_enabledStack == ~0; // test for all 1's
	}
	
	/** Pop the top enabledness from the stack. The resulting enabledness state is applied immediately. */
	public static void PopEnabled() {
		m_enabledStack |= (1 << m_enabledTop--); // pop bit and set to 1
		GUI.enabled = m_enabledStack == ~0; // test for all 1's
	}
	
	// functions for converting to and from 2.0 GUI space by reflecting Y
	
	static public Rect Convert(Rect r) {
		return new Rect(r.x,
						Screen.height - r.y - r.height,
						r.width,
						r.height);
	}
	
	static public Vector3 Convert(Vector3 p) {
		return new Vector3(	p.x,
							Screen.height - p.y,
							p.z);
	}
	
	static public Vector2 Convert(Vector2 p) {
		return new Vector2(	p.x,
							Screen.height - p.y);
	}
	
	// get screen-centred centred Rects
	
	static public Rect CentredRect(Rect r) {
		return CentredRect(new Vector2(r.width, r.height));
	}
	
	static public Rect CentredRect(Vector2 v) {
		return new Rect((Screen.width - v.x)/2f,
						(Screen.height - v.y)/2f,
						v.x,
						v.y);
	}
	
	/* Spits out a small white texture. Functionally very similar to EditorGUIUtility.WhiteTexture but usable outside of the editor.
		Do not modify the returned texture, it is shared.
	*/
	static public Texture2D WhiteTexture {
		get {
			if (!m_whiteTexture) {
				m_whiteTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
				Color[] pixels = new Color[4];
				for (int i = 0; i < pixels.Length; ++i) {
					pixels[i] = Color.white;
				}
				m_whiteTexture.SetPixels(pixels);
				m_whiteTexture.Apply();
			}
			return m_whiteTexture;
		}
	}
	static private Texture2D m_whiteTexture;
	
	/** Push a color. The current GUI color is saved to the stack, the given color is applied immediately. */
	public static void PushColor(Color c){
		m_colorStack.Push(GUI.color);		
		GUI.color = c;
	}
	
	/** Pop the color stack. The current GUI color is set to the top element of the stack, which is removed. */
	public static void PopColor(){
		GUI.color = m_colorStack.Pop();
	}
}
