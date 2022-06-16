using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticuleManager : MonoBehaviour
{
    public List<GameObject> ShootParticle = new List<GameObject>();

    public Dictionary<KindOfTurret, GameObject> KotToParticules = new Dictionary<KindOfTurret, GameObject>();


}
