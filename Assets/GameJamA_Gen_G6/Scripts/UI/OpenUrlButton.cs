using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    [RequireComponent(typeof(Button))]
    public class OpenUrlButton : MonoBehaviour
    {
        [SerializeField] private string url;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(Open);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(Open);
        }

        private void Open()
        {
            if (!string.IsNullOrEmpty(url))
            {
                Application.OpenURL(url);
            }
        }
    }
}
