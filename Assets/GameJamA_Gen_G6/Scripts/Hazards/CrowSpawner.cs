using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Genera cuervos de vez en cuando hacia parcelas sembradas que aun no han crecido.
    /// Se detiene cuando el LevelTimer deja de correr (nivel completado o tiempo agotado).
    /// </summary>
    public class CrowSpawner : MonoBehaviour
    {
        [SerializeField] private Crow crowPrefab;

        // Segundos antes del primer cuervo.
        [SerializeField] private float firstDelay = 15f;

        // Rango (min, max) de segundos entre cuervos.
        [SerializeField] private Vector2 interval = new Vector2(8f, 14f);
        [SerializeField] private int maxCrows = 3;

        // Prefiere parcelas al menos a esta distancia del jugador, para que el cuervo llegue a aterrizar.
        [SerializeField] private float preferredPlayerDistance = 14f;

        // Distancia y altura desde la parcela a la que aparece el cuervo.
        [SerializeField] private float spawnDistance = 50f;
        [SerializeField] private float spawnHeight = 16f;

        private readonly List<Crow> crows = new List<Crow>();
        private readonly List<PlantingPlot> candidates = new List<PlantingPlot>();
        private readonly List<PlantingPlot> farCandidates = new List<PlantingPlot>();
        private PlantingPlot[] plots;
        private LevelTimer timer;
        private Transform player;
        private float nextSpawn;

        private void Start()
        {
            plots = FindObjectsByType<PlantingPlot>(FindObjectsSortMode.None);
            timer = FindFirstObjectByType<LevelTimer>();
            PlayerMovement found = FindFirstObjectByType<PlayerMovement>();
            player = found != null ? found.transform : null;
            nextSpawn = Time.time + firstDelay;
        }

        private void Update()
        {
            if (crowPrefab == null || Time.time < nextSpawn)
            {
                return;
            }

            if (timer != null && !timer.IsRunning)
            {
                return;
            }

            nextSpawn = Time.time + Random.Range(interval.x, interval.y);
            crows.RemoveAll(crow => crow == null);
            if (crows.Count >= maxCrows)
            {
                return;
            }

            PlantingPlot target = PickTarget();
            if (target != null)
            {
                Spawn(target);
            }
        }

        private PlantingPlot PickTarget()
        {
            candidates.Clear();
            farCandidates.Clear();
            float minSqr = preferredPlayerDistance * preferredPlayerDistance;

            foreach (PlantingPlot plot in plots)
            {
                if (!plot.IsVulnerable || IsTargeted(plot))
                {
                    continue;
                }

                candidates.Add(plot);
                if (player == null || (plot.transform.position - player.position).sqrMagnitude >= minSqr)
                {
                    farCandidates.Add(plot);
                }
            }

            List<PlantingPlot> pool = farCandidates.Count > 0 ? farCandidates : candidates;
            return pool.Count > 0 ? pool[Random.Range(0, pool.Count)] : null;
        }

        private bool IsTargeted(PlantingPlot plot)
        {
            foreach (Crow crow in crows)
            {
                if (crow != null && crow.Plot == plot)
                {
                    return true;
                }
            }

            return false;
        }

        private void Spawn(PlantingPlot target)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            Vector3 position = target.transform.position + new Vector3(direction.x, 0f, direction.y) * spawnDistance + Vector3.up * spawnHeight;

            Crow crow = Instantiate(crowPrefab, position, Quaternion.identity);
            crow.Initialize(target);
            crows.Add(crow);
        }
    }
}
