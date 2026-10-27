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

            ChargeBarDrawer.Draw(ChargedThrowable.Charging, fillFromTop, barWidth, barHeight, barMargin);
        }
    }
}
