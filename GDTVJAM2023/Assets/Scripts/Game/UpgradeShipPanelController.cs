using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
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
        localPositon = transform.localPosition;

        // set Objects
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }
    private void OnEnable()
    {
        transform.localPosition = localPositon;//new Vector3(0, -211.2f, 0);

        // FadeIn Tween
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.6f, 8, 1).SetUpdate(true);

        // set Ship Panel Image
        ShipPanel.sprite = shipImages[1];
    }

    // FadeOut Tween
    public void FadeOut()
    {
        transform.DOLocalMoveY(-600, 0.5f, true).SetUpdate(true).SetEase(Ease.InQuart);
    }
}
