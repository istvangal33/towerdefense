using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Color notEnoughMoneyColor;

    public GameObject turret; 
    public Vector3 positionOffSet; 

    private Renderer rend; 
    private Color startColor;

    BuildManager buildManager;

    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        buildManager = BuildManager.instance;
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffSet;
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return; 

        
        if (turret != null)
        {
            return;
        }

        if (buildManager.CanBuild)
        {
            buildManager.BuildTurretOn(this); 
        }
    }

    void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return; 

        if (!buildManager.CanBuild)
            return;  

        if (buildManager.HasMoney)
        {
            rend.material.color = hoverColor;  
        }
        else
        {
            rend.material.color = notEnoughMoneyColor; 
        }
    }

    void OnMouseExit()
    {
        rend.material.color = startColor;
    }
}
