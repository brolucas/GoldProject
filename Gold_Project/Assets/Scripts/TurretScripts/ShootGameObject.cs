using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGameObject : MonoBehaviour
{
    public List<Transform> focus;
    [SerializeField] GameObject Projo;
    [SerializeField] bool shoot;

    private void Update()
    {
        if (shoot)
        {
            shoot = false;
            tir();
        }
    }

    public void tir()
    {
        for (int i = 0; i <= focus.Count - 1; i++)
        {
            if (focus[i] != null)
            {
                Vector3 difference = focus[i].position - transform.position;
                float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

                GameObject bullet = Instantiate(Projo, transform.position, Quaternion.Euler(-rotZ, 90, 180));
            }
        }
    }
}
