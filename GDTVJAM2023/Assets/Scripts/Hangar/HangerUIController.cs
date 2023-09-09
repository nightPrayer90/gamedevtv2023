using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HangerUIController : MonoBehaviour
{
    public GameObject bulletShip;
    public GameObject laserShip;
    public GameObject rocketShip;
    private int shipIndex=0;
    public TextMeshProUGUI shipNameText;
    public ParticleSystem particleChooseEffect;
    public PlayerData playerData;

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
            case 0:
                bulletShip.SetActive(true);
                rocketShip.SetActive(false);
                laserShip.SetActive(false);

                shipNameText.text = "Bullet Ship";
                playerData.playerShip = 0;
                break;
            case 1:
                bulletShip.SetActive(false);
                rocketShip.SetActive(true);
                laserShip.SetActive(false);

                shipNameText.text = "Rocket Ship";
                playerData.playerShip = 1;
                break;
            case 2:
                bulletShip.SetActive(false);
                rocketShip.SetActive(false);
                laserShip.SetActive(true);

                shipNameText.text = "Laser Ship";
                playerData.playerShip = 2;
                break;
        }
    }

}
