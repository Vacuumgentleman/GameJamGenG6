using System;
using UnityEngine;

namespace GameJam
{
    public static class GamePause
    {
        public static bool IsPaused { get; private set; }

        public static event Action<bool> Changed;

        public static void Set(bool paused)
        {
            if (IsPaused == paused)
            {
                return;
            }

            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
            Changed?.Invoke(paused);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            IsPaused = false;
            Changed = null;
        }
    }
}
