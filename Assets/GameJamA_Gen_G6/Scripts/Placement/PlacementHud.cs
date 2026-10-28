using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(PlacementGoal))]
    public class PlacementHud : MonoBehaviour
    {
        [SerializeField] private string itemName = "Objetos";
        [SerializeField] private float rejectedSeconds = 1.5f;
        [SerializeField] private Color accent = new Color(1f, 0.75f, 0.35f, 1f);
        [SerializeField] private Color completedColor = new Color(0.45f, 1f, 0.5f, 1f);
        [SerializeField] private Color rejectedColor = new Color(1f, 0.45f, 0.4f, 1f);

        private PlacementGoal goal;
        private PlayerPlacer placer;
        private float rejectedUntil;
        private GUIStyle rightStyle;
        private GUIStyle centerStyle;
        private GUIStyle messageStyle;

        private void Awake()
        {
            goal = GetComponent<PlacementGoal>();
        }

        private void Start()
        {
            placer = FindFirstObjectByType<PlayerPlacer>();
            if (placer != null)
            {
                placer.Rejected += HandleRejected;
            }
        }

        private void OnDestroy()
        {
            if (placer != null)
            {
                placer.Rejected -= HandleRejected;
            }
        }

        private void HandleRejected(PlacementSlot slot, PlaceableItem item)
        {
            rejectedUntil = Time.time + rejectedSeconds;
        }

        private void OnGUI()
        {
            CreateStyles();

            Color previous = GUI.contentColor;
            GUI.contentColor = goal.IsCompleted ? completedColor : accent;
            GUI.Label(new Rect(Screen.width - 380f, 16f, 364f, 36f), $"{itemName} colocados: {goal.Placed} / {goal.Total}", rightStyle);

            GUI.contentColor = Color.white;
            DrawHint();
            DrawRejected();
            DrawCompleted();
            GUI.contentColor = previous;
        }

        private void DrawHint()
        {
            if (placer == null || placer.Held == null || goal.IsCompleted)
            {
                return;
            }

            GUI.Label(new Rect(0f, Screen.height - 110f, Screen.width, 40f), $"{placer.Held.DisplayName}: mira su espacio y pulsa E  |  G: soltar", centerStyle);
        }

        private void DrawRejected()
        {
            if (Time.time >= rejectedUntil)
            {
                return;
            }

            GUI.contentColor = rejectedColor;
            GUI.Label(new Rect(0f, Screen.height - 150f, Screen.width, 40f), "Ese objeto no va aqui", centerStyle);
            GUI.contentColor = Color.white;
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
