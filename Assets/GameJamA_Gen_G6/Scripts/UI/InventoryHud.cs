using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Barra de inventario en la parte inferior de la pantalla: una casilla numerada por slot,
    /// con el slot seleccionado resaltado y el nombre del objeto que contiene.
    /// Es una version provisional hecha con IMGUI para poder probar ya.
    /// Kim puede reemplazarla por el HUD definitivo leyendo PlayerInventory
    /// (Capacity, GetItem, SelectedIndex y los eventos ItemAdded, ItemRemoved, SelectionChanged)
    /// y quitando este componente del prefab Player.
    /// </summary>
    [RequireComponent(typeof(PlayerInventory))]
    public class InventoryHud : MonoBehaviour
    {
        // Medidas pensadas para una pantalla de 1080 px de alto; se escalan solas.
        [SerializeField] private float slotSize = 64f;
        [SerializeField] private float spacing = 8f;
        [SerializeField] private float bottomMargin = 32f;

        [SerializeField] private Color normalColor = new Color(0f, 0f, 0f, 0.45f);
        [SerializeField] private Color selectedColor = new Color(1f, 0.85f, 0.3f, 0.45f);
        [SerializeField] private Color normalBorder = new Color(1f, 1f, 1f, 0.35f);
        [SerializeField] private Color selectedBorder = new Color(1f, 0.85f, 0.3f, 1f);

        private PlayerInventory inventory;
        private GUIStyle numberStyle;
        private GUIStyle nameStyle;

        private void Awake()
        {
            inventory = GetComponent<PlayerInventory>();
        }

        private void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            float scale = Screen.height / 1080f;
            float size = slotSize * scale;
            float gap = spacing * scale;
            float border = Mathf.Max(1f, 2f * scale);

            int count = inventory.Capacity;
            float totalWidth = count * size + (count - 1) * gap;
            float startX = (Screen.width - totalWidth) * 0.5f;
            float y = Screen.height - bottomMargin * scale - size;

            EnsureStyles(scale);

            for (int i = 0; i < count; i++)
            {
                Rect rect = new Rect(startX + i * (size + gap), y, size, size);
                bool selected = i == inventory.SelectedIndex;

                DrawRect(rect, selected ? selectedColor : normalColor);
                DrawBorder(rect, selected ? selectedBorder : normalBorder, border);

                // Numero del slot en la esquina superior izquierda.
                GUI.Label(new Rect(rect.x + 5f * scale, rect.y + 2f * scale, size, size * 0.3f), (i + 1).ToString(), numberStyle);

                // Nombre del objeto, si el slot esta ocupado.
                IPickable item = inventory.GetItem(i);
                if (item != null)
                {
                    GUI.Label(new Rect(rect.x, rect.y + size * 0.3f, size, size * 0.65f), item.DisplayName, nameStyle);
                }
            }
        }

        private void EnsureStyles(float scale)
        {
            if (numberStyle == null)
            {
                numberStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperLeft, fontStyle = FontStyle.Bold };
                nameStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, wordWrap = true, clipping = TextClipping.Clip };
            }

            numberStyle.fontSize = Mathf.RoundToInt(16f * scale);
            numberStyle.normal.textColor = Color.white;
            nameStyle.fontSize = Mathf.RoundToInt(13f * scale);
            nameStyle.normal.textColor = Color.white;
        }

        private static void DrawRect(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        private static void DrawBorder(Rect rect, Color color, float thickness)
        {
            DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
        }
    }
}
