using UnityEngine;

namespace GameJam
{
    public class PauseController : MonoBehaviour
    {
        private PlayerInputReader input;

        private void Awake()
        {
            input = FindFirstObjectByType<PlayerInputReader>();
            GamePause.Set(false);
        }

        private void OnEnable()
        {
            input.PausePerformed += Toggle;
        }

        private void OnDisable()
        {
            input.PausePerformed -= Toggle;
        }

        private void OnDestroy()
        {
            GamePause.Set(false);
        }

        private void Toggle()
        {
            GamePause.Set(!GamePause.IsPaused);
        }
    }
}
