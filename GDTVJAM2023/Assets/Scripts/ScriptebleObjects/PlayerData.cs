using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string playerName;

    public int expBullet = 0;
    public int expRocket = 0;
    public int expLaser = 0;

    public int playerShip = 0;

}
