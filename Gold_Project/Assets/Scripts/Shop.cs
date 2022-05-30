using UnityEngine;

public class Shop : MonoBehaviour
{
    private BuildManager buildManager;

    private void Start()
    {
        buildManager = BuildManager.Instance;
    }

    public void PurchaseTurret()
    {
        //Debug.Log("Sniper s�l�ctionner");

        buildManager.SetTurretToBuild(buildManager.sniperTurret);
    }
}
