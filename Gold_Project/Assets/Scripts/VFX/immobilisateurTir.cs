using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class immobilisateurTir : MonoBehaviour
{
    public List<Transform> focus;
    [SerializeField] GameObject snowBall;
    [SerializeField] bool shoot;
    private Animator anim;
    [SerializeField] GameObject TextureNeige;
    private void Start()
    {
        anim = TextureNeige.GetComponent<Animator>();
    }
    private void Update()
    {
        if (shoot)
        {
            anim.SetBool("shoot", true);
            shoot = false;
            tir();
            StartCoroutine(Animation());
        }
    }

    public void tir()
    {
        for (int i = 0; i <= focus.Count - 1; i++)
        {
            if(focus[i] != null)
            {
                Vector3 difference = focus[i].position - transform.position;
                float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                
                GameObject neige = Instantiate(snowBall, transform.position, Quaternion.Euler( - rotZ, 90, 180));
            }
        }
    }
    IEnumerator Animation()
    {
        yield return new WaitForSeconds(.5f);
        anim.SetBool("shoot", false);
    }
}
