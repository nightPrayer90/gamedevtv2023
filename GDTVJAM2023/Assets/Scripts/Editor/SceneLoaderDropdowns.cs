#if UNITY_EDITOR
using UnityEditor;
namespace KingdomOfNight
{
    public partial class SceneLoader
    {
        [MenuItem("Scenes/Encarta")]
        public static void LoadEncarta() { OpenScene("Assets/scenes/Encarta.unity"); }
        [MenuItem("Scenes/GameScene")]
        public static void LoadGameScene() { OpenScene("Assets/scenes/GameScene.unity"); }
        [MenuItem("Scenes/HangarScene")]
        public static void LoadHangarScene() { OpenScene("Assets/scenes/HangarScene.unity"); }
        [MenuItem("Scenes/IntroScene")]
        public static void LoadIntroScene() { OpenScene("Assets/scenes/IntroScene.unity"); }
        [MenuItem("Scenes/MenueScene")]
        public static void LoadMenueScene() { OpenScene("Assets/scenes/MenueScene.unity"); }
        [MenuItem("Scenes/OutroScene")]
        public static void LoadOutroScene() { OpenScene("Assets/scenes/OutroScene.unity"); }
        [MenuItem("Scenes/RLP_TestScene")]
        public static void LoadRLP_TestScene() { OpenScene("Assets/scenes/RLP_TestScene.unity"); }
        [MenuItem("Scenes/ShopScene")]
        public static void LoadShopScene() { OpenScene("Assets/scenes/ShopScene.unity"); }
        [MenuItem("Scenes/SkillBordScene")]
        public static void LoadSkillBordScene() { OpenScene("Assets/scenes/SkillBordScene.unity"); }
        [MenuItem("Scenes/TestScene")]
        public static void LoadTestScene() { OpenScene("Assets/scenes/TestScene.unity"); }
    }
}
#endif
