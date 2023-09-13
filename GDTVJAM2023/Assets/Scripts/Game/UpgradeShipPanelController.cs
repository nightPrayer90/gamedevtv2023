using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UpgradeShipPanelController : MonoBehaviour
{
    private Vector3 localPositon;

    [HideInInspector] public List<Color> classColors;

    //Objects
    private GameManager gameManager;

    public Image ShipPanel;
    public List<Sprite> shipImages = new List<Sprite>();

    private void Awake()
    {
        // save the first local position
        localPositon = gameObject.transform.position;

        // set Objects
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }
    private void OnEnable()
    {
        // FadeIn Tween
        transform.position = localPositon;
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.6f, 8, 1).SetUpdate(true);

        // set Ship Panel Image
        switch (gameManager.startShip)
        {
            case GameManager.StartShip.bullet:
                ShipPanel.sprite = shipImages[0];
                break;
            case GameManager.StartShip.laser:
                ShipPanel.sprite = shipImages[1];
                break;
            case GameManager.StartShip.rocket:
                ShipPanel.sprite = shipImages[2];
                break;


        }
    }

    // FadeOut Tween
    public void FadeOut()
    {
        transform.DOLocalMoveY(-600, 0.5f, true).SetUpdate(true).SetEase(Ease.InQuart);
    }
}
