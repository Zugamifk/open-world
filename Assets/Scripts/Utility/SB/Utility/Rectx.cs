using UnityEngine;
using System.Collections;

//Rect extension and helper methods
public static class Rectx {
	
	//Create Rect from (position, size) as Vector2s
	public static Rect MakeRect(Vector2 position, Vector2 size) {
		return new Rect(
			position.x,
			position.y,
			size.x,
			size.y
		);
	}
	
	//Get (x,y) as a Vector2
	public static Vector2 GetPosition(this Rect r) {
		return new Vector2(r.x, r.y);
	}
	
	//Get (width, height) as a Vector2
	public static Vector2 GetSize(this Rect r) {
		return new Vector2(r.width, r.height);
	}
	
	//Get the bounds of a group of Rects
	public static Rect BoundingRect(params Rect[] rs) {
		if (rs.Length == 0) {
			return new Rect(0,0,0,0);
		}
		float	xMin = System.Single.MaxValue,
				xMax = System.Single.MinValue,
				yMin = System.Single.MaxValue,
				yMax = System.Single.MinValue;
		foreach (Rect r in rs) {
			if (r.xMin < xMin) {
				xMin = r.xMin;
			}
			if (r.xMax > xMax) {
				xMax = r.xMax;
			}
			if (r.yMin < yMin) {
				yMin = r.yMin;
			}
			if (r.yMax > yMax) {
				yMax = r.yMax;
			}
		}
		return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
	}
	
	//Get the bounds of a group of points
	public static Rect BoundingRect(params Vector2[] vs) {
		if (vs.Length == 0) {
			return new Rect(0,0,0,0);
		}
		float	xMin = System.Single.MaxValue,
				xMax = System.Single.MinValue,
				yMin = System.Single.MaxValue,
				yMax = System.Single.MinValue;
		foreach (Vector2 v in vs) {
			if (v.x < xMin) {
				xMin = v.x;
			}
			if (v.x > xMax) {
				xMax = v.x;
			}
			if (v.y < yMin) {
				yMin = v.y;
			}
			if (v.y > yMax) {
				yMax = v.y;
			}
		}
		return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
	}
	
	//Expand or contract a Rect
	public static Rect Border(this Rect r, Vector2 borders) {
		return new Rect(
			r.x - borders.x,
			r.y - borders.y,
			r.width + 2*borders.x,
			r.height + 2*borders.y
		);
	}
	
	//Does this Rect contain another?
	public static bool Contains(this Rect r, Rect other) {
		return	r.x < other.x &&
				r.y < other.y &&
				r.xMax > other.xMax &&
				r.yMax > other.yMax;
	}
	
	// The centre of the Rect
	public static Vector2 GetCenter(this Rect r) {
		return new Vector2(
			r.x + r.width*0.5f,
			r.y + r.height*0.5f
		);
	}
	
	//Returns whether two rects intersect
	public static bool Intersects(this Rect r, Rect other){
		//Define rects by corner points, with first points less than or equal to second points
		float rx1, rx2, ry1, ry2, ox1, ox2, oy1, oy2;
		if(r.width > 0){
			rx1 = r.x;
			rx2 = r.xMax;
		}
		else{
			rx1 = r.xMax;
			rx2 = r.x;
		}
		if(r.height > 0){
			ry1 = r.y;
			ry2 = r.yMax;
		}
		else{
			ry1 = r.yMax;
			ry2 = r.y;
		}
		if(other.width > 0){
			ox1 = other.x;
			ox2 = other.xMax;
		}
		else{
			ox1 = other.xMax;
			ox2 = other.x;
		}
		if(other.height > 0){
			oy1 = other.y;
			oy2 = other.yMax;
		}
		else{
			oy1 = other.yMax;
			oy2 = other.y;
		}
		
		return rx1 < ox2 && rx2 > ox1 && ry1 < oy2 && ry2 > oy1;
	}
	
	//Returns whether two rects intersect and the rect that is the intersection.
	public static bool Intersection(this Rect r, Rect other, out Rect result){
		//Define rects by corner points, with first points less than or equal to second points
		float rx1, rx2, ry1, ry2, ox1, ox2, oy1, oy2;
		if(r.width > 0){
			rx1 = r.x;
			rx2 = r.xMax;
		}
		else{
			rx1 = r.xMax;
			rx2 = r.x;
		}
		if(r.height > 0){
			ry1 = r.y;
			ry2 = r.yMax;
		}
		else{
			ry1 = r.yMax;
			ry2 = r.y;
		}
		if(other.width > 0){
			ox1 = other.x;
			ox2 = other.xMax;
		}
		else{
			ox1 = other.xMax;
			ox2 = other.x;
		}
		if(other.height > 0){
			oy1 = other.y;
			oy2 = other.yMax;
		}
		else{
			oy1 = other.yMax;
			oy2 = other.y;
		}
		
		if(rx1 < ox2 && rx2 > ox1 && ry1 < oy2 && ry2 > oy1){
			float x = Mathf.Max(rx1, ox1);
			float y = Mathf.Max(ry1, oy1);			
			result = new Rect(x, y, Mathf.Min(rx2, ox2) - x, Mathf.Min(ry2, oy2) - y);
			return true;
		}
		else{
			result = new Rect();
			return false;
		}
	}

	public static Vector2 Bound(this Rect r, Vector2 pos) {
		if (r.Contains(pos)) return pos;

		if(pos.x < r.xMin) { pos.x = r.xMin; }
		if(pos.x > r.xMax) { pos.x = r.xMax; }
		if(pos.y < r.yMin) { pos.y = r.yMin; }
		if(pos.y > r.yMax) { pos.y = r.yMax; }

		return pos;
	}

	public static Rect Bound(this Rect r, Rect other) {
		if (r.Contains(other)) return other;

		var res = new Rect(other);
		if(other.x < r.x) { res.x += r.x-other.x; }
		if(other.xMax > r.xMax) { res.x += r.xMax-other.xMax; }
		if(other.y < r.y) { res.y += r.y-other.y; }
		if(other.yMax > r.yMax) { res.y += r.yMax-other.yMax; }

		return res;
	}
}
