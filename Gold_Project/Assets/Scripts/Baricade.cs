using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baricade : MonoBehaviour
{
    public int baseHp = 10;
    public int hp;
    public int price;

    public AudioSource breakingSound;
    // Start is called before the first frame update
    void Start()
    {
        hp = baseHp;
        GameManager.Instance.allBarricade.Add(this);
    }

    private void OnMouseDown()
    {
        BuildManager.Instance.shop.selectedItemInGame = this.gameObject;

        BuildManager.Instance.shop.DisplayCurrentBarricadeStats();
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
            StartCoroutine(BreakingAudio(breakingSound));
            Destroy(this.gameObject);
            GameManager.Instance.baricades.Remove(this.gameObject);
        }
    }

    private IEnumerator BreakingAudio(AudioSource breakingSound)
    {
        breakingSound.Play();
        yield return new WaitForSeconds(2);
    }
}
