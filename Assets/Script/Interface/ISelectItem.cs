using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectItem {
    public event EventHandler<OnItemSelectedEventArgs> OnItemSelected;
    public class OnItemSelectedEventArgs : EventArgs {
        public Item item;
    }
}
