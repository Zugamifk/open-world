using UnityEngine;
using System.Collections;

/*  Defines the resolution and world space the game is created in, and contains
	utility functions for scaling UnityGUI to match non-standard resolutions and
	aspect ratios. 
	
	Create a class called "ScreenxSettingsOverride" that extends ScreenxSettings
	to specify your own values.
*/

public class Screenx {
	//The resolution designed for
	public static Rect NormalScreen{
		get{
			return CurrentSettings.NormalScreen;
		}
	}
	//The area of the scene typically on camera
	public static Rect NormalWorld{
		get{
			return CurrentSettings.NormalWorld;
		}
	}
	public static float NormalAspect = NormalScreen.width/NormalScreen.height; //Aspect ratio of NormalScreen

	//Whether screen is bordered (true) or stretched (false) if aspect ratio differs from normal
	public static bool ScreenLetterboxed{
		get{
			return CurrentSettings.ScreenLetterboxed;
		}
	}

	//returns ratio of current resolution to "normal" resolution
	public static Vector2 ResolutionRatio{
		get{
			if(!ScreenLetterboxed){
				return new Vector2(Screen.width / NormalScreen.width, Screen.height / NormalScreen.height);
			}
			else{
				float small = Mathf.Min(Screen.width / NormalScreen.width, Screen.height / NormalScreen.height);
				return new Vector2(small, small);
			}
		}
	}
	
	//Screen pixels per world unit in the design resolution
	//E.g. If the camera is showing an 800x600 world space on a 1024x768 screen, this is 1.28.
	//Probably want to leave the Z coordinate as 1.
	public static Vector3 UIWorldSpaceRatio{
		get{return new Vector3(NormalScreen.width / NormalWorld.width, NormalScreen.height / NormalWorld.height, 1);} 
	}
	
	//Returns the origin of the GUI (in GUI coordinates), adjusted for screen letterboxing.
	public static Vector2 GUIOrigin{
		get{
			if(ScreenLetterboxed){
				return (new Vector2(Screen.width, Screen.height) - Vector2.Scale(new Vector2(NormalScreen.width, NormalScreen.height), ResolutionRatio))/2;
			}
			else{
				return Vector2.zero;
			}
		}
	}
	
	//Returns a Matrix4x4 that transforms to account for resolution differences between the default and current screens.
	public static Matrix4x4 AdjustedMatrix{
		get{ 
			return Matrix4x4.TRS((Vector3)GUIOrigin, Quaternion.identity, (Vector3)ResolutionRatio + Vector3.forward);
		}
	}
	
	//Gives an adjusted matrix that also conforms to the transform's local position and scale
	public static Matrix4x4 LocalAdjustedMatrix(Transform t){
		Vector3 uiSpaceTranslation = Vector3.Scale(t.localPosition, UIWorldSpaceRatio);
		uiSpaceTranslation.x += NormalScreen.width / 2;
		uiSpaceTranslation.y = -uiSpaceTranslation.y + NormalScreen.height / 2;
		uiSpaceTranslation.z = 0;
		return AdjustedMatrix * Matrix4x4.TRS(uiSpaceTranslation, Quaternion.identity, t.localScale);
	}
	
	//Gives a GUI Rect that is the visible GUI space, relative to the given transform
	public static Rect GroupRect(Transform t){
		Vector3 localPos = Vector3.Scale(t.localPosition, UIWorldSpaceRatio);
		Vector3 scale = t.localScale;
		return new Rect(
			(-NormalScreen.width/2 - localPos.x) / scale.x,
			(-NormalScreen.height/2 + localPos.y) / scale.y,
			NormalScreen.width / scale.x,
			NormalScreen.height / scale.y
			);
	}
	
	//Adjusts a layout Rect so that in a GroupRect adjusted by LocalAdjustedMatrix under Transform t, the Rect is transformed from "local" UI space to global UI space.
	public static Rect AdjustedRect(Transform t, Rect original){
		Vector3 localPos = Vector3.Scale(t.localPosition, UIWorldSpaceRatio);
		Vector3 scale = t.localScale;
		
		Rect adjustedRect = original; //Adjust the rect from local space to group space
		adjustedRect.x = adjustedRect.x + (localPos.x + NormalScreen.width/2) / scale.x;
		adjustedRect.y = adjustedRect.y + (-localPos.y + NormalScreen.height/2) / scale.y;
		return adjustedRect;
	}

	protected static ScreenxSettings CurrentSettings{
		get{
			if(m_currentSettings == null){
				//If override type exists, use it
				System.Type overrideType = System.Type.GetType("ScreenxSettingsOverride");
				if(overrideType != null){
					m_currentSettings = (ScreenxSettings)overrideType.GetConstructor(new System.Type[0]).Invoke(new System.Object[0]);
				}
				else{
					m_currentSettings = new ScreenxSettings();
				}
			}
			return m_currentSettings;
		}
		set{
			m_currentSettings = value;
		}
	}
	protected static ScreenxSettings m_currentSettings;
}

public class ScreenxSettings {	
	public virtual Rect NormalScreen{
		get{
			return new Rect(0, 0, 1024, 768);
		}
	}	
	
	public virtual Rect NormalWorld{
		get{
			return new Rect(-512, -384, 1024, 768);
		}
	} 
	public virtual bool ScreenLetterboxed{
		get{ return false; }
	}
}
