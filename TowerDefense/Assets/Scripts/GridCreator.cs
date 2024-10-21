using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private int gridWidth = 10;     
    [SerializeField] private int gridHeight = 10;   
    [SerializeField] private float cellSize = 1f;    
    [SerializeField] private GameObject cellPrefab;  
    [SerializeField] private float yOffset = 0.1f;   
    [SerializeField] private float rotationAngle = -45f; 

    void Start()
    {
        
        transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
        CreateGrid();
    }

    
    void CreateGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Cell Prefab is not assigned!");
            return;
        }

       
        Vector3 offset = transform.position;

        
        Quaternion rotation = transform.rotation;

        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
               
                Vector3 cellLocalPosition = new Vector3(x * cellSize, yOffset, y * cellSize);

                
                Vector3 rotatedPosition = rotation * cellLocalPosition;

                
                Vector3 cellPosition = offset + rotatedPosition;

                
                GameObject cellInstance = Instantiate(cellPrefab, cellPosition, rotation);

               
                cellInstance.transform.SetParent(transform);
            }
        }
    }
}
