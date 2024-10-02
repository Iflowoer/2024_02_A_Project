using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GridCell 클래스는 각 그리드 셀의 상태와 데이터를 관리

public class GridCell
{
    public Vector3Int Position;                 //셀의 그리드 내 위치
    public bool IsOccupied;                     //셀이 건물로 차있는지 여부
    public GameObject Building;                 //셀에 배치된 건물 객체

    public GridCell(Vector3Int position)      //클래스 이름과 동일한 함수 생성자 클래스가 생성될때 호출
    {
        Position = position;
        IsOccupied = false;
        Building = null;
    }
}

public class GridBuildingSystem : MonoBehaviour
{

    [SerializeField] private int width = 10;            //그리드 가로 크기
    [SerializeField] private int height = 10;           //그리드 세로 크기
    [SerializeField] private float cellSize = 1.0f;             //각 셀의 크기
    [SerializeField] private GameObject cellPrefabs;    //셀 프리팹
    [SerializeField] private GameObject buildingPrefabs;    //빌딩 프리팹


    [SerializeField] private PlayerController playerController;     //플레이어 컨트롤러 참조

    [SerializeField] private Grid grid;
    private GridCell[,] cells;                          //GridCell 클래스를 2차원 배열로 설정
    private Camera firstPersonCamera;


    // Start is called before the first frame update
    void Start()
    {
        firstPersonCamera = playerController.firstPersonCamera;
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookPosition = GetLookPosition();
        if (lookPosition != Vector3.zero)
        {
            Vector3Int gridPosition = grid.WorldToCell(lookPosition);
            if (isValidGridPosition(gridPosition))
            {
                HighlightCell(gridPosition);
                if(Input.GetMouseButton(0))
                {
                    PlaceBuilding(gridPosition);
                }
                if(Input.GetMouseButton(1))
                {
                    RemoveBuilding(gridPosition);
                }
            }
        }
    }

    //그리드 셀에 건물을 배치하는 메서드
    private void PlaceBuilding(Vector3Int gridPosition)
    {
        GridCell cell = cells[gridPosition.x, gridPosition.z];          //위치 기반으로 cell을 받아온다
        if(!cell.IsOccupied)                                            //해당 위치에 건물이 있는지 확인한다.
        {
            Vector3 worldPosition = grid.GetCellCenterWorld(gridPosition);      //월드 위치 변환 값
            GameObject building = Instantiate(buildingPrefabs, worldPosition, Quaternion.identity);     //건물을 생성
            cell.IsOccupied = true;                                     //건물 확인 값
            cell.Building = building;                                   //cell에 놓인 빌딩
        }
    }

    //그리드 셀에서 건물을 제거하는 메서드
    private void RemoveBuilding(Vector3Int gridPosition)
    {
        GridCell cell = cells[gridPosition.x, gridPosition.z];      //위치 기반으로 cell을 받아온다
        if (cell.IsOccupied)                                        //해당 위치에 건물이 있는지 없는지 확인한다
        {
            Destroy(cell.Building);                                 //cell 건물을 제거
            cell.IsOccupied = false;                                //건물 확인 값
            cell.Building = null;                                   //
        }
    }

    //선택된 셀을 하이라이트하는 메스
    private void HighlightCell(Vector3Int gridPosition)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GameObject cellObject = cells[x, z].Building != null ? cells[x, z].Building : transform.GetChild(x * height + z).gameObject;
                cellObject.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        GridCell cell = cells[gridPosition.x, gridPosition.z];
        GameObject hightlightObject = cell.Building != null ? cell.Building : transform.GetChild(gridPosition.x * height + gridPosition.z).gameObject;
        hightlightObject.GetComponent<Renderer>().material.color = cell.IsOccupied ? Color.red : Color.green;

    }

    //그리드 포지션이 유효한지 확인하는 메서드
    private bool isValidGridPosition(Vector3Int gridPosition)
    {
        return gridPosition.x>=0&&gridPosition.x<width&&
            gridPosition.z>=0&&gridPosition.z<height;
    }

    //그리드를 생성하고 셀을 초기화하는 메서드
    private void CreateGrid()
    {
        grid.cellSize = new Vector3(cellSize, cellSize, cellSize);

        cells = new GridCell[width, height];
        Vector3 gridCenter = playerController.transform.position;
        gridCenter.y = 0;
        transform.position = gridCenter - new Vector3(width * cellSize / 2.0f, 0, height * cellSize / 2.0f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3Int cellPosition = new Vector3Int(x, 0, z);
                Vector3 worldPosition = grid.GetCellCenterWorld(cellPosition);
                GameObject cellObject = Instantiate(cellPrefabs, worldPosition, cellPrefabs.transform.rotation);
                cellObject.transform.SetParent(transform);

                cells[x, z] = new GridCell(cellPosition);
            }
        }
    }

    //플레이어가 보고 있는 위치를 계산하는 메서드

    private Vector3 GetLookPosition()
    {
        if(playerController.isFirstPerson)
        {
            Ray ray = new Ray(firstPersonCamera.transform.position, firstPersonCamera.transform.forward);
            if(Physics.Raycast(ray, out RaycastHit hitInfo, 5.0f))
            {
                Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color. red);
                return hitInfo.point;
            }
            else
            {
                Debug.DrawLine(ray.origin, ray.direction * 5.0f, Color.white);
            }
        }
        //3인칭 모드
        else
        {
            Vector3 characterPosition = playerController.transform.position;
            Vector3 characterFoward = playerController.transform.forward;
            Vector3 rayOrigin = characterPosition + Vector3.up * 1.5f + characterFoward * 0.5f;
            Vector3 rayDirection = (characterFoward - Vector3.up).normalized;

            Ray ray = new Ray(rayOrigin, rayDirection);

            if(Physics.Raycast(ray,out RaycastHit hitInfo,5.0f))
            {
                Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.blue);
                return hitInfo.point;
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * 5.0f, Color.white);
            }
        }

        return Vector3.zero;
    }
    


    //그리드 셀을 Gizmo 로 표기하는 메서드

    private void OnDrawGizmos()         //유니티 Scene 창에 보이는 debug 그림
    {
        Gizmos.color = Color.blue;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 cellCenter = grid.GetCellCenterWorld(new Vector3Int(x, 0, z));
                Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, 0.1f, cellSize));
            }
        }
    }

}
