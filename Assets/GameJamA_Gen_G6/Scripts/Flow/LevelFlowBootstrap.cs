using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam
{
    public static class LevelFlowBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
            Attach(SceneManager.GetActiveScene());
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Attach(scene);
        }

        private static void Attach(Scene scene)
        {
            LevelCatalog catalog = LevelCatalog.Load();
            if (catalog == null || !catalog.TryGetByScene(scene.name, out LevelDefinition level))
            {
                return;
            }

            if (Object.FindFirstObjectByType<LevelSession>() != null)
            {
                return;
            }

            GameObject host = new GameObject("LevelSession");
            host.AddComponent<LevelSession>().Initialize(catalog, level);
        }
    }
}
