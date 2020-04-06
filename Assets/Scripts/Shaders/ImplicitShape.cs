using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplicitShape : MonoBehaviour
{
    public ShapeData data;

    public ImplicitShape()// also make another constructor
    {
        data.size = 1;
        data.pos = new Vector4(0, 0, 0, 0);
    }

    public ImplicitShape(Vector4 worldPos, float size, Vector4 rotation)// also make another constructor
    {
        data.size = 1;
        data.pos = new Vector4(0, 0, 0, 0);

    }

    float SDF()//todo
    {
        return data.pos.magnitude * data.size - data.size;

    }

    public void Transform(Matrix4x4 t)
    {
        data.pos = t.inverse * data.pos;
    }

    public Matrix4x4 Translate(Vector4 t)
    {
        return new Matrix4x4(new Vector4(1, 0, 0, t.x), 
                                    new Vector4(0, 1, 0, t.y), 
                                    new Vector4(0, 0, 1, t.z), 
                                    new Vector4(0, 0, 0, 1  ));
       
        
    }

    Matrix4x4 RotX(float angle)
    {
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);
        return new Matrix4x4(new Vector4(1, 0, 0, 0),
                             new Vector4(0, c, -s, 0),
                             new Vector4(0, s, c, 0),
                             new Vector4(0, 0, 0, 0));
    }
    Matrix4x4 RotY(float angle)
    {
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);
        return new Matrix4x4(new Vector4(c, 0, s, 0),
                             new Vector4(0, 1, 0, 0),
                             new Vector4(-s, 0, c, 0),
                             new Vector4(0, 0, 0, 0));
    }
    Matrix4x4 RotZ(float angle)
    {
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);
        return new Matrix4x4(new Vector4(c, -s, 0, 0),
                             new Vector4(s, c, 0, 0),
                             new Vector4(0, 0, 1, 0),
                             new Vector4(0, 0, 0, 0));
    }
    public struct ShapeData {

        public Vector4 pos;
        public float size;
    
    }


}
