using UnityEngine;

public class MainMenuDeleteBtn : MonoBehaviour
{
    public int btnIndex = 0;
    public StartGameController gameController;

    public void DeleteProfile()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        gameController.DeleteProfile(btnIndex);
    }
}
