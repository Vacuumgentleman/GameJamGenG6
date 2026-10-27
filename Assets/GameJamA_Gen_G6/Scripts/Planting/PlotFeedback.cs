using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Particulas de una parcela: tierra al cavar, polvo al sembrar, gotas al regar, destellos al crecer
    /// y plumas cuando un cuervo se come lo plantado. Solo reacciona a PlantingPlot.ActionPerformed.
    /// </summary>
    [RequireComponent(typeof(PlantingPlot))]
    public class PlotFeedback : MonoBehaviour
    {
        [SerializeField] private Color dirtColor = new Color(0.45f, 0.32f, 0.2f);
        [SerializeField] private Color seedColor = new Color(0.75f, 0.65f, 0.4f);
        [SerializeField] private Color waterColor = new Color(0.3f, 0.6f, 1f);
        [SerializeField] private Color growColor = new Color(0.5f, 0.9f, 0.3f);
        [SerializeField] private Color grownColor = new Color(1f, 0.9f, 0.35f);
        [SerializeField] private Color eatenColor = new Color(0.1f, 0.1f, 0.12f);

        [Header("Brillos de tipo de parcela")]
        [SerializeField] private Color fertileGlint = new Color(1f, 0.95f, 0.4f);
        [SerializeField] private Color springGlint = new Color(0.4f, 0.75f, 1f);
        [SerializeField] private float glintInterval = 1.4f;

        private PlantingPlot plot;
        private float nextGlint;

        private void Awake()
        {
            plot = GetComponent<PlantingPlot>();
        }

        private void OnEnable()
        {
            plot.ActionPerformed += HandleAction;
        }

        private void OnDisable()
        {
            plot.ActionPerformed -= HandleAction;
        }

        // Las parcelas especiales destellan para distinguirse de las normales.
        private void Update()
        {
            if (plot.IsGrown || Time.time < nextGlint)
            {
                return;
            }

            nextGlint = Time.time + glintInterval * Random.Range(0.8f, 1.3f);
            Vector3 position = transform.position + new Vector3(Random.Range(-0.9f, 0.9f), 0.2f, Random.Range(-0.9f, 0.9f));

            switch (plot.Kind)
            {
                case PlotKind.Fertile:
                    ParticleBurst.Play(position, fertileGlint, 3, 0.8f, 0.07f, -0.1f, 1.3f, 15f);
                    break;
                case PlotKind.Spring:
                    ParticleBurst.Play(position, springGlint, 3, 0.8f, 0.07f, -0.1f, 1.3f, 15f);
                    break;
            }
        }

        private void HandleAction(PlantingPlot source, PlotAction action)
        {
            Vector3 position = transform.position + Vector3.up * 0.15f;

            switch (action)
            {
                case PlotAction.Dug:
                    ParticleBurst.Play(position, dirtColor, 16, 3.2f, 0.13f, 1.5f, 0.7f, 50f);
                    break;
                case PlotAction.Planted:
                    ParticleBurst.Play(position, seedColor, 10, 1.8f, 0.09f, 1.2f, 0.6f, 40f);
                    break;
                case PlotAction.Watered:
                    ParticleBurst.Play(position + Vector3.up * 0.8f, waterColor, 22, 2.2f, 0.09f, 1.6f, 0.7f, 60f);
                    break;
                case PlotAction.StageGrown:
                    ParticleBurst.Play(position + Vector3.up * 0.5f, growColor, 14, 2.2f, 0.1f, 0.2f, 0.9f, 55f);
                    break;
                case PlotAction.Grown:
                    ParticleBurst.Play(position + Vector3.up * 0.8f, grownColor, 32, 3.6f, 0.13f, 0.1f, 1.2f, 70f);
                    break;
                case PlotAction.Eaten:
                    ParticleBurst.Play(position + Vector3.up * 0.4f, eatenColor, 18, 3f, 0.12f, 0.8f, 1f, 70f);
                    break;
            }
        }
    }
}
