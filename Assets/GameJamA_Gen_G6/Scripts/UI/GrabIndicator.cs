using UnityEngine;

namespace GameJam
{
    public class GrabIndicator : MonoBehaviour
    {
        [SerializeField] private float radius = 22f;
        [SerializeField] private float thickness = 4f;
        [SerializeField] private int segments = 64;
        [SerializeField] private Color backgroundColor = new Color(1f, 1f, 1f, 0.25f);
        [SerializeField] private Color fillColor = new Color(1f, 0.85f, 0.4f, 0.95f);

        private PlayerDragger dragger;
        private PlayerInteractor interactor;
        private Material material;

        private void Start()
        {
            dragger = FindFirstObjectByType<PlayerDragger>();
            interactor = FindFirstObjectByType<PlayerInteractor>();
        }

        private void OnDestroy()
        {
            if (material != null)
            {
                Destroy(material);
            }
        }

        private void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            float progress = Mathf.Max(
                dragger != null ? dragger.GrabProgress : 0f,
                interactor != null ? interactor.GrabProgress : 0f);
            if (progress <= 0f)
            {
                return;
            }

            EnsureMaterial();
            Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            GL.PushMatrix();
            GL.LoadPixelMatrix();
            material.SetPass(0);
            GL.Begin(GL.TRIANGLES);
            DrawArc(center, 1f, backgroundColor);
            DrawArc(center, progress, fillColor);
            GL.End();
            GL.PopMatrix();
        }

        private void DrawArc(Vector2 center, float fraction, Color color)
        {
            int count = Mathf.Max(1, Mathf.CeilToInt(segments * fraction));
            float step = Mathf.PI * 2f * fraction / count;
            float inner = radius - thickness * 0.5f;
            float outer = radius + thickness * 0.5f;

            GL.Color(color);
            for (int i = 0; i < count; i++)
            {
                float a0 = Mathf.PI * 0.5f - step * i;
                float a1 = Mathf.PI * 0.5f - step * (i + 1);

                Vector2 i0 = center + new Vector2(Mathf.Cos(a0), Mathf.Sin(a0)) * inner;
                Vector2 o0 = center + new Vector2(Mathf.Cos(a0), Mathf.Sin(a0)) * outer;
                Vector2 i1 = center + new Vector2(Mathf.Cos(a1), Mathf.Sin(a1)) * inner;
                Vector2 o1 = center + new Vector2(Mathf.Cos(a1), Mathf.Sin(a1)) * outer;

                GL.Vertex3(i0.x, i0.y, 0f);
                GL.Vertex3(o0.x, o0.y, 0f);
                GL.Vertex3(o1.x, o1.y, 0f);
                GL.Vertex3(i0.x, i0.y, 0f);
                GL.Vertex3(o1.x, o1.y, 0f);
                GL.Vertex3(i1.x, i1.y, 0f);
            }
        }

        private void EnsureMaterial()
        {
            if (material != null)
            {
                return;
            }

            material = new Material(Shader.Find("Hidden/Internal-Colored"))
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            material.SetInt("_ZWrite", 0);
        }
    }
}
