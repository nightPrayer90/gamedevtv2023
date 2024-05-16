using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UpgradeShipPanelController : MonoBehaviour
{
    private Vector3 localPositon;

    [HideInInspector] public List<Color> classColors;

    //Objects
    public Image ShipPanel;
    public PlayerWeaponController playerWeaponController;

    public Image[] classPointsBullets;
    public Image[] classPointsExplosion;
    public Image[] classPointsLaser;
    public Image[] classPointsSupport;
    public Image[] classUpImage;
    public Sprite[] classUpSprite;


    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Awake()
    {
        // save the first local position
        localPositon = transform.localPosition;

    }
    private void OnEnable()
    {
        transform.localPosition = localPositon;//new Vector3(0, -211.2f, 0);

        // FadeIn Tween
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.6f, 8, 1).SetUpdate(true);

        ClassBarFiller();
    }



    /* **************************************************************************** */
    /* MISC ----------------------------------------------------------------------- */
    /* **************************************************************************** */
    // FadeOut Tween
    public void FadeOut()
    {
        transform.DOLocalMoveY(-600, 0.5f, true).SetUpdate(true).SetEase(Ease.InQuart);
    }

    void ClassBarFiller()
    {
        // Class Image
        if (playerWeaponController.shipData.mcBulletLvl > 0)
        { classUpImage[0].sprite = classUpSprite[0]; }

        if (playerWeaponController.shipData.mcExplosionLvl > 0)
        { classUpImage[1].sprite = classUpSprite[1]; }

        if (playerWeaponController.shipData.mcLaserLvl > 0)
        { classUpImage[2].sprite = classUpSprite[2]; }

        if (playerWeaponController.shipData.mcSupportLvl > 0)
        { classUpImage[3].sprite = classUpSprite[3]; }


        // Class qantity
        for (int i = 0; i < 8; i++) 
        {
            if (!DisplayClassPoints(playerWeaponController.shipData.mcBulletLvl, i))
            { classPointsBullets[i].color = Color.white; }

            if (!DisplayClassPoints(playerWeaponController.shipData.mcExplosionLvl, i))
            { classPointsExplosion[i].color = Color.white; }

            if (!DisplayClassPoints(playerWeaponController.shipData.mcLaserLvl, i))
            { classPointsLaser[i].color = Color.white; }

            if (!DisplayClassPoints(playerWeaponController.shipData.mcSupportLvl, i))
            { classPointsSupport[i].color = Color.white; }
        }

     
    }

    bool DisplayClassPoints(int _class, int pointNumber)
    {
        return ((pointNumber) >= _class);
    }
}
