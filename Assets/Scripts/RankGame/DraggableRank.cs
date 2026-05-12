using UnityEngine;

public class DraggableRank : MonoBehaviour
{

    public int rankLevel = 1;                   //계급장 레벨 (0은 빈칸)
    public float dragSpeed = 30f;               //드래그 시 오브젝트 이동 속도
    public float snapBackSpeed = 20f;           //원 위치로 돌아가는 속도

    public bool isDragging = false;             //현재 드래그 중인지 확인하는 변수

    public Vector3 originalPosition;            //계급장의 원래 위치
    public GridCell currentCell;                //현재 위치한 칸

    public Camera mainCamera;                   //메인 카메라
    public Vector3 dragOffset;                  //드래그 시 오프셋(보정값)
    public SpriteRenderer spriteRenderer;       //계급 이미지 렌더러

    public RankGameManager GameManager;         //게임 매니저

    private void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameManager = FindAnyObjectByType<RankGameManager>();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDragging)
        {
            Vector3 targetPosition = GetMouseWorldPosition() + dragOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, dragSpeed * Time.deltaTime);
        }
        else if(transform.position != originalPosition && currentCell != null)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, snapBackSpeed * Time.deltaTime);
        }
    }

    private void OnMouseDown()
    {
        StartDragging();
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        StopDragging();

    }

    void StartDragging()
    {
        isDragging = true;
        dragOffset = transform.position - GetMouseWorldPosition();   //마우스 계급장 위치 차이 계산
        spriteRenderer.sortingOrder = 0;                             //드래그 시작 시 계급장을 앞으로 보내기
    }

    void StopDragging()
    {
        isDragging = false;
        spriteRenderer.sortingOrder = 1;
        GridCell targetCell = GameManager.FindClosesteCell(transform.position);

        if(targetCell != null)
        {
            if(targetCell.currentRank == null)
            {
                MoveToCell(targetCell);
            }
            else if(targetCell.currentRank != this && targetCell.currentRank.rankLevel == rankLevel)
            {
                MergeWithCell(targetCell);
            }
            else
            {
                ReturnToOriginalPosition();
            }
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }


    public void MoveToCell(GridCell targetCell)             //특정 칸으로 이동
    {
        if(currentCell != null)                             //기존 칸에서 제거
        {
            currentCell.currentRank = null;                 //새로운 칸으로 이동
        }

        currentCell = targetCell;
        targetCell.currentRank = this;

        originalPosition = new Vector3(targetCell.transform.position.x, targetCell.transform.position.y, 0);
        transform.position = originalPosition;
    }

    public void ReturnToOriginalPosition()                 //기존 위치로 돌아가는 함수
    {
        transform.position = originalPosition;
    }

    public void MergeWithCell(GridCell targetCell)         //Merge 시도 함수
    {
        if(targetCell.currentRank == null || targetCell.currentRank.rankLevel != rankLevel)       //다른 레벨이거나 비어있다면
        {
            ReturnToOriginalPosition();                   //기존 위치로 돌아가기
            return;
        }

        if(currentCell != null)
        {
            currentCell.currentRank = null;               //기존 칸에서 제거
        }

        GameManager.MergeRanks(this, targetCell.currentRank);
    }


    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    public void SetRankLevel(int level)
    {
        rankLevel = level;

        if(GameManager != null && GameManager.rankSprites.Length > level - 1)
        {
            spriteRenderer.sprite = GameManager.rankSprites[level - 1];
        }
    }
}
