using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class truck : MonoBehaviour
{
    public int Truck_Hp= 10;

    
    public static float gold = 1000;

    public GameObject Truck_Game_Over_Screen;

   

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void TakeDamage()
    {
        this.Truck_Hp --;
        if (this.Truck_Hp <= 0)
        {
            Loose();
        }
    }
    public void Loose()
    {
        Truck_Game_Over_Screen.SetActive(true);
        Time.timeScale = 0;
        //Debug.Log("Fin de partie");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
            collision.gameObject.GetComponent<EnemiesTemp>().Die();
        }
    }
}
