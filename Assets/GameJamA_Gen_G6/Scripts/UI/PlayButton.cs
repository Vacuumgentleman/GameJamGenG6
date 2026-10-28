using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    [RequireComponent(typeof(Button))]
    public class PlayButton : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(GameFlow.StartGame);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(GameFlow.StartGame);
        }
    }
}
