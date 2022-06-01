using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    #region Singleton
    private static BuildManager instance = null;

    // Game Instance Singleton
    public static BuildManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<BuildManager>();

                return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null)
        {
            Destroy(this.gameObject);
        }

        instance = this;

        Debug.Log(instance);

    }
    #endregion

    public GameObject sniperTurret;

    public GameObject turretToBuild;

    public GameObject GetTurretToBuild()
    {
        return turretToBuild;
    }

    public void SetTurretToBuild(GameObject turret)
    {
        turretToBuild = turret;
    }

    public void CreateTurret(KindOfTurret type)
    {
        GameObject newTurret = Instantiate(turretToBuild);

        newTurret.GetComponent<Turret>().InitTurretData(type);
    }
}
