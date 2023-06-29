using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class UpgradeContainer
{
    public enum MainClass
    {
        Nothing,
        Bullet,
        Explosion,
        Laser,
        Support
    }

    public enum SubClass
    {
        Nothing,
        Swarm,
        Defence,
        Targeting,
        backwards
    }

    public string headerStr;
    public string descriptionStr;
    public Sprite iconPanel;
    public MainClass mainClass;
    public SubClass subClass;

}
