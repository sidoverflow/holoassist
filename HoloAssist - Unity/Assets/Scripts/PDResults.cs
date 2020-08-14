using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PDResults
{
    public List<Face> faces;
    public string requestId;
    public MetaData metadata;
}
[System.Serializable]
public class FaceRectangle
{
    public int height;
    public int left;
    public int top;
    public int width;
}

[System.Serializable]
public class Face
{
    public int age;
    public string gender;
    public FaceRectangle faceRectangle;
    
}
