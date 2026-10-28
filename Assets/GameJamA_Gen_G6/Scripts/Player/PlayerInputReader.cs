using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam
{
    /// <summary>
    /// Unico lugar donde se lee el teclado y el mouse.
    /// Los demas scripts del jugador piden datos aqui y nunca leen teclas directamente.
    /// Tiene dos tipos de datos:
    ///  - Propiedades (Move, LookDelta...): se consultan cada frame.
    ///  - Eventos (AttackPerformed...): avisan una sola vez cuando se pulsa una tecla.
    /// </summary>
    public class PlayerInputReader : MonoBehaviour
    {
        // Archivo InputSystem_Actions del proyecto. Se asigna automaticamente al crear el prefab.
        [SerializeField] private InputActionAsset actionsAsset;
        [SerializeField] private string mapName = "Player";

        // Copia en memoria del asset, para no modificar el archivo original al jugar.
        private InputActionAsset runtimeAsset;

        // Acciones que vienen del asset.
        private InputAction move;
        private InputAction look;
        private InputAction sprint;
        private InputAction crouch;
        private InputAction attack;
        private InputAction interact;
        private InputAction jump;

        // Acciones creadas por codigo, para no editar el archivo compartido del asset.
        private InputAction drop;
        private InputAction slotSelect;
        private InputAction slotScroll;
        private InputAction pause;

        // Valores que se consultan cada frame.
        public Vector2 Move => move.ReadValue<Vector2>();
        public Vector2 LookDelta => look.ReadValue<Vector2>();
        public bool SprintHeld => sprint.IsPressed();
        public bool CrouchHeld => crouch.IsPressed();

        // Eventos que se disparan al pulsar.
        public event Action AttackPerformed;
        public event Action InteractPerformed;
        public event Action JumpPerformed;
        public event Action DropPerformed;
        public event Action PausePerformed;

        // Se dispara con las teclas 1 a 9. El valor es el indice del slot (0 a 8).
        public event Action<int> SlotSelected;

        // Se dispara con la rueda del mouse. El valor es -1 (anterior) o 1 (siguiente).
        public event Action<int> SlotCycled;

        private void Awake()
        {
            runtimeAsset = Instantiate(actionsAsset);
            InputActionMap map = runtimeAsset.FindActionMap(mapName, true);
            move = map.FindAction("Move", true);
            look = map.FindAction("Look", true);
            sprint = map.FindAction("Sprint", true);
            crouch = map.FindAction("Crouch", true);
            attack = map.FindAction("Attack", true);
            interact = map.FindAction("Interact", true);
            jump = map.FindAction("Jump", true);

            drop = new InputAction("Drop", InputActionType.Button, "<Keyboard>/g");
            pause = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape");
            slotScroll = new InputAction("SlotScroll", InputActionType.PassThrough, "<Mouse>/scroll/y");
            slotSelect = new InputAction("SlotSelect", InputActionType.Button);
            for (int i = 1; i <= 9; i++)
            {
                slotSelect.AddBinding($"<Keyboard>/{i}");
            }
        }

        private void OnEnable()
        {
            runtimeAsset.FindActionMap(mapName, true).Enable();
            drop.Enable();
            pause.Enable();
            slotScroll.Enable();
            slotSelect.Enable();

            attack.performed += HandleAttack;
            interact.performed += HandleInteract;
            jump.performed += HandleJump;
            drop.performed += HandleDrop;
            pause.performed += HandlePause;
            slotScroll.performed += HandleScroll;
            slotSelect.performed += HandleSlotSelect;
        }

        private void OnDisable()
        {
            attack.performed -= HandleAttack;
            interact.performed -= HandleInteract;
            jump.performed -= HandleJump;
            drop.performed -= HandleDrop;
            pause.performed -= HandlePause;
            slotScroll.performed -= HandleScroll;
            slotSelect.performed -= HandleSlotSelect;

            runtimeAsset.FindActionMap(mapName, true).Disable();
            drop.Disable();
            pause.Disable();
            slotScroll.Disable();
            slotSelect.Disable();
        }

        private void OnDestroy()
        {
            drop?.Dispose();
            pause?.Dispose();
            slotScroll?.Dispose();
            slotSelect?.Dispose();
            if (runtimeAsset != null)
            {
                Destroy(runtimeAsset);
            }
        }

        private void HandleAttack(InputAction.CallbackContext context) => AttackPerformed?.Invoke();
        private void HandleInteract(InputAction.CallbackContext context) => InteractPerformed?.Invoke();
        private void HandleJump(InputAction.CallbackContext context) => JumpPerformed?.Invoke();
        private void HandleDrop(InputAction.CallbackContext context) => DropPerformed?.Invoke();
        private void HandlePause(InputAction.CallbackContext context) => PausePerformed?.Invoke();

        private void HandleScroll(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            if (Mathf.Approximately(value, 0f))
            {
                return;
            }

            // Rueda hacia arriba = slot anterior, hacia abajo = siguiente.
            SlotCycled?.Invoke(value > 0f ? -1 : 1);
        }

        private void HandleSlotSelect(InputAction.CallbackContext context)
        {
            // El nombre de la tecla es "1", "2"... y se convierte en indice (0, 1...).
            if (int.TryParse(context.control.name, out int number))
            {
                SlotSelected?.Invoke(number - 1);
            }
        }
    }
}
