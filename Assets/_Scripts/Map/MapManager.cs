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

    [Serializable]
    public class CharacterSet
    {
        public CharacterInformation CharacterInformation;
        public BaseCharacterMapGameObject MapGameObject;
        public BaseCharacterCardGameObject CardGameObject;
    }
    
    [Header("Entities")]
    public CharacterSet[] CharacterSets;
    public VentMapGameObject[] VentMapGameObjects;
    
    [Header("Adjacency Cell")]
    [HideInInspector] public Vector2Int[] AdjacencyDirections = new[]
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
    }

    void InitializeGrid()
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

    
    private void Start()
    {
        InitializeCellAdjacency();
        InitializeCellItem();
        InitializeCellCharacter();
        InitializeVent();
    }

    private void InitializeCellAdjacency()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                var cell = WorldGrid.GetCell(x, y);
                
                foreach (var direction in AdjacencyDirections)
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

    
    private void InitializeCellCharacter()
    {
        foreach (var characterSet in CharacterSets)
        {
            characterSet.MapGameObject.InitializeCharacter(characterSet.CharacterInformation,characterSet.CardGameObject);
            characterSet.CardGameObject.InitializeCharacter(characterSet.CharacterInformation,characterSet.MapGameObject);
        }
    }
    
    
}
