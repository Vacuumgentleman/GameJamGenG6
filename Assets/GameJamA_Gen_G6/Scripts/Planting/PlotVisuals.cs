using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Presentacion de una parcela: activa y escala los objetos hijos segun su estado, muestra la sed,
    /// el progreso de la etapa y un pequeno rebote al crecer.
    /// Arte puede reemplazar los hijos por modelos finales sin tocar la logica.
    /// </summary>
    [RequireComponent(typeof(PlantingPlot))]
    public class PlotVisuals : MonoBehaviour
    {
        [Header("Estados")]
        [SerializeField] private GameObject rocks;
        [SerializeField] private GameObject dug;
        [SerializeField] private GameObject seed;
        [SerializeField] private Transform olive;
        [SerializeField] private Transform vine;

        [Header("Indicadores")]
        // Gota que flota sobre la planta cuando necesita agua.
        [SerializeField] private Transform thirst;

        // Barra que muestra el avance de la etapa de crecimiento; mira siempre a la camara.
        [SerializeField] private Transform progressRoot;
        [SerializeField] private Transform progressFill;
        [SerializeField] private float progressWidth = 1.2f;

        [Header("Crecimiento")]
        // Escala de la planta recien brotada (0 a 1 del tamano final).
        [SerializeField, Range(0f, 1f)] private float sproutScale = 0.2f;

        [Header("Rebote")]
        [SerializeField] private float punchStrength = 0.25f;
        [SerializeField] private float punchDecay = 2f;

        private PlantingPlot plot;
        private Vector3 oliveScale;
        private Vector3 vineScale;
        private Vector3 thirstOrigin;
        private Transform viewer;
        private float punch;

        private void Awake()
        {
            plot = GetComponent<PlantingPlot>();
            oliveScale = olive != null ? olive.localScale : Vector3.one;
            vineScale = vine != null ? vine.localScale : Vector3.one;
            thirstOrigin = thirst != null ? thirst.localPosition : Vector3.zero;
        }

        private void OnEnable()
        {
            plot.Changed += Refresh;
            plot.ActionPerformed += HandleAction;
        }

        private void Start()
        {
            PlayerAim aim = FindFirstObjectByType<PlayerAim>();
            viewer = aim != null && aim.PlayerCamera != null ? aim.PlayerCamera.transform : null;
            Refresh(plot);
        }

        private void OnDisable()
        {
            plot.Changed -= Refresh;
            plot.ActionPerformed -= HandleAction;
        }

        private void Update()
        {
            punch = Mathf.MoveTowards(punch, 0f, punchDecay * Time.deltaTime);

            if (plot.HasPlant)
            {
                ApplyGrowth();
            }

            UpdateThirst();
            UpdateProgress();
        }

        private void HandleAction(PlantingPlot source, PlotAction action)
        {
            if (action == PlotAction.StageGrown || action == PlotAction.Grown || action == PlotAction.Watered)
            {
                punch = punchStrength;
            }
        }

        private void Refresh(PlantingPlot source)
        {
            PlotState state = plot.State;
            bool hasPlant = plot.HasPlant;

            bool showRocks = state == PlotState.Empty && plot.DiggingsRequired > 1 && plot.DiggingsLeft > 0;
            SetActive(rocks, showRocks);
            if (showRocks && rocks != null)
            {
                rocks.transform.localScale = Vector3.one * Mathf.Lerp(0.4f, 1f, plot.DiggingsLeft / (float)plot.DiggingsRequired);
            }

            SetActive(dug, state != PlotState.Empty);
            SetActive(seed, state == PlotState.NeedsWater && !hasPlant);
            SetActive(olive != null ? olive.gameObject : null, hasPlant && plot.Crop == CropType.Olive);
            SetActive(vine != null ? vine.gameObject : null, hasPlant && plot.Crop == CropType.Vine);

            if (hasPlant)
            {
                ApplyGrowth();
            }
        }

        private void ApplyGrowth()
        {
            float scale = Mathf.Lerp(sproutScale, 1f, plot.Growth01) * (1f + punch);
            if (olive != null)
            {
                olive.localScale = oliveScale * scale;
            }

            if (vine != null)
            {
                vine.localScale = vineScale * scale;
            }
        }

        private void UpdateThirst()
        {
            if (thirst == null)
            {
                return;
            }

            bool thirsty = plot.State == PlotState.NeedsWater;
            SetActive(thirst.gameObject, thirsty);
            if (thirsty)
            {
                thirst.localPosition = thirstOrigin + Vector3.up * (Mathf.Sin(Time.time * 4f) * 0.08f);
            }
        }

        private void UpdateProgress()
        {
            if (progressRoot == null)
            {
                return;
            }

            bool growing = plot.State == PlotState.Growing && !plot.IsGrown;
            SetActive(progressRoot.gameObject, growing);
            if (!growing)
            {
                return;
            }

            if (viewer != null)
            {
                progressRoot.rotation = viewer.rotation;
            }

            if (progressFill != null)
            {
                Vector3 scale = progressFill.localScale;
                scale.x = Mathf.Max(0.001f, progressWidth * plot.StageProgress);
                progressFill.localScale = scale;
            }
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null && target.activeSelf != active)
            {
                target.SetActive(active);
            }
        }
    }
}
