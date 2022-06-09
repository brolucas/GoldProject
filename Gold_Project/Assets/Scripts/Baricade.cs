using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baricade : MonoBehaviour
{
    public int baseHp = 10;
    public int hp;
    // Start is called before the first frame update
    void Start()
    {
        
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
            GameManager.Instance.baricades.RemoveAt(GameManager.Instance.baricades.IndexOf(this.gameObject));
            Destroy(this);

        }
    }
}
