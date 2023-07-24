using System;

using System.Collections.Generic;

using Shun_Grid_System;
using UnityEngine;
using UnityUtilities;
using Random = UnityEngine.Random;


public class MapManager : SingletonMonoBehaviour<MapManager>
{
    [field: Header("Grid")] 
    public Transform MapParent;
    public GridXY<MapCellItem> WorldGrid { get; private set;}
    public int GridWidth, GridHeight;
    public float WidthSize, HeightSize;


    [Header("Entities")] 
    public List<BaseCharacterMapDynamicGameObject> CharacterMapGameObjects;
    public List<CharacterLight> CharacterLights;
    public VentMapGameObject[] VentMapGameObjects;
    public ExitMapGameObject[] ExitMapGameObjects;
    public Transform[] SpawnPointsInLight;
    public Transform[] SpawnPointsInDark;
    
    [Header("Sets")]
    private RandomBag<Transform> _spawnPointBags;


    [Header("Adjacency Cell")] 
    [SerializeField] private LayerMask _lightCastLayerMask;
    [SerializeField] private Vector2Int[] _adjacencyDirections = new[]
    {
        new Vector2Int(0, 1),
        //new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        //new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        //new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        //new Vector2Int(-1, 1),
    };
    
    private void Awake()
    {
        InitializeGrid();
        InitializeBag();
        
    }

    private void InitializeGrid()
    {
        WorldGrid = new GridXY<MapCellItem>(GridWidth, GridHeight, WidthSize, HeightSize , MapParent.transform.position);

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                
                var cell = new GridXYCell<MapCellItem>(WorldGrid, x, y, null);
                cell.Item = new MapCellItem(WorldGrid,cell);

                WorldGrid.SetCell(cell,x,y);   
            }
        }
    }

    private void InitializeBag()
    {
        var spawnPoints = Shun_Utility.SetOperations.MergeArrays(SpawnPointsInLight, SpawnPointsInDark);
        _spawnPointBags = new RandomBag<Transform>(spawnPoints, 1);
    }
    
    private void Start()
    {
        InitializeCellAdjacency();
        InitializeCellItem();
        InitializeVent();
    }

    private void InitializeCellAdjacency()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                var cell = WorldGrid.GetCell(x, y);
                
                foreach (var direction in _adjacencyDirections)
                {
                    var adjacencyCell = WorldGrid.GetCell(x + direction.x, y + direction.y); 
                    if(adjacencyCell != null) cell.SetAdjacencyCell(adjacencyCell);
                }
            }
        }
    }
    
    
    private void InitializeCellItem()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                var cell = WorldGrid.GetCell(x, y);

                cell.Item.CellSelectHighlighter = Instantiate(ResourceManager.Instance.CellSelectHighlighter, WorldGrid.GetWorldPositionOfNearestCell(x,y), Quaternion.identity, MapParent.transform);
            }
        }
    }

    private void InitializeVent()
    {
        var distanceCost = new ManhattanDistanceCost();

        foreach (var vent1 in VentMapGameObjects)
        {
            
            foreach (var vent2 in VentMapGameObjects)
            {
                if (vent1 == vent2) continue;
                
                var difference = WorldGrid.GetIndexDifferenceAbsolute(vent1.Cell, vent2.Cell);
                double discountCost = distanceCost.GetDistanceCost(difference.x, difference.y);
                vent1.Cell.SetAdjacencyCell(vent2.Cell, 1 - discountCost );
            }
        }
    }
    

    public BaseCharacterMapDynamicGameObject CreateCharacterMapGameObject(BaseCharacterMapDynamicGameObject prefab)
    {
        var spawnPoint = GetRandomSpawnPoint();
        var character = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, MapParent.transform);
        
        CharacterMapGameObjects.Add(character);
        CharacterLights.Add(character.CharacterLight);

        character.name += " " + Random.Range(1,1000);
        
        return character;
    }

    private Transform GetRandomSpawnPoint()
    {
        return _spawnPointBags.PopRandomItem();
    }

    public List<BaseCharacterMapDynamicGameObject> SearchAllInDarkCharacter()
    {
        List<BaseCharacterMapDynamicGameObject> charactersInDark = new();
        foreach (var checkingCharacter in CharacterMapGameObjects)
        {
            bool isInDark = true;
            foreach (var castingCharacter in CharacterLights)
            {
                if (checkingCharacter.CharacterLight == castingCharacter) continue;

                if (!castingCharacter.TryCastToCharacter(checkingCharacter)) continue;
                
                isInDark = false;
                break;

            }

            if (isInDark)
            {
                charactersInDark.Add(checkingCharacter);
            }
        }

        return charactersInDark;
    }

}
