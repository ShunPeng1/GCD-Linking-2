using UnityEngine;

[CreateAssetMenu(fileName = "Light Information", menuName = "Light Information")]
public class LightInformation : ScriptableObject
{
    
    public float LightRange = 5f;
    public Color LightColor = Color.white;
    public float FieldOfView = 360f;
    public int RayCount = 90;
    public Vector3 StartDirection = Vector3.right;
    public LayerMask WallCastLayerMask;
    public LayerMask TargetCastLayerMask;
    

}
