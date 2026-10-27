using System;
using UnityEngine;

namespace GameJam
{
    public enum PlotState
    {
        Empty,
        Dug,
        NeedsWater,
        Growing,
        Grown
    }

    public enum PlotKind
    {
        Normal,
        Stony,
        Fertile,
        Spring
    }

    public enum PlotAction
    {
        Dug,
        Planted,
        Watered,
        StageGrown,
        Grown,
        Eaten
    }

    /// <summary>
    /// Parcela de cultivo. Pasa por Empty -> Dug (azada) -> NeedsWater (semillas) -> Growing (agua) y vuelve
    /// a NeedsWater entre etapas hasta llegar a Grown. Los parametros definen el tipo de parcela
    /// (normal, pedregosa, fertil, manantial) sin cambiar el codigo.
    /// No sabe de herramientas ni de la interfaz: reacciona a IDiggable, ISeedBed e IWaterable
    /// y avisa de sus cambios con eventos.
    /// </summary>
    public class PlantingPlot : MonoBehaviour, IDiggable, ISeedBed, IWaterable, IHighlightable
    {
        [Header("Tipo de parcela")]
        // Golpes de azada necesarios para dejar la tierra lista.
        [SerializeField] private int diggingsRequired = 1;

        // Riegos necesarios; cada riego hace crecer una etapa. Con 0 la planta crece sola.
        [SerializeField] private int wateringsRequired = 2;

        // Segundos que dura cada etapa de crecimiento.
        [SerializeField] private float stageTime = 3f;

        // Si es verdadero la parcela ya viene cavada.
        [SerializeField] private bool startsDug;

        [Header("Sonidos")]
        [SerializeField] private AudioCue digCue;
        [SerializeField] private AudioCue plantCue;
        [SerializeField] private AudioCue waterCue;
        [SerializeField] private AudioCue stageCue;
        [SerializeField] private AudioCue grownCue;
        [SerializeField] private AudioCue eatenCue;

        private int diggings;
        private int stagesDone;
        private int waterings;
        private float stageProgress;

        public PlotState State { get; private set; } = PlotState.Empty;
        public CropType Crop { get; private set; }

        public int DiggingsRequired => diggingsRequired;
        public int DiggingsLeft => State == PlotState.Empty ? Mathf.Max(0, diggingsRequired - diggings) : 0;
        public int Waterings => waterings;
        public int StagesTotal => Mathf.Max(1, wateringsRequired);
        public int StagesDone => stagesDone;
        public float StageProgress => stageProgress;
        public float Growth01 => State == PlotState.Grown ? 1f : Mathf.Clamp01((stagesDone + stageProgress) / StagesTotal);

        public bool IsGrown => State == PlotState.Grown;
        public bool IsVulnerable => State == PlotState.NeedsWater || State == PlotState.Growing;
        public bool HasPlant => State == PlotState.Grown || State == PlotState.Growing || (State == PlotState.NeedsWater && stagesDone > 0);
        public bool CanHighlight => State != PlotState.Grown;

        // El tipo se deduce de los parametros, asi no hay que mantener un dato mas en cada prefab.
        public PlotKind Kind
        {
            get
            {
                if (wateringsRequired <= 0)
                {
                    return PlotKind.Spring;
                }

                if (diggingsRequired > 1)
                {
                    return PlotKind.Stony;
                }

                return wateringsRequired == 1 ? PlotKind.Fertile : PlotKind.Normal;
            }
        }

        public string Label
        {
            get
            {
                switch (Kind)
                {
                    case PlotKind.Stony: return "Parcela pedregosa";
                    case PlotKind.Fertile: return "Parcela fertil";
                    case PlotKind.Spring: return "Manantial";
                    default: return "Parcela";
                }
            }
        }

        public string HighlightText => $"{Label}\n{KindDescription()}\n{StepHint()}";

        public event Action<PlantingPlot> Changed;
        public event Action<PlantingPlot, PlotAction> ActionPerformed;
        public event Action<PlantingPlot> BecameGrown;

        private string KindDescription()
        {
            switch (Kind)
            {
                case PlotKind.Stony: return $"Hay que quitar las piedras: {diggingsRequired} golpes de azada";
                case PlotKind.Fertile: return "Tierra rica: crece rapido con un solo riego";
                case PlotKind.Spring: return "Tiene agua propia: crece sola, solo hay que sembrarla";
                default: return $"Necesita {wateringsRequired} riegos para crecer";
            }
        }

        private string StepHint()
        {
            switch (State)
            {
                case PlotState.Empty: return DiggingsLeft > 1 ? $"Cavar con la azada ({DiggingsLeft} golpes)" : "Cavar con la azada";
                case PlotState.Dug: return "Lanzar semillas";
                case PlotState.NeedsWater: return "Regar con el anfora";
                case PlotState.Growing: return "Creciendo...";
                default: return string.Empty;
            }
        }

        private void Awake()
        {
            if (startsDug)
            {
                State = PlotState.Dug;
            }
        }

        public bool TryDig()
        {
            if (State != PlotState.Empty)
            {
                return false;
            }

            diggings++;
            digCue.Play(transform.position);

            if (diggings >= diggingsRequired)
            {
                SetState(PlotState.Dug);
            }
            else
            {
                Changed?.Invoke(this);
            }

            ActionPerformed?.Invoke(this, PlotAction.Dug);
            return true;
        }

        public bool TryPlant(CropType crop, bool instantGrowth)
        {
            if (State != PlotState.Dug)
            {
                return false;
            }

            Crop = crop;
            stagesDone = 0;
            waterings = 0;
            stageProgress = 0f;
            plantCue.Play(transform.position);
            ActionPerformed?.Invoke(this, PlotAction.Planted);

            if (instantGrowth)
            {
                Grow();
            }
            else if (wateringsRequired <= 0)
            {
                SetState(PlotState.Growing);
            }
            else
            {
                SetState(PlotState.NeedsWater);
            }

            return true;
        }

        public bool TryWater()
        {
            if (State != PlotState.NeedsWater)
            {
                return false;
            }

            waterings++;
            stageProgress = 0f;
            waterCue.Play(transform.position);
            ActionPerformed?.Invoke(this, PlotAction.Watered);
            SetState(PlotState.Growing);
            return true;
        }

        // Un cuervo se come lo plantado: la parcela vuelve a quedar cavada y se pierde la semilla.
        public bool TryEat()
        {
            if (!IsVulnerable)
            {
                return false;
            }

            stagesDone = 0;
            waterings = 0;
            stageProgress = 0f;
            eatenCue.Play(transform.position);
            ActionPerformed?.Invoke(this, PlotAction.Eaten);
            SetState(PlotState.Dug);
            return true;
        }

        private void Update()
        {
            if (State != PlotState.Growing)
            {
                return;
            }

            stageProgress += Time.deltaTime / Mathf.Max(0.01f, stageTime);
            if (stageProgress < 1f)
            {
                return;
            }

            stageProgress = 0f;
            stagesDone++;

            if (stagesDone >= StagesTotal)
            {
                Grow();
                return;
            }

            stageCue.Play(transform.position);
            ActionPerformed?.Invoke(this, PlotAction.StageGrown);
            SetState(PlotState.NeedsWater);
        }

        private void Grow()
        {
            stagesDone = StagesTotal;
            stageProgress = 0f;
            grownCue.Play(transform.position);
            ActionPerformed?.Invoke(this, PlotAction.Grown);
            SetState(PlotState.Grown);
            BecameGrown?.Invoke(this);
        }

        private void SetState(PlotState state)
        {
            State = state;
            Changed?.Invoke(this);
        }
    }
}
