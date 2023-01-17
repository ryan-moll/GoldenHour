using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float cameraOffsetX = 4.5f;
    [SerializeField] private float cameraOffsetY = 1f;

    private void Update()
    {
        transform.position = new Vector3(player.position.x + cameraOffsetX, player.position.y + cameraOffsetY, transform.position.z);
    }
}
