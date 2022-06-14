using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baricade : MonoBehaviour
{
    public int baseHp = 10;
    public int hp;
    public int price;
    // Start is called before the first frame update
    void Start()
    {
        hp = baseHp;
        GameManager.Instance.allBarricade.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takeDamage(int nbr)
    {
        hp = hp - nbr;

        if (hp <= 0)
        {
            Pathfinding.Instance.GetGrid().GetXY(transform.position, out int x, out int y);
            Pathfinding.Instance.GetNode(x, y).isBarricade = null;
            Pathfinding.Instance.GetNode(x, y).isUsed = false;
            Pathfinding.Instance.mapHasChanged = true;
            Destroy(this.gameObject);
            GameManager.Instance.baricades.Remove(this.gameObject);
        }
    }
}
