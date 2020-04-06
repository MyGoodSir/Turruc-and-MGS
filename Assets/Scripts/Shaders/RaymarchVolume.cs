using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaymarchVolume : MonoBehaviour
{
    //add list of Vector4 positions from child objects to this. add a compute buffer that holds an array of the corresponding SDFs for each indexed vector's original shape.
    public MaterialPropertyBlock mpb = new MaterialPropertyBlock();

    public List<ImplicitShape> shapeList;
    public List<ImplicitShape.ShapeData> shapeDataList;
    ComputeBuffer shapeBuffer;
    void Start()
    {


        //this.gameObject.GetComponent<MeshRenderer>().SetPropertyBlock();
    }
    void Update()
    {
        
    }
}
