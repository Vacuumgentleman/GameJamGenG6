using System;
using UnityEngine;

namespace GameJam
{
    [CreateAssetMenu(menuName = "GameJam/Level Catalog", fileName = "LevelCatalog")]
    public class LevelCatalog : ScriptableObject
    {
        private const string ResourcePath = "LevelCatalog";

        [SerializeField] private string hubScene = "Era_5_Futuro";
        [SerializeField] private float levelDuration = 180f;
        [SerializeField] private float completedDelay = 3f;
        [SerializeField] private float expiredDelay = 2.5f;
        [SerializeField] private LevelDefinition[] levels = Array.Empty<LevelDefinition>();

        public string HubScene => hubScene;
        public float LevelDuration => levelDuration;
        public float CompletedDelay => completedDelay;
        public float ExpiredDelay => expiredDelay;
        public int Count => levels.Length;

        public static LevelCatalog Load()
        {
            return Resources.Load<LevelCatalog>(ResourcePath);
        }

        public LevelDefinition this[int index] => levels[index];

        public bool TryGetById(string id, out LevelDefinition level)
        {
            return TryFind(candidate => candidate.Id == id, out level);
        }

        public bool TryGetByScene(string sceneName, out LevelDefinition level)
        {
            return TryFind(candidate => candidate.SceneName == sceneName, out level);
        }

        private bool TryFind(Predicate<LevelDefinition> match, out LevelDefinition level)
        {
            foreach (LevelDefinition candidate in levels)
            {
                if (candidate != null && match(candidate))
                {
                    level = candidate;
                    return true;
                }
            }

            level = null;
            return false;
        }
    }
}
