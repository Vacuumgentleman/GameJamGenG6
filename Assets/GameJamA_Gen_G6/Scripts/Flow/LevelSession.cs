using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    public class LevelSession : MonoBehaviour
    {
        public enum SessionState
        {
            Running,
            Completed,
            Expired
        }

        private readonly List<ILevelGoal> goals = new List<ILevelGoal>();

        private LevelCatalog catalog;
        private LevelDefinition level;
        private LevelTimer timer;

        public SessionState State { get; private set; }
        public LevelDefinition Level => level;
        public LevelTimer Timer => timer;

        public event Action<SessionState> StateChanged;

        public void Initialize(LevelCatalog sourceCatalog, LevelDefinition sourceLevel)
        {
            catalog = sourceCatalog;
            level = sourceLevel;
        }

        private void Start()
        {
            timer = FindFirstObjectByType<LevelTimer>();
            if (timer == null)
            {
                timer = gameObject.AddComponent<LevelTimer>();
            }

            timer.Configure(catalog.LevelDuration);
            timer.Expired += HandleExpired;

            foreach (MonoBehaviour behaviour in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                if (behaviour is ILevelGoal goal)
                {
                    goals.Add(goal);
                    goal.Completed += HandleGoalCompleted;
                }
            }

            if (goals.Count == 0)
            {
                Debug.LogWarning($"El nivel '{level.Id}' no tiene ningun ILevelGoal: nunca se marcara como completado.");
            }

            gameObject.AddComponent<LevelSessionHud>();
        }

        private void OnDestroy()
        {
            if (timer != null)
            {
                timer.Expired -= HandleExpired;
            }

            foreach (ILevelGoal goal in goals)
            {
                goal.Completed -= HandleGoalCompleted;
            }
        }

        private void HandleGoalCompleted()
        {
            if (State != SessionState.Running)
            {
                return;
            }

            foreach (ILevelGoal goal in goals)
            {
                if (!goal.IsCompleted)
                {
                    return;
                }
            }

            timer.Stop();
            GameProgress.MarkCompleted(level.Id);
            Finish(SessionState.Completed, catalog.CompletedDelay);
        }

        private void HandleExpired()
        {
            if (State != SessionState.Running)
            {
                return;
            }

            Finish(SessionState.Expired, catalog.ExpiredDelay);
        }

        private void Finish(SessionState state, float delay)
        {
            State = state;
            StateChanged?.Invoke(state);
            StartCoroutine(ReturnToHub(delay));
        }

        private IEnumerator ReturnToHub(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            GameFlow.LoadHub();
        }
    }
}
