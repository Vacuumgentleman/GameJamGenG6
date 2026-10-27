using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Se encarga de lo fisico del inventario: pone el objeto equipado en la mano (HoldPoint),
    /// esconde los demas, ejecuta el clic izquierdo sobre el objeto equipado y lo suelta con G.
    /// No sabe que objetos existen: solo habla con las interfaces IPickable e IUsable.
    /// </summary>
    public class PlayerCarrier : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerAim aim;
        [SerializeField] private PlayerInventory inventory;

        // Posicion en la mano, hija de la camara.
        [SerializeField] private Transform holdPoint;

        // Distancia delante de la camara donde aparece un objeto soltado.
        [SerializeField] private float dropDistance = 0.8f;

        // Impulso con el que sale un objeto soltado.
        [SerializeField] private float dropSpeed = 2.5f;

        // Objeto que esta en la mano ahora mismo.
        private IPickable current;

        private void OnEnable()
        {
            input.AttackPerformed += HandleAttack;
            input.DropPerformed += DropSelected;
            inventory.ItemAdded += HandleItemAdded;
            inventory.ItemRemoved += HandleItemRemoved;
            inventory.SelectionChanged += HandleSelectionChanged;
        }

        private void OnDisable()
        {
            input.AttackPerformed -= HandleAttack;
            input.DropPerformed -= DropSelected;
            inventory.ItemAdded -= HandleItemAdded;
            inventory.ItemRemoved -= HandleItemRemoved;
            inventory.SelectionChanged -= HandleSelectionChanged;
        }

        // Recoge un objeto. Si el inventario esta lleno, suelta antes el objeto de la mano.
        public bool Pickup(IPickable item)
        {
            if (item == null)
            {
                return false;
            }

            if (inventory.IsFull)
            {
                DropSelected();
            }

            return inventory.TryAdd(item);
        }

        // Suelta el objeto del slot seleccionado delante de la camara.
        public void DropSelected()
        {
            IPickable item = inventory.Selected;
            if (item == null)
            {
                return;
            }

            Unequip();
            inventory.Remove(inventory.SelectedIndex);

            Ray ray = aim.GetRay();
            Vector3 position = ray.origin + ray.direction * dropDistance;
            item.OnDropped(position, Quaternion.LookRotation(ray.direction), ray.direction * dropSpeed);
        }

        // Clic izquierdo: usa el objeto equipado si es usable.
        private void HandleAttack()
        {
            if (current is not IUsable usable)
            {
                return;
            }

            UseResult result = usable.Use(new UseContext(aim.GetRay(), gameObject));

            // Si el objeto se consume (lanza lanzada), sale del inventario y se encarga de si mismo.
            if (result == UseResult.Consumed)
            {
                current = null;
                inventory.Remove(inventory.SelectedIndex);
            }
        }

        // Un objeto entro al inventario: se pone como hijo de la mano y se oculta hasta que se equipe.
        private void HandleItemAdded(int slot, IPickable item)
        {
            item.OnPickedUp();
            Transform itemTransform = item.GameObject.transform;
            itemTransform.SetParent(holdPoint, false);
            itemTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            item.GameObject.SetActive(false);
        }

        // Un objeto salio del inventario: deja de ser hijo de la mano.
        private void HandleItemRemoved(int slot, IPickable item)
        {
            if (ReferenceEquals(current, item))
            {
                current = null;
            }

            Transform itemTransform = item.GameObject.transform;
            if (itemTransform.parent == holdPoint)
            {
                itemTransform.SetParent(null, true);
            }
        }

        // Cambio de slot: se guarda el objeto anterior y se saca el nuevo.
        private void HandleSelectionChanged(int index)
        {
            Unequip();

            IPickable item = inventory.Selected;
            if (item == null)
            {
                return;
            }

            item.GameObject.SetActive(true);
            current = item;
            if (item is IUsable usable)
            {
                usable.OnEquipped(holdPoint);
            }
        }

        // Guarda el objeto de la mano: lo avisa y lo oculta.
        private void Unequip()
        {
            if (current == null)
            {
                return;
            }

            if (current is IUsable usable)
            {
                usable.OnUnequipped();
            }

            current.GameObject.SetActive(false);
            current = null;
        }
    }
}
