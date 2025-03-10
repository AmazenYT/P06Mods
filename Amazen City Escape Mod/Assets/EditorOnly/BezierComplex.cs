using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
[System.Serializable] // allow storage of variables + display in the inspector to get them back when launching unity.
 
public class BezierComplex : System.Object {
   
    public Vector3[] nodes = new Vector3[0];
    public Vector3[] handlesA = new Vector3[0]; // handle before point
    public Vector3[] handlesB = new Vector3[0]; // hanldesA[0]  hanldesB[max] are unsued
   
    public BezierComplex() {
    }
 
    public Vector3[] GetPointAtDistance(float dist=1f, Transform m_Transform=null) {
        List<Vector3> m_Position = new List<Vector3>();

		dist/=nodes.Length;
		
		float m_Time=0.0f;
		float m_End=0.0f;
		float m_Step = 0f;		
        while (m_Time <= 1.0f) {		
			m_Step=1f/(this.GetSegmentLengthAtTime (m_Time)/dist);

			m_End=m_Time;
			m_Time+=m_Step;			
		}
		
		float m_Frac = (1f-m_End)*dist;
		m_Step = 0f;
		m_Time=0.0f;
		float m_Time2=0.0f;
		int m_OverShot=0;
        while (m_Time <= 1.0f) {
			if (m_Time2>1f)
				if (m_OverShot<1)
					m_OverShot++;
				else
					m_Time=1f;
				
			Vector3 m_LocalPosition = this.GetPointAtTime (m_Time2);
			
			if (m_Transform)
			    m_Position.Add (m_Transform.TransformPoint(m_LocalPosition));
			else
				m_Position.Add (m_LocalPosition);
			
			m_Step=1f/(this.GetSegmentLengthAtTime (m_Time)/dist);
			
			m_Time2+=m_Step+(m_Step*m_Frac/dist);
			m_Time+=m_Step;
		}
		
		//Debug.Log (m_Time2-(m_Step+m_Frac));
		
		return m_Position.ToArray();
    }
	
    // t = 0 =---= 1;
    public Vector3[] GetTangentAtDistance(float dist=1f, Transform m_Transform=null) {
        List<Vector3> m_Tangent = new List<Vector3>();
		
		dist/=nodes.Length;
		
		float m_Time=0.0f;
		float m_End=0.0f;
		
        while (m_Time <= 1.0f) {		
			float m_Step=1f/(this.GetSegmentLengthAtTime (m_Time)/dist);

			m_End=m_Time;
			m_Time+=m_Step;		
		}
		
		float m_Frac = (1f-m_End)*dist;
		
		m_Time=0.0f;
		float m_Time2=0.0f;		
		int m_OverShot=0;
        while (m_Time <= 1.0f) {
			if (m_Time2>1f)
				if (m_OverShot<1)
					m_OverShot++;
				else
					m_Time=1f;			
			m_Tangent.Add (this.GetTangentAtTime (m_Time2));
			
			float m_Step=1f/(this.GetSegmentLengthAtTime (m_Time)/dist);
			
			m_Time2+=m_Step+(m_Step*m_Frac/dist);
			m_Time+=m_Step;
		}
			
		return m_Tangent.ToArray();
    }
 
      // t = 0 =---= 1;
    public Vector3[] GetNormalAtDistance(float dist=1f) {
        List<Vector3> m_Normal = new List<Vector3>();

		float fTime=0.0f;
		
        while (fTime <= 1.0f) {
			Vector3 m_Tangent0 = this.GetTangentAtTime (fTime);
			Vector3 m_Tangent1 = this.GetTangentAtTime (fTime+0.001f);			
			m_Normal.Add (Vector3.Cross (m_Tangent1.normalized, m_Tangent0.normalized).normalized);
			
			fTime+=0.1f/(this.GetSegmentLengthAtTime (fTime)/dist);
		}	
		
		return m_Normal.ToArray();
    }
	
    public float GetSegmentLengthAtTime(float t, int m_Interp=1000) {
        //if (t < 0 || t > 1) return Vector3.zero;
		t = Mathf.Clamp (t, 0f , 1f);
        if (nodes.Length < 2) return 1f;
       
        int seg = GetSegmentAtTime(ref t); // the function will also modify t (ref)
        //if (t == 0) return 1f;
	   
	    float m_TotalLength = 0f;
	    Vector3 m_Pos = nodes[seg];
		
	    for (int i = 0; i <= m_Interp; i++) {			
			Vector3 m_TempPos;
			
			if (seg+1>=nodes.Length) 
				m_TempPos = nodes[seg];				
			else
				m_TempPos = CalculatePosition(i/(m_Interp*1.0f), nodes[seg], handlesB[seg], handlesA[seg+1], nodes[seg+1]);
			
			m_TotalLength += Vector3.Distance(m_Pos, m_TempPos);
			m_Pos=m_TempPos;
		}
		
	    return m_TotalLength;
    } 
 
    // t = 0 =---= 1;
    public Vector3 GetPointAtTime(float t) {
        //if (t < 0 || t > 1) return Vector3.zero;
		t = Mathf.Clamp (t, 0f , 1f);
        if (nodes.Length < 2) return Vector3.zero;
       
        int seg = GetSegmentAtTime(ref t); // the function will also modify t (ref)
        if (t == 0) return nodes[seg];
	   
        return CalculatePosition(t, nodes[seg], handlesB[seg], handlesA[seg+1], nodes[seg+1]);
        //return CalculatePosition(t, nodes[seg], A[seg], B[seg], C[seg]);
    }

    public int GetSegmentAtTime(ref float t) { // get segment and retrieve the delta value to t
        int nbSegment = nodes.Length - 1;
        float tRect = t * nbSegment;
        int seg = (int) tRect; // cast to int to get segment
        tRect -= seg; // 0-1 for that segment
        t = tRect;
        return seg;
    }
 
    public int GetSegmentAtTime(float t) { // get segment and retrieve the delta value to t
        int nbSegment = nodes.Length - 1;
        float tRect = t * nbSegment;
        return (int) tRect;
    }
 
    private Vector3 CalculatePosition (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
		//Vector3 v1 = -p0 + p1 - p2 + p3;
		//Vector3 v2 = p0 - p1 + p2;
		//Vector3 v3 = -p0 + p1;
		
		//t = t + L / (t * t * v1 + t * v2 + v3).magnitude;
		//t = t + (t * t * v1 + t * v2 + v3).magnitude;
		
        float u = 1.0f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        Vector3 p = uuu * p0; //first term
        p += 3 * uu * t * p1; //second term
        p += 3 * u * tt * p2; //third term
        p += ttt * p3; //fourth term
        return p;
    }
       
        /*
        private Vector3 CalculatePosition(float t, Vector3 p0, Vector3 A, Vector3 B, Vector3 C) {
            float t2 = t * t;
            float t3 = t2 * t;
            float x = A.x * t3 + B.x * t2 + C.x * t + p0.x;
            float y = A.y * t3 + B.y * t2 + C.y * t + p0.y;
            float z = A.z * t3 + B.z * t2 + C.z * t + p0.z;
            return new Vector3(x, y, z);
        }//*/
 
    public Vector3 GetTangentAtTime(float t) {
        //if (t < 0 || t > 1) return Vector3.zero;
		t = Mathf.Clamp (t, 0f , 1f);		
        if (nodes.Length < 2) return Vector3.zero;
        int seg = GetSegmentAtTime(ref t); // the function will also modify t (ref)
        if (t == 0)
        {
            if (seg == nodes.Length-1) return nodes[seg] - handlesA[seg];
            return (handlesB[seg] - nodes[seg]).normalized;
        }
       
        return CalculateTangent(t, nodes[seg], handlesB[seg], handlesA[seg+1], nodes[seg+1]).normalized;
    }
   
        private Vector3 CalculateTangent(float t, Vector3 p0, Vector3 h0, Vector3 h1, Vector3 p1) {
           
            float a = 1-t;
            float b = a*6*t;
            a = a*a*3;
            float c = t*t*3;
           
            return     - a * p0  // derivative formula from GetBezierPoint formula
                    + a * h0
                    - b * h0
                    - c * h1
                    + b * h1
                    + c * p1;
            //     - 3(1-t)^2 *P0
            //    + 3(1-t)^2 *P1
            //    - 6t(1-t) * P1
            //    - 3t^2 *     P2
            //    + 6t(1-t) * P2
            //    + 3t^2 *     P3
        }
   
    /*
    // If all bezier is instantaneously created through function
    public void SetAllConstants() {
        for (int i = 0; i < A.Length; i++) SetConstant(i);
    }
   
    private void SetConstant(int i) { // i base node of bezier
        if (i >= A.Length) return; // A.Length = nodes.Length - 1
       
        CalculateConstant(i, nodes[i], handlesB[i], nodes[i+1], handlesA[i+1]);
    }
   
            private void CalculateConstant(int i, Vector3 p0, Vector3 h0, Vector3 p1, Vector3 h1) {
               
                C[i].x = 3f * ( ( p0.x + h0.x ) - p0.x );
                B[i].x = 3f * ( ( p1.x + h1.x ) - ( p0.x + h0.x ) ) - C[i].x;
                A[i].x = p1.x - p0.x - C[i].x - B[i].x;
               
                C[i].y = 3f * ( ( p0.y + h0.y ) - p0.y );
                B[i].y = 3f * ( ( p1.y + h1.y ) - ( p0.y + h0.y ) ) - C[i].y;
                A[i].y = p1.y - p0.y - C[i].y - B[i].y;
               
                C[i].z = 3f * ( ( p0.z + h0.z ) - p0.z );
                B[i].z = 3f * ( ( p1.z + h1.z ) - ( p0.z + h0.z ) ) - C[i].z;
                A[i].z = p1.z - p0.z - C[i].z - B[i].z;
            }
    //*/
   
   
    public Vector3[,] GetBezierInHandlesFormat() {
        // format : (start, end, handleStart, handleEnd)
        int l = GetNumberOfSegment();
        if (l == 0) return null;
       
        Vector3[,] b = new Vector3[l, 4];
        for (int i = 0; i < l; i++)
        {
            b[i, 0] = nodes[i];
            b[i, 1] = nodes[i + 1];
            b[i, 2] = handlesB[i];
            b[i, 3] = handlesA[i + 1];
        }
        return b;
       
    }
   
    /*    Use example
            Vector3[,] h = thisBezier.GetBezierInHandlesFormat(); //Vector3[segments, 4(p0,h0,p1,h1)];
            for (int i = 0; i < l; i++)
            {
                Handles.DrawBezier(h[i,0], h[i,1], h[i,2], h[i,3], Color.green, null, 2);
            }
    //*/
   
   
    public void DeleteAllNodes() {
           
        nodes = new Vector3[0];
        handlesA = new Vector3[0];
        handlesB = new Vector3[0];
       
        //A = new Vector3[0];
        //B = new Vector3[0];
        //C = new Vector3[0];
       
    }
   
    public void DeleteNode(int i) {
       
         if (i < 0 || i >= nodes.Length) Debug.LogError("You are  trying to DELETE a node which does not exist. Max node index : " +  (nodes.Length - 1) + " . Index wanted : " + i);
       
        ArrayList n = new ArrayList(nodes);
        ArrayList ha = new ArrayList(handlesA);
        ArrayList hb = new ArrayList(handlesB);
        //ArrayList a = new ArrayList(A);
        //ArrayList b = new ArrayList(B);
        //ArrayList c = new ArrayList(C);
       
        n.RemoveAt(i);
        ha.RemoveAt(i);
        hb.RemoveAt(i);
        //a.RemoveAt(i);
        //b.RemoveAt(i);
        //c.RemoveAt(i);
       
        nodes =     (Vector3 []) n.ToArray(typeof(Vector3));
        handlesA =     (Vector3 []) ha.ToArray(typeof(Vector3));
        handlesB =     (Vector3 []) hb.ToArray(typeof(Vector3));
        //A =         (Vector3 []) a.ToArray(typeof(Vector3));
        //B =         (Vector3 []) b.ToArray(typeof(Vector3));
        //C =         (Vector3 []) c.ToArray(typeof(Vector3));
    }
   
   
    public void InsertNode(int i, Vector3 p, Vector3 h1, Vector3 h2) {
       
         if (i < 0 || i > nodes.Length) Debug.LogError("You are  trying to delete a node which does not exist. Max node index (with added  node) : " + nodes.Length + " . Index wanted : " + i);
       
        /*
        string str = "";
        for (int k = 0; k < nodes.Length; k++)
        {
            str += System.Math.Round(nodes[k].x, 0) + ", ";
        }
        Debug.Log(str);
        //*/
       
        ArrayList n = new ArrayList(nodes);
        ArrayList ha = new ArrayList(handlesA);
        ArrayList hb = new ArrayList(handlesB);
        //ArrayList a = new ArrayList(A);
        //ArrayList b = new ArrayList(B);
        //ArrayList c = new ArrayList(C);
       
        if (i == nodes.Length)
        {
            n.Add(p);
            ha.Add(h1);
            hb.Add(h2);
            //a.Add(Vector3.zero);
            //b.Add(Vector3.zero);
            //c.Add(Vector3.zero);
        }
        else
        {
            n.Insert(i, p);
            ha.Insert(i, h1);
            hb.Insert(i, h2);
            //a.Insert(i, Vector3.zero);
            //b.Insert(i, Vector3.zero);
            //c.Insert(i, Vector3.zero);
        }
       
        nodes =     (Vector3 []) n.ToArray(typeof(Vector3));
        handlesA =     (Vector3 []) ha.ToArray(typeof(Vector3));
        handlesB =     (Vector3 []) hb.ToArray(typeof(Vector3));
        //A =         (Vector3 []) a.ToArray(typeof(Vector3));
        //B =         (Vector3 []) b.ToArray(typeof(Vector3));
        //C =         (Vector3 []) c.ToArray(typeof(Vector3));
       
        //if (i != nodes.Length - 1) SetConstant(i);
        //if (i != 0) SetConstant(i - 1);
       
        /*
        for (int k = 0; k < nodes.Length; k++)
        {
            str += System.Math.Round(nodes[k].x, 2) + ", ";
        }
        Debug.Log(str);
        //*/
    }
   
   
    public void MoveNode(Vector3 p, int i) {
         if (i < 0 || i >= nodes.Length) Debug.LogError("You are  trying to MOVE a node which does not exist. Max node index : " +  (nodes.Length - 1) + " . Index wanted : " + i);
        var delta = p - nodes[i];
        handlesA[i] += delta;
        handlesB[i] += delta;
        nodes[i] = p;
       
        //if (i != nodes.Length - 1) SetConstant(i);
        //if (i != 0) SetConstant(i - 1);
    }
   
    public void MoveHandle(Vector3 p, int i, bool handleA) {
         if (i < 0 || i >= nodes.Length) Debug.LogError("You are  trying to MOVE an handle which does not exist. Max node index : " +  (nodes.Length - 1) + " . Index wanted : " + i);
        if (handleA)    handlesA[i] = p;
        else             handlesB[i] = p;
       
        //if (i != nodes.Length - 1) SetConstant(i);
        //if (i != 0) SetConstant(i - 1);
    }
   
   
    public int GetNumberOfSegment() {
        int n = nodes.Length - 1;
        if (n < 0) n = 0;
        return n;
    }
   
}