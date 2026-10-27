using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameJam.EditorTools
{
    /// <summary>
    /// Arma el prototipo de caza en la escena abierta: suelo, jugador, lanza, animales y objetivo.
    /// Solo existe en el editor; no entra en el juego final.
    /// </summary>
    public static class PrehistoriaSetup
    {
        private const string MaterialsFolder = "Assets/GameJamA_Gen_G6/Materials";

        private static readonly Vector3[] AnimalPositions =
        {
            new Vector3(-10f, 0.1f, 12f),
            new Vector3(0f, 0.1f, 16f),
            new Vector3(10f, 0.1f, 12f),
            new Vector3(6f, 0.1f, 22f)
        };

        [MenuItem("GameJam/Prehistoria/Add Hunt Prototype")]
        public static void AddHuntPrototype()
        {
            if (Object.FindFirstObjectByType<HuntGoal>() != null)
            {
                Debug.LogWarning("La escena ya tiene el prototipo de caza.");
                return;
            }

            if (Object.FindFirstObjectByType<PlayerMovement>() == null)
            {
                PlayerSetup.AddPlayerToOpenScene();
            }

            EnsureGround();

            Material spearMaterial = GetOrCreateMaterial("Prehistoria_Spear", new Color(0.55f, 0.38f, 0.2f));
            Material animalMaterial = GetOrCreateMaterial("Prehistoria_Animal", new Color(0.45f, 0.3f, 0.18f));

            CreateSpear(new Vector3(1f, 0.4f, -1.5f), spearMaterial);

            for (int i = 0; i < AnimalPositions.Length; i++)
            {
                CreateAnimal(i + 1, AnimalPositions[i], animalMaterial);
            }

            GameObject goal = new GameObject("HuntGoal");
            goal.AddComponent<HuntGoal>();
            goal.AddComponent<HuntHud>();
            Undo.RegisterCreatedObjectUndo(goal, "Add Hunt Goal");

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("GameJam/Prehistoria/Add Extra Spear")]
        public static void AddExtraSpear()
        {
            PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
            Vector3 origin = player != null ? player.transform.position : Vector3.zero;
            Vector3 forward = player != null ? player.transform.forward : Vector3.forward;

            CreateSpear(origin + forward * 1.5f + Vector3.up * 0.5f, GetOrCreateMaterial("Prehistoria_Spear", new Color(0.55f, 0.38f, 0.2f)));
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static void EnsureGround()
        {
            if (GameObject.Find("Ground") != null || GameObject.Find("Plane") != null)
            {
                return;
            }

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(6f, 1f, 6f);
            Undo.RegisterCreatedObjectUndo(ground, "Add Ground");
        }

        private static void CreateSpear(Vector3 position, Material material)
        {
            GameObject root = new GameObject("Spear");
            root.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 30f, 0f));

            Rigidbody body = root.AddComponent<Rigidbody>();
            body.mass = 1f;

            BoxCollider box = root.AddComponent<BoxCollider>();
            box.size = new Vector3(0.25f, 0.25f, 1.7f);

            GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shaft.name = "Shaft";
            shaft.transform.SetParent(root.transform, false);
            shaft.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            shaft.transform.localScale = new Vector3(0.04f, 0.8f, 0.04f);
            Object.DestroyImmediate(shaft.GetComponent<Collider>());
            ApplyMaterial(shaft, material);

            GameObject tip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tip.name = "Tip";
            tip.transform.SetParent(root.transform, false);
            tip.transform.localPosition = new Vector3(0f, 0f, 0.85f);
            tip.transform.localScale = new Vector3(0.08f, 0.08f, 0.22f);
            Object.DestroyImmediate(tip.GetComponent<Collider>());

            Spear spear = root.AddComponent<Spear>();
            SetString(spear, "displayName", "Lanza");

            Undo.RegisterCreatedObjectUndo(root, "Add Spear");
        }

        private static void CreateAnimal(int index, Vector3 position, Material material)
        {
            GameObject root = new GameObject($"Animal_{index}");
            root.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 180f, 0f));

            CharacterController controller = root.AddComponent<CharacterController>();
            controller.height = 0.9f;
            controller.radius = 0.35f;
            controller.center = new Vector3(0f, 0.45f, 0f);

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            body.transform.localScale = new Vector3(0.55f, 0.5f, 1f);
            Object.DestroyImmediate(body.GetComponent<Collider>());
            ApplyMaterial(body, material);

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "Head";
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = new Vector3(0f, 0.65f, 0.6f);
            head.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            Object.DestroyImmediate(head.GetComponent<Collider>());
            ApplyMaterial(head, material);

            root.AddComponent<Animal>();
            Undo.RegisterCreatedObjectUndo(root, "Add Animal");
        }

        private static void ApplyMaterial(GameObject target, Material material)
        {
            if (material != null)
            {
                target.GetComponent<Renderer>().sharedMaterial = material;
            }
        }

        private static Material GetOrCreateMaterial(string name, Color color)
        {
            string path = $"{MaterialsFolder}/{name}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
            {
                return material;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                return null;
            }

            material = new Material(shader);
            material.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void SetString(Object target, string field, string value)
        {
            SerializedObject serialized = new SerializedObject(target);
            serialized.FindProperty(field).stringValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
