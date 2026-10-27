using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameJam.EditorTools
{
    /// <summary>
    /// Atajos del menu "GameJam" para preparar una escena de prueba.
    /// Solo existen en el editor; no entran en el juego final.
    /// </summary>
    public static class PlayerSetup
    {
        private const string PrefabPath = "Assets/GameJamA_Gen_G6/Prefabs/Player/Player.prefab";

        // Coloca el prefab del jugador en la escena abierta y desactiva las demas camaras.
        [MenuItem("GameJam/Add Player To Open Scene")]
        public static void AddPlayerToOpenScene()
        {
            if (Object.FindFirstObjectByType<PlayerMovement>() != null)
            {
                Debug.LogWarning("La escena ya tiene un jugador.");
                return;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null)
            {
                Debug.LogError($"No se encontro el prefab en {PrefabPath}");
                return;
            }

            GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(player, "Add Player");
            player.transform.position = new Vector3(0f, 0.1f, -3f);

            // Evita tener dos camaras y dos AudioListener a la vez.
            foreach (Camera sceneCamera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
            {
                if (sceneCamera.transform.IsChildOf(player.transform))
                {
                    continue;
                }

                Undo.RecordObject(sceneCamera.gameObject, "Disable scene camera");
                sceneCamera.gameObject.SetActive(false);
            }

            EditorSceneManager.MarkSceneDirty(player.scene);
            Selection.activeGameObject = player;
        }

        // Crea tres cilindros recogibles delante del jugador (o cerca del origen si no hay jugador).
        [MenuItem("GameJam/Add Test Items To Open Scene")]
        public static void AddTestItemsToOpenScene()
        {
            PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
            Vector3 origin = player != null ? player.transform.position : new Vector3(0f, 0f, -3f);
            Vector3 forward = player != null ? player.transform.forward : Vector3.forward;
            Vector3 right = player != null ? player.transform.right : Vector3.right;

            Vector3[] offsets =
            {
                forward * 2f - right,
                forward * 2f,
                forward * 2.6f + right
            };

            for (int i = 0; i < offsets.Length; i++)
            {
                GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                item.name = $"TestItem_{i + 1}";
                item.transform.position = origin + offsets[i] + Vector3.up * 0.4f;
                item.transform.localScale = new Vector3(0.12f, 0.3f, 0.12f);
                item.AddComponent<Rigidbody>();
                PlaceholderItem placeholder = item.AddComponent<PlaceholderItem>();
                SetString(placeholder, "displayName", $"Objeto {i + 1}");
                Undo.RegisterCreatedObjectUndo(item, "Add Test Item");
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // Asigna un texto a un campo privado marcado con [SerializeField].
        private static void SetString(Object target, string field, string value)
        {
            SerializedObject serialized = new SerializedObject(target);
            serialized.FindProperty(field).stringValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
