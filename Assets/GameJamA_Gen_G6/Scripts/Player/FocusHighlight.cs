using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Ilumina el objeto recogible que el jugador esta mirando.
    /// Escucha PlayerInteractor.FocusChanged y colorea los renderers del objeto con un color mas brillante.
    /// No requiere ninguna configuracion en los objetos: funciona con cualquier IPickable.
    /// </summary>
    [RequireComponent(typeof(PlayerInteractor))]
    public class FocusHighlight : MonoBehaviour
    {
        // Multiplicador del color original del material. Valores mayores que 1 lo hacen brillar.
        [SerializeField] private Color tint = new Color(1.9f, 1.7f, 1.1f, 1f);

        private PlayerInteractor interactor;
        private RendererTint tinter;

        private void Awake()
        {
            interactor = GetComponent<PlayerInteractor>();
            tinter = new RendererTint(tint);
        }

        private void OnEnable()
        {
            interactor.FocusChanged += HandleFocusChanged;
        }

        private void OnDisable()
        {
            interactor.FocusChanged -= HandleFocusChanged;
            tinter.Clear();
        }

        private void HandleFocusChanged(IPickable focus)
        {
            if (focus == null)
            {
                tinter.Clear();
                return;
            }

            tinter.Apply(focus.GameObject);
        }
    }
}
