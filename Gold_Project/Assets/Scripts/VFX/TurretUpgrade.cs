using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretUpgrade : MonoBehaviour
{
    private Animator Anim;
    [SerializeField] private ParticleSystem Particles;

    void Start()
    {
        Anim = GetComponent<Animator>();
    }
    [ContextMenu("Upgrade")]
    public void UpgradeTurret()
    {
        Anim.SetBool("upgrading", true);
    }
    public void FinUpdate()
    {
        Anim.SetBool("upgrading", false);

    }
    public void UpgradeParticles()
    {

        Instantiate(Particles, transform.position, Quaternion.identity);
    }
}
