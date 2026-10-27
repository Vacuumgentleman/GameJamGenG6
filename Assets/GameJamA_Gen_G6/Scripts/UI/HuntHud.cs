using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Interfaz provisional del prototipo de caza (IMGUI): contador de presas y mensaje final.
    /// Kim puede reemplazarla por la interfaz definitiva leyendo los datos de HuntGoal.
    /// </summary>
    [RequireComponent(typeof(HuntGoal))]
    public class HuntHud : MonoBehaviour
    {
        [Header("Barra de fuerza")]
        // true: la barra se llena de arriba hacia abajo. false: de abajo hacia arriba.
        [SerializeField] private bool fillFromTop = true;
        [SerializeField] private float barWidth = 18f;
        [SerializeField] private float barHeight = 240f;
        [SerializeField] private float barMargin = 24f;

        private HuntGoal goal;
        private GUIStyle counterStyle;
        private GUIStyle messageStyle;

        private void Awake()
        {
            goal = GetComponent<HuntGoal>();
        }

        private void OnGUI()
        {
            if (counterStyle == null)
            {
                counterStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 22,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperRight
                };
                messageStyle = new GUIStyle(counterStyle)
                {
                    fontSize = 34,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            GUI.Label(new Rect(Screen.width - 320f, 16f, 304f, 36f), $"Presas: {goal.Hunted} / {goal.Total}", counterStyle);

            if (goal.IsCompleted)
            {
                GUI.Label(new Rect(0f, Screen.height * 0.25f, Screen.width, 60f), "Caceria completada", messageStyle);
            }

            Spear charging = Spear.Charging;
            if (charging != null)
            {
                DrawChargeBar(charging);
            }
        }

        // Barra vertical en el borde derecho de la pantalla.
        private void DrawChargeBar(Spear spear)
        {
            Rect frame = new Rect(Screen.width - barMargin - barWidth, (Screen.height - barHeight) * 0.5f, barWidth, barHeight);

            Color color = Color.Lerp(Color.white, new Color(1f, 0.35f, 0.15f), spear.Charge01);
            if (spear.Strain01 > 0.6f)
            {
                // Parpadea para avisar que el brazo esta por cansarse.
                bool on = Mathf.Repeat(Time.unscaledTime * 8f, 1f) < 0.5f;
                color = on ? new Color(1f, 0.15f, 0.1f) : color;
            }

            float fill = spear.Charge01;
            float fillHeight = (frame.height - 4f) * fill;
            float fillY = fillFromTop ? frame.y + 2f : frame.yMax - 2f - fillHeight;

            Color previous = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(frame, Texture2D.whiteTexture);
            GUI.color = color;
            GUI.DrawTexture(new Rect(frame.x + 2f, fillY, frame.width - 4f, fillHeight), Texture2D.whiteTexture);
            GUI.color = previous;
        }
    }
}
