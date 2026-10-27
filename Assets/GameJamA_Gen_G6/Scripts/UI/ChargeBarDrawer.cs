using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Dibuja (IMGUI) la barra vertical de fuerza de cualquier objeto lanzable.
    /// La comparten las interfaces provisionales de caza y plantacion.
    /// </summary>
    public static class ChargeBarDrawer
    {
        public static void Draw(ChargedThrowable throwable, bool fillFromTop, float width, float height, float margin)
        {
            if (throwable == null)
            {
                return;
            }

            Rect frame = new Rect(Screen.width - margin - width, (Screen.height - height) * 0.5f, width, height);

            Color color = Color.Lerp(Color.white, new Color(1f, 0.35f, 0.15f), throwable.Charge01);
            if (throwable.Strain01 > 0.6f)
            {
                // Parpadea para avisar que el brazo esta por cansarse.
                bool on = Mathf.Repeat(Time.unscaledTime * 8f, 1f) < 0.5f;
                color = on ? new Color(1f, 0.15f, 0.1f) : color;
            }

            float fillHeight = (frame.height - 4f) * throwable.Charge01;
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
