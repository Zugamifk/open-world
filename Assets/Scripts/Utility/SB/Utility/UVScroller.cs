/*____________________________________________________________________________

UVScroller.cs
Animates UV coordinates to "scroll" a texture.
____________________________________________________________________________*/

using UnityEngine;
using System.Collections;
[AddComponentMenu("Effects/UVScroller")]
public class UVScroller : MonoBehaviour {

    //__________________________________________________/           Public \__
	public Vector2 m_bgScrollRate;
	public Vector2 BgOffset{
		get{return m_bgOffset;}
		set{m_bgOffset = value; SetTextureOffset(m_bgOffset);}
	}
	
    //__________________________________________________/        Protected \__
	private Vector2 m_bgOffset;
	private Material m_material;
	
    //__________________________________________________/    Monobehaviour \__

	private void Awake(){
		m_material = renderer.material;
		m_bgOffset = m_material.GetTextureOffset("_MainTex");
	}

	private void Update() {
		m_bgOffset += m_bgScrollRate * Time.deltaTime;
		SetTextureOffset(m_bgOffset);
	}
	
	private void SetTextureOffset(Vector2 offset){
		m_bgOffset.x = m_bgOffset.x % 1f;
		m_bgOffset.y = m_bgOffset.y % 1f;
		m_material.SetTextureOffset("_MainTex", offset);
	}
}
