using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam
{
    public class PauseMenu : MonoBehaviour
    {
        private const string VolumeKey = "Volume";

        [SerializeField] private Vector2 size = new Vector2(360f, 230f);
        [SerializeField] private Color panelColor = new Color(0f, 0f, 0f, 0.7f);
        [SerializeField] private Color accentColor = new Color(1f, 0.85f, 0.4f, 1f);

        private bool inHub;
        private Texture2D panelTexture;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;

        private void Start()
        {
            LevelCatalog catalog = LevelCatalog.Load();
            inHub = catalog != null && catalog.HubScene == SceneManager.GetActiveScene().name;
            if (PlayerPrefs.HasKey(VolumeKey))
            {
                AudioListener.volume = PlayerPrefs.GetFloat(VolumeKey);
            }
        }

        private void OnDestroy()
        {
            if (panelTexture != null)
            {
                Destroy(panelTexture);
            }
        }

        private void OnGUI()
        {
            if (!GamePause.IsPaused)
            {
                return;
            }

            CreateStyles();

            Rect panel = new Rect((Screen.width - size.x) * 0.5f, (Screen.height - size.y) * 0.5f, size.x, size.y);
            GUI.DrawTexture(panel, panelTexture);

            GUILayout.BeginArea(new Rect(panel.x + 24f, panel.y + 16f, panel.width - 48f, panel.height - 32f));
            GUILayout.Label("PAUSA", titleStyle);
            GUILayout.Space(8f);
            DrawVolume();
            GUILayout.Space(14f);
            DrawBackButton();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Esc: continuar", labelStyle);
            GUILayout.EndArea();
        }

        private void DrawVolume()
        {
            GUILayout.Label($"Volumen: {Mathf.RoundToInt(AudioListener.volume * 100f)}%", labelStyle);
            float value = GUILayout.HorizontalSlider(AudioListener.volume, 0f, 1f);
            if (!Mathf.Approximately(value, AudioListener.volume))
            {
                AudioListener.volume = value;
                PlayerPrefs.SetFloat(VolumeKey, value);
            }
        }

        private void DrawBackButton()
        {
            if (inHub)
            {
                return;
            }

            if (GUILayout.Button("Volver a la nave", buttonStyle, GUILayout.Height(40f)))
            {
                GameFlow.LoadHub();
            }
        }

        private void CreateStyles()
        {
            if (titleStyle != null)
            {
                return;
            }

            panelTexture = new Texture2D(1, 1);
            panelTexture.SetPixel(0, 0, panelColor);
            panelTexture.Apply();

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 34,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            titleStyle.normal.textColor = accentColor;

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };
            labelStyle.normal.textColor = Color.white;

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
        }
    }
}
