using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class HangerUIController : MonoBehaviour
{
    public GameObject bulletShip;
    public GameObject laserShip;
    public GameObject rocketShip;
    public ShipData bulletShipData;
    public ShipData rocketShipData;
    public ShipData laserShipData;

    public Image bkHeader;
    public Image bkHeaderShipIcon;

    public TextMeshProUGUI weaponHeader;
    public Image weaponImage;
    public TextMeshProUGUI weaponDescription;

    public TextMeshProUGUI damageValue;
    public TextMeshProUGUI healthValue;
    public TextMeshProUGUI speedValue;
    public TextMeshProUGUI boostValue;
    public TextMeshProUGUI boostDuration;

    private int shipIndex=0;
    public TextMeshProUGUI shipNameText;
    public TextMeshProUGUI shipDescriptionText;
    public ParticleSystem particleChooseEffect;
    public PlayerData playerData;

    private void Start()
    {
        ShipChange(playerData.playerShip); // todo Playerdate save and load
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
    private void ShipChange(int shipIndex)
    {
        particleChooseEffect.Play();
        switch (shipIndex)
        {
            case 0: // bullet ship
                bulletShip.SetActive(true);
                rocketShip.SetActive(false);
                laserShip.SetActive(false);

                bulletShip.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.5f, 10, 1);
                rocketShip.transform.DOComplete();
                laserShip.transform.DOComplete();

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

                playerData.playerShip = 0;
                break;

            case 1: // Rocket ship
                bulletShip.SetActive(false);
                rocketShip.SetActive(true);
                laserShip.SetActive(false);

                bulletShip.transform.DOComplete();
                rocketShip.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.5f, 10, 1);
                laserShip.transform.DOComplete();

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

                playerData.playerShip = 1;
                break;

            case 2: // Laser ship
                bulletShip.SetActive(false);
                rocketShip.SetActive(false);
                laserShip.SetActive(true);

                bulletShip.transform.DOComplete();
                rocketShip.transform.DOComplete();
                laserShip.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.5f, 10, 1);

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

                playerData.playerShip = 2;
                break;
        }
    }

}
