using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticuleManager : MonoBehaviour
{
    public List<GameObject> ShootParticle = new List<GameObject>();

    public Dictionary<KindOfTurret, GameObject> KotToParticules = new Dictionary<KindOfTurret, GameObject>();

    private void Awake()
    {
        /*switch (ShootParticle.)
        {
            default:
                break;
        }
        for (int i = 0; i < ShootParticle.Count; i++)
        {
            KotToParticules.Add(ShootParticle[i].name, ShootParticle[i]);
        }*/

        for (int i = 0; i < GameManager.Instance.turretDatabase.turrets.Count; i++)
        {
            KotToParticules.Add(GameManager.Instance.turretDatabase.turrets[i].kindOfTurret, ShootParticle[i]);
        }
    }
}
