using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public float Enemy_Speed = 3 ;
    public int Enemy_Damage = 1;
    public int Enemy_HP;
    public bool Enemy_Flying;
    public float Enemy_Reward;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(200 * Enemy_Speed * Time.deltaTime, 0) );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
