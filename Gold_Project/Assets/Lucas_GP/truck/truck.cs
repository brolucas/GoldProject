using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class truck : MonoBehaviour
{
    public int Truck_Hp= 10;

    public Text Truck_Coins_Text;

    public static float gold = 1000;

    public GameObject Truck_Game_Over_Screen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.Truck_Hp <= 0 )
        {
            Loose();
        }

        Truck_Coins_Text.text = ("Coins : " + gold.ToString());
    }

    public void TakeDamage()
    {
        this.Truck_Hp --;
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
