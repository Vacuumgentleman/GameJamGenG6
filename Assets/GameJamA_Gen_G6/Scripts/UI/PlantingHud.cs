using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Interfaz provisional del prototipo de plantacion (IMGUI): tiempo, parcelas, anfora y mensajes.
    /// Kim puede reemplazarla leyendo los datos de PlantingGoal, LevelTimer y WaterJar.
    /// </summary>
    [RequireComponent(typeof(PlantingGoal))]
    public class PlantingHud : MonoBehaviour
    {
        [SerializeField] private LevelTimer timer;

        [Header("Barra de fuerza")]
        [SerializeField] private bool fillFromTop = true;
        [SerializeField] private float barWidth = 18f;
        [SerializeField] private float barHeight = 240f;
        [SerializeField] private float barMargin = 24f;

        [Header("Avisos")]
        [SerializeField] private float lowTimeThreshold = 30f;
        [SerializeField] private float bonusFlashSeconds = 1.5f;

        private PlantingGoal goal;
        private GUIStyle rightStyle;
        private GUIStyle centerStyle;
        private GUIStyle messageStyle;
        private float bonusAmount;
        private float bonusUntil;

        private void Awake()
        {
            goal = GetComponent<PlantingGoal>();
        }

        private void Start()
        {
            if (timer == null)
            {
                timer = FindFirstObjectByType<LevelTimer>();
            }

            if (timer != null)
            {
                timer.TimeAdded += HandleTimeAdded;
            }
        }

        private void OnDestroy()
        {
            if (timer != null)
            {
                timer.TimeAdded -= HandleTimeAdded;
            }
        }

        private void HandleTimeAdded(float seconds)
        {
            bonusAmount = seconds;
            bonusUntil = Time.unscaledTime + bonusFlashSeconds;
        }

        private void OnGUI()
        {
            CreateStyles();

            GUI.Label(new Rect(Screen.width - 340f, 16f, 324f, 36f), $"Parcelas: {goal.Grown} / {goal.Total}", rightStyle);

            WaterJar jar = WaterJar.Equipped;
            if (jar != null)
            {
                string text = jar.Charges > 0 ? $"Agua restante: {jar.Charges} / {jar.Capacity}" : "Sin agua: rellena en el pozo";
                GUI.Label(new Rect(Screen.width - 540f, 52f, 524f, 36f), text, rightStyle);
            }

            DrawTimer();
            DrawCrowWarning();
            DrawMessages();
            ChargeBarDrawer.Draw(ChargedThrowable.Charging, fillFromTop, barWidth, barHeight, barMargin);
        }

        private void DrawTimer()
        {
            if (timer == null)
            {
                return;
            }

            int total = Mathf.CeilToInt(timer.Remaining);
            string text = $"{total / 60}:{total % 60:00}";

            Color previous = GUI.contentColor;
            bool low = timer.Remaining <= lowTimeThreshold && !goal.IsCompleted;
            GUI.contentColor = low ? new Color(1f, 0.3f, 0.2f) : Color.white;
            GUI.Label(new Rect(0f, 16f, Screen.width, 40f), text, centerStyle);
            GUI.contentColor = previous;

            if (Time.unscaledTime < bonusUntil)
            {
                GUI.Label(new Rect(0f, 56f, Screen.width, 36f), $"+{bonusAmount:0} s", centerStyle);
            }
        }

        private void DrawCrowWarning()
        {
            if (Crow.EatingCount <= 0 || goal.IsCompleted || (timer != null && timer.IsExpired))
            {
                return;
            }

            Color previous = GUI.contentColor;
            bool on = Mathf.Repeat(Time.unscaledTime * 3f, 1f) < 0.6f;
            GUI.contentColor = on ? new Color(1f, 0.45f, 0.2f) : Color.white;
            GUI.Label(new Rect(0f, 96f, Screen.width, 36f), "Un cuervo ataca una parcela", centerStyle);
            GUI.contentColor = previous;
        }

        private void DrawMessages()
        {
            Rect area = new Rect(0f, Screen.height * 0.25f, Screen.width, 60f);

            if (goal.IsCompleted)
            {
                GUI.Label(area, "Plantacion completada", messageStyle);
                return;
            }

            if (timer != null && timer.IsExpired)
            {
                GUI.Label(area, "Se acabo el tiempo. El bucle se reinicia", messageStyle);
            }
        }

        private void CreateStyles()
        {
            if (rightStyle != null)
            {
                return;
            }

            rightStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperRight
            };
            centerStyle = new GUIStyle(rightStyle)
            {
                fontSize = 30,
                alignment = TextAnchor.UpperCenter
            };
            messageStyle = new GUIStyle(rightStyle)
            {
                fontSize = 34,
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}
