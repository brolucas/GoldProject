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
        Turret turret = buildManager.sniperTurret.GetComponent<Turret>();

        buildManager.SetTurretToBuild(buildManager.sniperTurret);

        rangeText.text = buildManager.sniperTurret.range.ToString();
        priceText.text = turret.turretPrice.ToString();
        damageText.text = turret.atqPoints.ToString();
    }
}
