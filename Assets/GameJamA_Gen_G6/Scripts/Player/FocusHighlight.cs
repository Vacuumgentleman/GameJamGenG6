using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Ilumina el objeto recogible que el jugador esta mirando.
    /// Escucha PlayerInteractor.FocusChanged y colorea los renderers del objeto con un color mas brillante.
    /// Usa MaterialPropertyBlock: no modifica los materiales originales ni crea copias, y se revierte limpiando el bloque.
    /// No requiere ninguna configuracion en los objetos: funciona con cualquier IPickable.
    /// </summary>
    [RequireComponent(typeof(PlayerInteractor))]
    public class FocusHighlight : MonoBehaviour
    {
        // Multiplicador del color original del material. Valores mayores que 1 lo hacen brillar.
        [SerializeField] private Color tint = new Color(1.9f, 1.7f, 1.1f, 1f);

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private PlayerInteractor interactor;
        private MaterialPropertyBlock block;
        private readonly List<Renderer> highlighted = new List<Renderer>();

        private void Awake()
        {
            interactor = GetComponent<PlayerInteractor>();
            block = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            interactor.FocusChanged += HandleFocusChanged;
        }

        private void OnDisable()
        {
            interactor.FocusChanged -= HandleFocusChanged;
            Clear();
        }

        private void HandleFocusChanged(IPickable focus)
        {
            Clear();

            if (focus == null)
            {
                return;
            }

            foreach (Renderer target in focus.GameObject.GetComponentsInChildren<Renderer>())
            {
                Apply(target);
                highlighted.Add(target);
            }
        }

        // Cada material del renderer recibe su propio color original multiplicado por el tinte.
        private void Apply(Renderer target)
        {
            Material[] materials = target.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                if (material == null)
                {
                    continue;
                }

                int propertyId;
                if (material.HasProperty(BaseColorId))
                {
                    propertyId = BaseColorId;
                }
                else if (material.HasProperty(ColorId))
                {
                    propertyId = ColorId;
                }
                else
                {
                    continue;
                }

                block.Clear();
                block.SetColor(propertyId, material.GetColor(propertyId) * tint);
                target.SetPropertyBlock(block, i);
            }
        }

        // Quita el resaltado de todo lo que estaba iluminado.
        private void Clear()
        {
            foreach (Renderer target in highlighted)
            {
                if (target == null)
                {
                    continue;
                }

                int count = target.sharedMaterials.Length;
                for (int i = 0; i < count; i++)
                {
                    target.SetPropertyBlock(null, i);
                }
            }

            highlighted.Clear();
        }
    }
}
