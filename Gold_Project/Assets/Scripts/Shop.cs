using UnityEngine;

public class Shop : MonoBehaviour
{
    private BuildManager buildManager;

    private void Start()
    {
        buildManager = BuildManager.Instance;
    }

    public void PurchaseStandardTurret()
    {
        Debug.Log("Sniper séléctionner");

        buildManager.SetTurretToBuild(buildManager.sniperTurret);
    }
}
