using System;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Inventario de 5 espacios (slots). Solo guarda datos y avisa con eventos;
    /// no mueve objetos ni toca la escena (eso lo hace PlayerCarrier).
    /// Teclas 1 a 5 seleccionan un slot y la rueda del mouse recorre los ocupados.
    /// Un slot vacio seleccionado significa manos libres.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private int capacity = 5;

        private IPickable[] slots;
        private int selectedIndex;

        // Se avisa cuando entra un objeto, cuando sale y cuando cambia el slot seleccionado.
        public event Action<int, IPickable> ItemAdded;
        public event Action<int, IPickable> ItemRemoved;
        public event Action<int> SelectionChanged;

        public int Capacity => capacity;
        public int SelectedIndex => selectedIndex;

        // Objeto del slot seleccionado, o null si esta vacio.
        public IPickable Selected => slots[selectedIndex];

        public bool IsFull
        {
            get
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i] == null)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private void Awake()
        {
            slots = new IPickable[capacity];
        }

        private void OnEnable()
        {
            input.SlotSelected += Select;
            input.SlotCycled += Cycle;
        }

        private void OnDisable()
        {
            input.SlotSelected -= Select;
            input.SlotCycled -= Cycle;
        }

        public IPickable GetItem(int index)
        {
            return slots[index];
        }

        // Guarda el objeto en el primer slot libre y lo deja seleccionado.
        public bool TryAdd(IPickable item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                {
                    continue;
                }

                slots[i] = item;
                ItemAdded?.Invoke(i, item);
                Apply(i);
                return true;
            }

            return false;
        }

        // Saca el objeto del slot. El slot seleccionado no cambia, solo queda vacio.
        public IPickable Remove(int index)
        {
            IPickable item = slots[index];
            if (item == null)
            {
                return null;
            }

            slots[index] = null;
            ItemRemoved?.Invoke(index, item);
            return item;
        }

        // Teclas numericas: selecciona un slot, este vacio o no.
        public void Select(int index)
        {
            if (index < 0 || index >= capacity || index == selectedIndex)
            {
                return;
            }

            Apply(index);
        }

        // Rueda del mouse: salta al siguiente slot ocupado en la direccion dada (-1 o 1).
        public void Cycle(int direction)
        {
            for (int step = 1; step < capacity; step++)
            {
                int index = ((selectedIndex + direction * step) % capacity + capacity) % capacity;
                if (slots[index] != null)
                {
                    Apply(index);
                    return;
                }
            }
        }

        private void Apply(int index)
        {
            selectedIndex = index;
            SelectionChanged?.Invoke(index);
        }
    }
}
