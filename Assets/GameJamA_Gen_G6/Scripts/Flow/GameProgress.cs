using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    public static class GameProgress
    {
        private static readonly HashSet<string> completed = new HashSet<string>();

        public static int CompletedCount => completed.Count;

        public static event Action Changed;

        public static bool IsCompleted(string levelId)
        {
            return completed.Contains(levelId);
        }

        public static void MarkCompleted(string levelId)
        {
            if (completed.Add(levelId))
            {
                Changed?.Invoke();
            }
        }

        public static void Reset()
        {
            completed.Clear();
            Changed?.Invoke();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearOnPlay()
        {
            completed.Clear();
            Changed = null;
        }
    }
}
