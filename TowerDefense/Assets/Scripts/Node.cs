using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Vector3 positionOffset;

    private Renderer render;


    private GameObject turret;



    private Color startColor;

    BuildManager buildManager;

    void Start()
    {
        render = GetComponent<Renderer>();
        startColor = render.material.color;

        buildManager = BuildManager.instance;
    }

    void OnMouseDown ()
    {
        if(buildManager.GetTurretToBuild() == null) 
        {
            return;
        }

        if (turret != null) 
        {
            Debug.Log("Nem lehetséges odaépíteni");
            return;

        }

        GameObject turretToBuild = buildManager.GetTurretToBuild();
        turret = (GameObject)Instantiate(turretToBuild, transform.position + positionOffset, transform.rotation);
    }
    
    void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (buildManager.GetTurretToBuild() == null)
        {
            return;
        }
            render.material.color = hoverColor;
    }

    void OnMouseExit ()
    {
        render.material.color = startColor;
    }
}
