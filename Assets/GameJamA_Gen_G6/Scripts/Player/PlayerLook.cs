using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Vista en primera persona con el mouse.
    /// Gira el cuerpo del jugador en horizontal (yaw) y solo la camara en vertical (pitch).
    /// Tambien bloquea el cursor; Escape lo libera o lo vuelve a bloquear.
    /// </summary>
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;

        // Objeto padre de la camara. Es el que se inclina arriba y abajo.
        [SerializeField] private Transform cameraRoot;

        // Grados que gira la vista por cada pixel que se mueve el mouse.
        [SerializeField] private float sensitivity = 0.12f;

        // Limite para no dar la vuelta completa mirando arriba o abajo.
        [SerializeField] private float maxPitch = 85f;
        [SerializeField] private bool invertY;
        [SerializeField] private bool lockCursorOnStart = true;

        private float pitch;

        public bool IsLocked => Cursor.lockState == CursorLockMode.Locked;

        private void OnEnable()
        {
            input.PausePerformed += HandlePause;
        }

        private void OnDisable()
        {
            input.PausePerformed -= HandlePause;
        }

        private void Start()
        {
            if (lockCursorOnStart)
            {
                SetLocked(true);
            }
        }

        private void Update()
        {
            // Si el cursor esta libre (menu o pausa), no se gira la vista.
            if (!IsLocked)
            {
                return;
            }

            Vector2 delta = input.LookDelta * sensitivity;

            // El cuerpo gira en horizontal; asi WASD siempre avanza hacia donde miras.
            transform.Rotate(Vector3.up, delta.x, Space.Self);

            // Solo la camara se inclina en vertical.
            pitch += invertY ? delta.y : -delta.y;
            pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
            cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        // Los menus (HUD de Kim) pueden llamar a este metodo para liberar o bloquear el cursor.
        public void SetLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        private void HandlePause()
        {
            SetLocked(!IsLocked);
        }
    }
}
