using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(BurnGoal))]
    public class BurnHud : MonoBehaviour
    {
        [SerializeField] private string itemName = "Fantasmas";
        [SerializeField] private Color accent = new Color(1f, 0.75f, 0.35f, 1f);
        [SerializeField] private Color completedColor = new Color(0.45f, 1f, 0.5f, 1f);

        private BurnGoal goal;
        private PlayerDragger dragger;
        private BurnZone[] zones;
        private GUIStyle rightStyle;
        private GUIStyle centerStyle;
        private GUIStyle messageStyle;

        private void Awake()
        {
            goal = GetComponent<BurnGoal>();
        }

        private void Start()
        {
            dragger = FindFirstObjectByType<PlayerDragger>();
            zones = FindObjectsByType<BurnZone>(FindObjectsSortMode.None);
        }

        private void OnGUI()
        {
            CreateStyles();

            Color previous = GUI.contentColor;
            GUI.contentColor = goal.IsCompleted ? completedColor : accent;
            GUI.Label(new Rect(Screen.width - 380f, 16f, 364f, 36f), $"{itemName} quemados: {goal.Burned} / {goal.Total}", rightStyle);
            GUI.contentColor = Color.white;
            GUI.Label(new Rect(Screen.width - 380f, 52f, 364f, 36f), $"Restantes: {goal.Remaining}", rightStyle);

            int burning = CountBurning();
            if (burning > 0)
            {
                GUI.contentColor = accent;
                GUI.Label(new Rect(Screen.width - 380f, 88f, 364f, 36f), $"Quemando: {burning}", rightStyle);
            }

            GUI.contentColor = Color.white;
            DrawCarryHint();
            DrawCompleted();
            GUI.contentColor = previous;
        }

        private int CountBurning()
        {
            int total = 0;
            foreach (BurnZone zone in zones)
            {
                if (zone != null)
                {
                    total += zone.BurningCount;
                }
            }

            return total;
        }

        private void DrawCarryHint()
        {
            if (dragger == null || dragger.Current == null || goal.IsCompleted)
            {
                return;
            }

            GUI.Label(new Rect(0f, Screen.height - 110f, Screen.width, 40f), "Llevalo al caldero  |  E: soltar", centerStyle);
        }

        private void DrawCompleted()
        {
            if (!goal.IsCompleted)
            {
                return;
            }

            GUI.contentColor = completedColor;
            GUI.Label(new Rect(0f, Screen.height * 0.25f, Screen.width, 60f), "Era completada", messageStyle);
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
                fontSize = 26,
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
