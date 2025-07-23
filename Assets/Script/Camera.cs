using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Player target;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float xOffset = 1f;
    [SerializeField] private float yOffset = 2f;
    private const float ZIndex = -10;

    private void Update()
    {
        var targetPosition = target.GetPosition();
        var dynamicXOffset = target.IsFacingRight ? xOffset : -xOffset;
        var newPos = new Vector3(targetPosition.x + dynamicXOffset, targetPosition.y + yOffset, ZIndex);
        transform.position = Vector3.Slerp(transform.position, newPos, followSpeed * Time.deltaTime);
    }
}