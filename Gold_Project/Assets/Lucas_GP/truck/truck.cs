using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class truck : MonoBehaviour
{
    public int Truck_Hp= 5;

    public Text Truck_Coins_Text;
    //public Text Truck_Waves_Text;

    public float gold;

    public GameObject Truck_Game_Over_Screen;

    //feedback visuel dammag
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Truck_Coins_Text.text = ("  : " + gold.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        Truck_Coins_Text.text = ("  : " + gold.ToString());
    }

    public void TakeDamage()
    {
        this.Truck_Hp --;
        if (this.Truck_Hp <= 0)
        {
            Loose();
        }
        anim.SetBool("dmg", true);
        StartCoroutine(DelaisDmg());
    }
    public void Loose()
    {
        Truck_Game_Over_Screen.SetActive(true);
        //Time.timeScale = 0;
        Destroy(this);
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
    IEnumerator DelaisDmg()
    {
        yield return new WaitForSeconds(.1f);
        anim.SetBool("dmg", false);
    }
}
