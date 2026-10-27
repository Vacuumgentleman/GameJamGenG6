using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Rafaga de particulas sin assets: crea un sistema temporal que se destruye solo al terminar.
    /// Sirve de placeholder; los VFX finales pueden reemplazar las llamadas a Play.
    /// </summary>
    public static class ParticleBurst
    {
        private static Material material;

        public static void Play(
            Vector3 position, Color color, int count = 14, float speed = 2.5f,
            float size = 0.1f, float gravity = 1f, float lifetime = 0.7f, float coneAngle = 35f)
        {
            GameObject go = new GameObject("ParticleBurst");
            go.transform.SetPositionAndRotation(position, Quaternion.Euler(-90f, 0f, 0f));

            ParticleSystem system = go.AddComponent<ParticleSystem>();
            system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            ParticleSystem.MainModule main = system.main;
            main.duration = lifetime;
            main.loop = false;
            main.playOnAwake = false;
            main.startLifetime = lifetime;
            main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.5f, speed);
            main.startSize = new ParticleSystem.MinMaxCurve(size * 0.6f, size);
            main.startColor = color;
            main.gravityModifier = gravity;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.stopAction = ParticleSystemStopAction.Destroy;

            ParticleSystem.EmissionModule emission = system.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

            ParticleSystem.ShapeModule shape = system.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = coneAngle;
            shape.radius = 0.15f;

            ParticleSystem.ColorOverLifetimeModule fade = system.colorOverLifetime;
            fade.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
            fade.color = gradient;

            ParticleSystem.SizeOverLifetimeModule shrink = system.sizeOverLifetime;
            shrink.enabled = true;
            shrink.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.2f));

            ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = GetMaterial();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;

            system.Play();
        }

        private static Material GetMaterial()
        {
            if (material == null)
            {
                material = new Material(Shader.Find("Sprites/Default"));
            }

            return material;
        }
    }
}
