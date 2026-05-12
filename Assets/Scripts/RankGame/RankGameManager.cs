using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class RankGameManager : MonoBehaviour
{
    public int gridWidth = 7;                 //가로 칸 수
    public int gridHeight = 7;                //세로 칸 수
    public float CellSize = 1.3f;             //각 칸의 크기
    public GameObject cellPrefabs;            //빈칸 프리팹
    public Transform gridContainer;           //그리드를 다음 부모 오브젝트

    public GameObject rankPrefabs;            //계급장 프리팹
    public Sprite[] rankSprites;              //각 레벨별 계급장 이미지
    public int maxRankLevel = 7;              //최대 계급장 레벨

    public GridCell[,] grid;                  //모든 칸을 저장하는 2차원 배열


    void InitializeGrid()
    {
        grid = new GridCell[gridWidth, gridHeight];

        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(
                    x * CellSize - (gridWidth * CellSize / 2) + CellSize / 2,
                    y * CellSize - (gridWidth * CellSize / 2) + CellSize / 2,
                    1f
                );  
                
                GameObject cellObj = Instantiate(cellPrefabs, position, Quaternion.identity, gridContainer);
                GridCell cell = cellObj.AddComponent<GridCell>();
                cell.Initialize(x, y);

                grid[x, y] = cell;

            }

        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeGrid();

        for(int i = 0; i < 4; i++)
        {
            SpawnNewRank();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            SpawnNewRank();
        }
    }

    public DraggableRank CreateRankInCell(GridCell cell, int level)
    {
        if (cell == null || !cell.isEmpty()) return null;

        level = Mathf.Clamp(level, 1, maxRankLevel);

        Vector3 rankPosition = new Vector3(cell.transform.position.x, cell.transform.position.y, 0f);


        GameObject rankObj = Instantiate(rankPrefabs, rankPosition, Quaternion.identity, gridContainer);
        rankObj.name = "Rank_Level_" + level;

        DraggableRank rank = rankObj.AddComponent<DraggableRank>();

        rank.SetRankLevel(level);

        cell.SetRank(rank);

        return rank;
    }

    private GridCell FineEmptyCell()
    {
        List<GridCell> emptyCells = new List<GridCell>();

        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].isEmpty())
                {
                    emptyCells.Add(grid[x, y]);
                }
            }
        }

        if(emptyCells.Count == 0)
        {
            return null;
        }

        return emptyCells[Random.Range(0, emptyCells.Count)];
    }

    public bool SpawnNewRank()
    {
        GridCell emptyCell = FineEmptyCell();
        if (emptyCell == null) return false;

        int rankLevel = Random.Range(0, 100) < 80 ? 1 : 2;  //80% 확률로 레벨 1, 20% 확률로 레벨 2

        CreateRankInCell(emptyCell, rankLevel);

        return true;
    }

    public GridCell FindClosesteCell(Vector3 position)      //가장 가까운 칸 찾기
    {
        for(int x = 0; x <gridWidth; x++)                   //1.먼저 위치가 포함되 ㄴ칸 확인
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].ContainsPosition(position))
                {
                    return grid[x, y];
                }
            }
        }

        GridCell clossestCell = null;
        float closestDistance = float.MaxValue;

        for (int x = 0; x < gridWidth; x++)                   
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float distance = Vector3.Distance(position, grid[x, y].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    clossestCell = grid[x, y];
                }
            }
        }

        if(closestDistance > CellSize * 2)
        {
            return null;
        }
        return clossestCell;
    }

    public void RemoveRank(DraggableRank rank)
    {
        if (rank == null) return;

        if(rank.currentCell != null)
        {
            rank.currentCell.currentRank = null;
        }

        Destroy(rank.gameObject);
    }

    public void MergeRanks(DraggableRank draggableRank, DraggableRank targetRank)
    {
        if(draggableRank == null || targetRank == null || draggableRank.rankLevel != targetRank.rankLevel)
        {
            if (draggableRank != null) draggableRank.ReturnToOriginalPosition();
            return;
        }

        int newLevel = targetRank.rankLevel + 1;
        if(newLevel > maxRankLevel)
        {
            RemoveRank(draggableRank);
            return;
        }

        targetRank.SetRankLevel(newLevel);
        RemoveRank(draggableRank);

    }
}