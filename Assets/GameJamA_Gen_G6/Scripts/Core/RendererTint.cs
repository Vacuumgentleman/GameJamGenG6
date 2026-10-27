using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Ilumina todos los renderers de un objeto multiplicando el color de sus materiales.
    /// Usa MaterialPropertyBlock: no modifica los materiales ni crea copias, y se revierte con Clear.
    /// </summary>
    public sealed class RendererTint
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private readonly Color tint;
        private readonly MaterialPropertyBlock block = new MaterialPropertyBlock();
        private readonly List<Renderer> tinted = new List<Renderer>();
        private readonly List<Renderer> buffer = new List<Renderer>();

        public RendererTint(Color tint)
        {
            this.tint = tint;
        }

        // Quita el resaltado anterior e ilumina el objeto indicado.
        public void Apply(GameObject target)
        {
            Clear();

            target.GetComponentsInChildren(false, buffer);
            foreach (Renderer renderer in buffer)
            {
                ApplyTo(renderer);
                tinted.Add(renderer);
            }
        }

        public void Clear()
        {
            foreach (Renderer renderer in tinted)
            {
                if (renderer == null)
                {
                    continue;
                }

                int count = renderer.sharedMaterials.Length;
                for (int i = 0; i < count; i++)
                {
                    renderer.SetPropertyBlock(null, i);
                }
            }

            tinted.Clear();
        }

        private void ApplyTo(Renderer renderer)
        {
            Material[] materials = renderer.sharedMaterials;
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
                renderer.SetPropertyBlock(block, i);
            }
        }
    }
}
