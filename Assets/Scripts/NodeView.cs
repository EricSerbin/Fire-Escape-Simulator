using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeView : MonoBehaviour
{
    public GameObject tile;
    [Range (0,0.5f)] 
    public float borderSize = 0.15f;
    

    public void Init(Node node)
    {
        if(tile!=null)
        {
            Node nodeTemp = node;
            BoxCollider tileCollider = gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
            tile.name = "Node(" + node.xIndex + ", " + node.yIndex + ")";

            tile.transform.position = node.position;
            tile.transform.localScale=new Vector3(1f - borderSize, 1, 1f-borderSize);

            tileCollider.transform.localScale = new Vector3(1f - borderSize, 1, 1f - borderSize);
            tileCollider.center = node.position;
            
        }
    }
    public void ColorNode(Color color, GameObject go)
    {
        if(go!=null)
        {
            Renderer goRenderer = go.GetComponent<Renderer>();
            if(goRenderer!=null)
            {
                goRenderer.material.color = color;
            }
        }
    }

    public void ColorNode(Color color)
    {
        ColorNode(color, tile);
    }
   
}
