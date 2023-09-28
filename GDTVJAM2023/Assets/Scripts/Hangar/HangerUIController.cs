using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class HangerUIController : MonoBehaviour
{
    [Header("All Panels")] 
    public PlayerData playerData;
    public ShipData bulletShipData;
    public ShipData rocketShipData;
    public ShipData laserShipData;
    public GameObject bulletShip;
    public GameObject laserShip;
    public GameObject rocketShip;
    public ParticleSystem particleChooseEffect;

    [Header("Ship Panel")]
    public Transform shipValuePanel;
    public Image bkHeader;
    public Image bkHeaderShipIcon;
    public TextMeshProUGUI shipNameText;
    public TextMeshProUGUI shipDescriptionText;
    public TextMeshProUGUI damageValue;
    public TextMeshProUGUI healthValue;
    public TextMeshProUGUI speedValue;
    public TextMeshProUGUI boostValue;
    public TextMeshProUGUI boostDuration;

    [Header("Weapon Panel")]
    public Transform shipWeaponPanel;
    public TextMeshProUGUI weaponHeader;
    public Image weaponImage;
    public TextMeshProUGUI weaponDescription;

    [Header("Upgrade Panel")]
    public Transform shipUpgradePanel;
    public TextMeshProUGUI upgradePointsText;
    public GameObject bulletUpgrades;
    public GameObject rocketUpgrades;
    public GameObject laserObjects;

    private int shipIndex = 0;


    private void Start()
    {
        shipIndex = playerData.playerShip;
        ShipChange(shipIndex, false); 
    }

    //shipToggle
    public void PosShipToggle()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        shipIndex += 1;
        if (shipIndex == 3) shipIndex = 0;
        ShipChange(shipIndex);
    }
    public void negShipToggle()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        shipIndex -= 1;
        if (shipIndex == -1) shipIndex = 2;
        ShipChange(shipIndex);
    }
    private void ShipChange(int shipIndex, bool startEffect = true)
    {
        if (startEffect == true) particleChooseEffect.Play();

        switch (shipIndex)
        {
            case 0: // bullet ship
                bulletShip.SetActive(true);
                rocketShip.SetActive(false);
                laserShip.SetActive(false);

                bulletShip.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.5f, 10, 1);
                rocketShip.transform.DOComplete();
                laserShip.transform.DOComplete();

                if (startEffect == true) TweenPanels();

                shipNameText.text = bulletShipData.shipName;
                bkHeader.color = bulletShipData.mainClassColor;
                bkHeaderShipIcon.sprite = bulletShipData.shipIcon;

                shipDescriptionText.text = bulletShipData.shipDescription;

                damageValue.text = bulletShipData.baseDamage.ToString();
                healthValue.text = bulletShipData.health.ToString();
                speedValue.text = bulletShipData.speed.ToString();
                boostValue.text = bulletShipData.boostPower.ToString();
                boostDuration.text = bulletShipData.boostDuration.ToString();

                weaponHeader.text = bulletShipData.shipWeaponStr;
                weaponHeader.color = bulletShipData.mainClassColor;
                weaponImage.sprite = bulletShipData.shipWeaponImage;
                weaponDescription.text = bulletShipData.shipWeaponDescription;

                SetUpgradePointText(playerData.expBullet);
                bulletUpgrades.SetActive(true);
                rocketUpgrades.SetActive(false);
                laserObjects.SetActive(false);

                playerData.playerShip = 0;
                break;

            case 1: // Rocket ship
                bulletShip.SetActive(false);
                rocketShip.SetActive(true);
                laserShip.SetActive(false);

                bulletShip.transform.DOComplete();
                rocketShip.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.5f, 10, 1);
                laserShip.transform.DOComplete();

                if (startEffect == true) TweenPanels();

                shipNameText.text = rocketShipData.shipName;
                bkHeader.color = rocketShipData.mainClassColor;
                bkHeaderShipIcon.sprite = rocketShipData.shipIcon;

                shipDescriptionText.text = rocketShipData.shipDescription;

                damageValue.text = rocketShipData.baseDamage.ToString();
                healthValue.text = rocketShipData.health.ToString();
                speedValue.text = rocketShipData.speed.ToString();
                boostValue.text = rocketShipData.boostPower.ToString();
                boostDuration.text = rocketShipData.boostDuration.ToString();

                weaponHeader.text = rocketShipData.shipWeaponStr;
                weaponHeader.color = rocketShipData.mainClassColor;
                weaponImage.sprite = rocketShipData.shipWeaponImage;
                weaponDescription.text = rocketShipData.shipWeaponDescription;

                SetUpgradePointText(playerData.expRocket);
                bulletUpgrades.SetActive(false);
                rocketUpgrades.SetActive(true);
                laserObjects.SetActive(false);

                playerData.playerShip = 1;
                break;

            case 2: // Laser ship
                bulletShip.SetActive(false);
                rocketShip.SetActive(false);
                laserShip.SetActive(true);

                bulletShip.transform.DOComplete();
                rocketShip.transform.DOComplete();
                laserShip.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.5f, 10, 1);

                if (startEffect == true) TweenPanels();

                shipNameText.text = laserShipData.shipName;
                bkHeader.color = laserShipData.mainClassColor;
                bkHeaderShipIcon.sprite = laserShipData.shipIcon;

                shipDescriptionText.text = laserShipData.shipDescription;

                damageValue.text = laserShipData.baseDamage.ToString();
                healthValue.text = laserShipData.health.ToString();
                speedValue.text = laserShipData.speed.ToString();
                boostValue.text = laserShipData.boostPower.ToString();
                boostDuration.text = laserShipData.boostDuration.ToString();

                weaponHeader.text = laserShipData.shipWeaponStr;
                weaponHeader.color = laserShipData.mainClassColor;
                weaponImage.sprite = laserShipData.shipWeaponImage;
                weaponDescription.text = laserShipData.shipWeaponDescription;

                SetUpgradePointText(playerData.expLaser);
                bulletUpgrades.SetActive(false);
                rocketUpgrades.SetActive(false);
                laserObjects.SetActive(true);

                playerData.playerShip = 2;
                break;
        }
    }

    private void TweenPanels()
    {
        float tweenvalue = 0.012f;

        shipValuePanel.DOComplete();
        shipWeaponPanel.DOComplete();
        shipUpgradePanel.DOComplete();

        shipValuePanel.DOPunchScale(new Vector3(tweenvalue, tweenvalue, tweenvalue), 0.35f, 12, 1);
        shipWeaponPanel.DOPunchScale(new Vector3(tweenvalue, tweenvalue, tweenvalue), 0.35f, 12, 1);
        shipUpgradePanel.DOPunchScale(new Vector3(tweenvalue, tweenvalue, tweenvalue), 0.35f, 12, 1);
    }

    public void SetUpgradePointText(int value)
    {
        upgradePointsText.text = "Upgrade points: " + value;
    }
}
