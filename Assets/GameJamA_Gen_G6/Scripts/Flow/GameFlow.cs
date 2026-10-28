using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam
{
    public static class GameFlow
    {
        public static void StartGame()
        {
            GameProgress.Reset();
            LoadHub();
        }

        public static void LoadHub()
        {
            LevelCatalog catalog = LevelCatalog.Load();
            if (catalog == null)
            {
                Debug.LogError("Falta el asset LevelCatalog en una carpeta Resources.");
                return;
            }

            Load(catalog.HubScene);
        }

        public static void EnterLevel(LevelDefinition level)
        {
            Load(level.SceneName);
        }

        private static void Load(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"No se puede cargar '{sceneName}': agregala a Build Profiles (Scene List).");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
