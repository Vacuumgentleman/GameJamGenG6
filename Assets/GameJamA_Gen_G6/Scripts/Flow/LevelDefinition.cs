using System;
using UnityEngine;

namespace GameJam
{
    [Serializable]
    public class LevelDefinition
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string sceneName;

        public string Id => id;
        public string DisplayName => displayName;
        public string SceneName => sceneName;
    }
}
