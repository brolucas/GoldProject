using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<EnemiesTemp> enemies = new List<EnemiesTemp>();

    public List<Turret> allTurret = new List<Turret>();

    private static GameManager instance = null;

    public GameObject rangeSprite;

    public TurretDatabase turretDatabase;

    public truck truck;

    public List<GameObject> baricades = new List<GameObject>();

    // Game Instance Singleton
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }

    public TurretData GetStatsKindOfTurret(KindOfTurret kindOfTurret)
    {
        // Get the stats in the scriptable object TurretDatabase based on the type kindOfTurret

        TurretData turretData = GameManager.Instance.turretDatabase.turrets.Find(data => data.kindOfTurret == kindOfTurret);

        return turretData;
    }
}
