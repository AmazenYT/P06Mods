using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LinearBez {
	public Vector3[] knots;
	private float[] param_data;	
	private float length;
	
	public LinearBez (Vector3[] _knots) {
		this.knots = _knots;
		this.param_data = new float[_knots.Length];
		Parametrize ();
	}	
	
	void Parametrize () {
		int points = (knots.Length-1);
		this.length = 0.0f;
		Vector3 lastPoint = GetPosition (0.0f);

		this.param_data[0] = 0.0f;
		
		for (int i=0; i<points; i++) {
			float time = (1.0f+i)/points;
			
			this.param_data[i+1] = time;
			
			Vector3 point = GetPosition (time);
			this.length+= Vector3.Distance (point, lastPoint);
			lastPoint = point;
		}	
	}	

	public float Length () {
        return this.length;
	}	

	public float GetTime (int index) {
		if (index < 0) {
			return param_data[0];
		}
        return this.param_data[index];
	}

    public int GetSegment (float t) { // get segment and retrieve the delta value to t
		int nbSegment = knots.Length - 1;
        float tRect = t * nbSegment;
        int seg = (int) tRect; // cast to int to get segment
        tRect -= seg; // 0-1 for that segment
        t = tRect;
		
		if (seg>nbSegment-1) {
			t = 1.0f;
			return nbSegment-1;		
		} else
            return seg;
    }
	
    public int GetSegment (ref float t) { // get segment and retrieve the delta value to t
		int nbSegment = knots.Length - 1;
        float tRect = t * nbSegment;
        int seg = (int) tRect; // cast to int to get segment
        tRect -= seg; // 0-1 for that segment
        t = tRect;
		
		if (seg>nbSegment-1) {
			t = 1.0f;
			return nbSegment-1;		
		} else
            return seg;
    }	
	
	public Vector3 GetPosition (float t) {
        if (t <= 0f) return knots[0];
        if (t >= 1f) return knots[knots.Length-1];	

        int seg = GetSegment (ref t);
        if (t == 0f) return knots[seg];
		
		//float d = 1f - t;
		return Vector3.Lerp (knots[seg], knots[seg+1], t); //(knots[seg] * t) + (knots[seg+1] * d);
	}

	public Vector3 GetTangent (float t) {
        if (t <= 0f) return (knots[1] - knots[0]).normalized;
        if (t >= 1f) return (knots[knots.Length-1] - knots[knots.Length-2]).normalized;	

        int seg = GetSegment (ref t);
        //if (t == 0f) return (knots[seg+1] - knots[seg]).normalized;

        return (knots[seg+1] - knots[seg]).normalized;
		
		/*float d = 1f - t;		
		return ((knots[seg+1] * d) - (knots[seg] * t)).normalized;*/
	}	
}

[System.Serializable]
public class QuadBez {
	public Vector3 st, en, ctrl;
	
	public QuadBez(Vector3 st, Vector3 en, Vector3 ctrl) {
		this.st = st;
		this.en = en;
		this.ctrl = ctrl;
	}
	
	
	public Vector3 Interp(float t) {
		float d = 1f - t;
		return d * d * st + 2f * d * t * ctrl + t * t * en;
	}
	
	
	public Vector3 Velocity(float t) {
		return (2f * st - 4f * ctrl + 2f * en) * t + 2f * ctrl - 2f * st;
	}
	
	
	public void GizmoDraw(float t) {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(st, ctrl);
		Gizmos.DrawLine(ctrl, en);
		
		Gizmos.color = Color.white;
		Vector3 prevPt = st;
		
		for (int i = 1; i <= 20; i++) {
			float pm = (float) i / 20f;
			Vector3 currPt = Interp(pm);
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}
		
		Gizmos.color = Color.blue;
		Vector3 pos = Interp(t);
		Gizmos.DrawLine(pos, pos + Velocity(t));
	}
	

}


[System.Serializable]
public class CubicBez {
	public Vector3 st, en, ctrl1, ctrl2;
	
	public CubicBez(Vector3 st, Vector3 en, Vector3 ctrl1, Vector3 ctrl2) {
		this.st = st;
		this.en = en;
		this.ctrl1 = ctrl1;
		this.ctrl2 = ctrl2;
	}
	
	
	public Vector3 Interp(float t) {
		float d = 1f - t;
		return d * d * d * st + 3f * d * d * t * ctrl1 + 3f * d * t * t * ctrl2 + t * t * t * en;
	}
	
	
	public Vector3 Velocity(float t) {
		return (-3f * st + 9f * ctrl1 - 9f * ctrl2 + 3f * en) * t * t
			+  (6f * st - 12f * ctrl1 + 6f * ctrl2) * t
			-  3f * st + 3f * ctrl1;
	}
	
	
	public void GizmoDraw(float t) {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(st, ctrl1);
		Gizmos.DrawLine(ctrl2, en);
		
		Gizmos.color = Color.white;
		Vector3 prevPt = st;
		
		for (int i = 1; i <= 20; i++) {
			float pm = (float) i / 20f;
			Vector3 currPt = Interp(pm);
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}
		
		Gizmos.color = Color.blue;
		Vector3 pos = Interp(t);
		Gizmos.DrawLine(pos, pos + Velocity(t));
	}
}


[System.Serializable]
public class CRSpline {
	public List<Vector3> knots;
	private List<Vector3> lastKnots;
	
	/*public CRSpline(params Vector3[] knots) {
		this.knots = new Vector3[knots.Count];
		System.Array.Copy(knots, this.knots, knots.Count);
	}*/
	
	public CRSpline () {
		knots = new List<Vector3> ();
	}

	/*public Ray GetPositionTangent (float t) {
        float time = this.GetUnscaledTime (t);	
		return new Ray (this.InterpolatePosition (time), this.InterpolateTangent (time));
	}*/
	
    public Vector3 GetPosition (float t) {
		if (knots.Count<4)
			return Vector3.zero;
		
		int numSections = knots.Count - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
				
		Vector3 a = knots[currPt];
		Vector3 b = knots[currPt + 1];
		Vector3 c = knots[currPt + 2];
		Vector3 d = knots[currPt + 3];
		
		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
		);
	}	
	
	public Vector3 GetTangent (float t) {
		if (knots.Count<4)
			return Vector3.zero;
		
		int numSections = knots.Count - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
				
		Vector3 a = knots[currPt];
		Vector3 b = knots[currPt + 1];
		Vector3 c = knots[currPt + 2];
		Vector3 d = knots[currPt + 3];

		return (1.5f * (-a + 3f * b - 3f * c + d) * (u * u)
				+ (2f * a -5f * b + 4f * c - d) * u
				+ .5f * c - .5f * a).normalized;
	}
	
	public Vector3 GetVelocity (float t) {
		if (knots.Count<4)
			return Vector3.zero;
		
		int numSections = knots.Count - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
				
		Vector3 a = knots[currPt];
		Vector3 b = knots[currPt + 1];
		Vector3 c = knots[currPt + 2];
		Vector3 d = knots[currPt + 3];

		return 1.5f * (-a + 3f * b - 3f * c + d) * (u * u)
				+ (2f * a -5f * b + 4f * c - d) * u
				+ .5f * c - .5f * a;
	}

	public void GizmoDraw () {
		if (knots.Count<4)
			return;
		
		Gizmos.color = Color.white;
		Vector3 prevPt = GetPosition(0);
		
		for (int i = 1; i <= 20; i++) {
			float pm = (float) i / 20f;
			Vector3 currPt = GetPosition(pm);
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}
		
        for(int i=0; i<knots.Count; i++) {
			if (i==0)
                Gizmos.color = Color.green;
		    else if (i==knots.Count-1)
				Gizmos.color = Color.red;
			else
				Gizmos.color = Color.blue;				
			
            Gizmos.DrawWireSphere(knots[i], 0.2f);
        }
	}	
	
	public void GizmoDraw (float t) {
		if (knots.Count<4)
			return;
		
		Gizmos.color = Color.white;
		Vector3 prevPt = GetPosition(0);
		
		for (int i = 1; i <= 20; i++) {
			float pm = (float) i / 20f;
			Vector3 currPt = GetPosition(pm);
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}
		
		Gizmos.color = Color.blue;
		Vector3 pos = GetPosition(t);
		Gizmos.DrawLine(pos, pos + GetVelocity(t));
	}
}