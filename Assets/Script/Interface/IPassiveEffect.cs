using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPassiveEffect
{
    void ApplyEffect(Player player);
    void RemoveEffect(Player player);
}
