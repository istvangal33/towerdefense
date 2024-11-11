using UnityEngine;
using UnityEngine.EventSystems;

public class HideNodeUIOnClick : MonoBehaviour
{
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            
            if (Physics.Raycast(ray, out hit))
            {
                Node clickedNode = hit.collider.GetComponent<Node>();
                Tower clickedTower = hit.collider.GetComponent<Tower>();

                if (clickedNode == null && clickedTower == null)
                {
                    BuildManager.instance.DeselectNode();
                }
            }
            else
            {
                
                BuildManager.instance.DeselectNode();
            }
        }
    }
}