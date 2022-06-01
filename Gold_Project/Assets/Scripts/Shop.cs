using UnityEngine;
using UnityEngine.UI;
public class Shop : MonoBehaviour
{
    private BuildManager buildManager;
    private Turret turret;
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
        //priceText.text = turret..ToString();
        rangeText.text = turret.range.ToString();
        damageText.text = turret.atqPoints.ToString();
    }
}
