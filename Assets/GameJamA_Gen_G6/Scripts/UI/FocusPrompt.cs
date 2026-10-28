using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Mira central y mensaje "E - Recoger: nombre" cuando se mira un objeto recogible.
    /// Es una version provisional hecha con IMGUI para poder probar ya.
    /// Kim puede reemplazarla por el HUD definitivo: solo tiene que suscribirse a PlayerInteractor.FocusChanged
    /// y borrar este componente del prefab Player.
    /// </summary>
    [RequireComponent(typeof(PlayerInteractor))]
    public class FocusPrompt : MonoBehaviour
    {
        [SerializeField] private float crosshairSize = 6f;
        [SerializeField] private Color crosshairColor = new Color(1f, 1f, 1f, 0.85f);
        [SerializeField] private Color textColor = new Color(1f, 0.95f, 0.7f, 1f);

        private PlayerInteractor interactor;
        private GUIStyle style;

        private void Awake()
        {
            interactor = GetComponent<PlayerInteractor>();
        }

        private void OnGUI()
        {
            float centerX = Screen.width * 0.5f;
            float centerY = Screen.height * 0.5f;

            // Punto en el centro de la pantalla para apuntar.
            GUI.color = crosshairColor;
            GUI.DrawTexture(
                new Rect(centerX - crosshairSize * 0.5f, centerY - crosshairSize * 0.5f, crosshairSize, crosshairSize),
                Texture2D.whiteTexture);

            IPickable focus = interactor.Focus;
            if (focus == null || focus is IHighlightable)
            {
                return;
            }

            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter };
            }

            style.fontSize = Mathf.RoundToInt(Screen.height * 0.025f);
            style.normal.textColor = textColor;

            GUI.color = Color.white;
            GUI.Label(new Rect(centerX - 300f, centerY + 30f, 600f, 60f), $"E  -  Recoger: {focus.DisplayName}", style);
        }
    }
}
