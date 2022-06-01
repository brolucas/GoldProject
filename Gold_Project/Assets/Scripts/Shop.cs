using UnityEngine;
using UnityEngine.UI;
public class Shop : MonoBehaviour
{
    private BuildManager buildManager;
    public Text priceText;
    public Text rangeText;
    public Text damageText;

    private void Start()
    {
        buildManager = BuildManager.Instance;
    }

    public void PurchaseTurret()
    {
        //Debug.Log("Sniper séléctionner");

        buildManager.SetTurretToBuild(buildManager.sniperTurret);
    }
}
