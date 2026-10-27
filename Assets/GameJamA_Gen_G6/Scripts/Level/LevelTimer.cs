using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam
{
    /// <summary>
    /// Cuenta atras de un nivel. Al llegar a cero avisa con Expired y, si esta activado,
    /// reinicia la escena. Provisional: el sistema compartido de bucles puede reemplazarlo
    /// escuchando los mismos eventos.
    /// </summary>
    public class LevelTimer : MonoBehaviour
    {
        [SerializeField] private float duration = 180f;
        [SerializeField] private bool restartSceneOnExpire = true;
        [SerializeField] private float restartDelay = 2.5f;

        public float Duration => duration;
        public float Remaining { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsExpired { get; private set; }

        public event Action<float> TimeAdded;
        public event Action Expired;

        private void Start()
        {
            Remaining = duration;
            IsRunning = true;
        }

        private void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            Remaining -= Time.deltaTime;
            if (Remaining > 0f)
            {
                return;
            }

            Remaining = 0f;
            IsRunning = false;
            IsExpired = true;
            Expired?.Invoke();

            if (restartSceneOnExpire)
            {
                StartCoroutine(RestartAfterDelay());
            }
        }

        public void AddTime(float seconds)
        {
            if (!IsRunning)
            {
                return;
            }

            Remaining += seconds;
            TimeAdded?.Invoke(seconds);
        }

        public void Stop()
        {
            IsRunning = false;
        }

        private IEnumerator RestartAfterDelay()
        {
            yield return new WaitForSeconds(restartDelay);

            string sceneName = SceneManager.GetActiveScene().name;
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"No se puede reiniciar '{sceneName}': agregala a Build Profiles (Scene List).");
                yield break;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
