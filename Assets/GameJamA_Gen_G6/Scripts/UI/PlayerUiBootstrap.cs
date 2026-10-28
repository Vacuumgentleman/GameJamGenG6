using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam
{
    public static class PlayerUiBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
            Attach();
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Attach();
        }

        private static void Attach()
        {
            if (Object.FindFirstObjectByType<PlayerInputReader>() == null || Object.FindFirstObjectByType<PauseController>() != null)
            {
                return;
            }

            GameObject host = new GameObject("PlayerUi");
            host.AddComponent<PauseController>();
            host.AddComponent<ControlsHud>();
            host.AddComponent<PauseMenu>();
        }
    }
}
