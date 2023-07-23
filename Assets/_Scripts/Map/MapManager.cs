using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Cards.Card_UI;
using Shun_Card_System;
using Shun_Grid_System;
using UnityEngine;
using UnityUtilities;

public class MapManager : SingletonMonoBehaviour<MapManager>
{
    [field: Header("Grid")]
    public GridXY<MapCellItem> WorldGrid { get; private set;}
    public int GridWidth, GridHeight;
    public float WidthSize, HeightSize;

    
    public class CharacterSet
    {
        public CharacterInformation CharacterInformation;
        public BaseCharacterMapDynamicGameObject CharacterMapGameObject;
        public BaseCharacterCardGameObject CharacterCardGameObject;

        public CharacterSet(CharacterInformation characterInformation, BaseCharacterMapDynamicGameObject characterMapGameObject, BaseCharacterCardGameObject characterCardGameObject)
        {
            CharacterInformation = characterInformation;
            CharacterMapGameObject = characterMapGameObject;
            CharacterCardGameObject = characterCardGameObject;
        }
    }

    [Header("Card Region")] 
    [SerializeField] private HandCardRegion _handCardRegion;
    [SerializeField] private PlayCardRegion _playCardRegion;
    
    [Header("Entities")]
    public CharacterInformation[] CharacterInformation;
    public VentMapGameObject[] VentMapGameObjects;
    public ExitMapGameObject[] ExitMapGameObjects;
    public Transform[] SpawnPointsInLight;
    public Transform[] SpawnPointsInDark;
    
    [Header("Sets")]
    private Dictionary<CharacterInformation, CharacterSet> _characterSets = new();
    private RandomBag<Transform> _spawnPointBags;


    [Header("Adjacency Cell")]
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
        WorldGrid = new GridXY<MapCellItem>(GridWidth, GridHeight, WidthSize, HeightSize , transform.position);

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
        InitializeCharacters();
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
                
                //cell.Item.MapCellGameObject = Instantiate(ResourceManager.Instance.TestItem, WorldGrid.GetWorldPositionOfNearestCell(x,y), Quaternion.identity, transform);
                cell.Item.CellSelectHighlighter = Instantiate(ResourceManager.Instance.CellSelectHighlighter, WorldGrid.GetWorldPositionOfNearestCell(x,y), Quaternion.identity, transform);
            }
        }
    }

    private void InitializeVent()
    {
        var distanceCost = new ManhattanDistanceCost();


        /*
        foreach (var vent in VentMapGameObjects)
        {
            vent.InitializeGrid();   
        }
        */

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
    
    
    private void InitializeCharacters()
    {
        foreach (var characterInformation in CharacterInformation)
        {
            var spawnPoint = GetRandomSpawnPoint();
            var characterMap = Instantiate(characterInformation.CharacterMapDynamicGameObjectPrefab, spawnPoint.position, spawnPoint.rotation, transform);
            var characterCard = Instantiate(characterInformation.BaseCharacterCardGameObjectPrefab);
            
            CharacterSet set = new CharacterSet(characterInformation, characterMap, characterCard);
            _characterSets[characterInformation] = set;
            _handCardRegion.AddCard(characterCard, null);
            
            characterMap.InitializeCharacter(characterInformation,characterCard);
            characterCard.InitializeCharacter(characterInformation,characterMap);
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        return _spawnPointBags.PopRandomItem();
    }
    
    
}
