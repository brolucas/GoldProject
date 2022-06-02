using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class truck : MonoBehaviour
{
    public int Truck_Hp= 5;

    public Text Truck_Coins_Text;
    //public Text Truck_Waves_Text;

    public static float gold = 200;

    public GameObject Truck_Game_Over_Screen;
    public GameObject Truck_Victory_Screen;
    public WaveSpawner WS;
   

    // Update is called once per frame
    void Update()
    {
        if (!WS.isActiveAndEnabled && Truck_Hp > 0)
        {
            Truck_Victory_Screen.SetActive(true);
            Time.timeScale = 0;
        }

        Truck_Coins_Text.text = ("  : " + gold.ToString());
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
            StartCoroutine(collision.gameObject.GetComponent<EnemiesTemp>().Die());
        }
    }
}
