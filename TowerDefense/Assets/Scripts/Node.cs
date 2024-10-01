using UnityEngine;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Vector3 positionOffset;

    private Renderer render;


    private GameObject turret;



    private Color startColor;

    void Start()
    {
        render = GetComponent<Renderer>();
        startColor = render.material.color;
    }

    void OnMouseDown ()
    {
        if (turret != null) 
        {
            Debug.Log("Nem lehetséges odaépíteni");
            return;

        }

        GameObject turretToBuild = BuildManager.instance.GetTurretToBuild();
        turret = (GameObject)Instantiate(turretToBuild, transform.position + positionOffset, transform.rotation);
    }
    
    void OnMouseEnter()
    {
        render.material.color = hoverColor;
    }

    void OnMouseExit ()
    {
        render.material.color = startColor;
    }
}
