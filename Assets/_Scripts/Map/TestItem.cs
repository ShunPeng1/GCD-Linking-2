
using UnityEngine;
using static UnityEngine.Physics2D;

public class TestItem : BaseGridXYItemGameObject
{
    [SerializeField] private float _length;

    [SerializeField] private Vector2[] _directions = new[]
    {
        new Vector2(0, 1).normalized,
        new Vector2(1, 1).normalized,
        new Vector2(1, 0).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(0, -1).normalized,
        new Vector2(-1, -1).normalized,
        new Vector2(-1, 0).normalized,
        new Vector2(-1, 1).normalized,
    };

    [SerializeField] private LayerMask _wallLayerMask;
        
    protected override void Start()
    {
        base.Start();

        Test();
    }
    
    private void Test()
    {
        foreach (var direction in _directions)
        {
            var hit = Raycast(transform.position, direction, 3, _wallLayerMask);
            if (hit.transform != null)
            {
                Instantiate(ResourceManager.Instance.TestSquare, transform.position + (Vector3) direction, Quaternion.identity, transform);
            }
        }
    }
    
    
}
