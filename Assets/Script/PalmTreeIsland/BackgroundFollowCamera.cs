using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFollowCamera : MonoBehaviour {
    [SerializeField] private Camera target;
    [SerializeField] private float followSpeed = 500f;

    private void Update() {
        var targetPosition = target.transform.position;
        //var dynamicXOffset = target.IsFacingRight ? xOffset : -xOffset;
        var newPos = new Vector3(targetPosition.x, targetPosition.y, 1);
        transform.position = Vector3.Slerp(transform.position, newPos, followSpeed * Time.deltaTime);
    }
}