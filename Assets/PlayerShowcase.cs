using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShowcase : MonoBehaviour
{
    public float rotationSpeed = 1.0f;

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, transform.position.z));
        float angle = Mathf.Atan2(worldPosition.x - transform.position.x, worldPosition.z - transform.position.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, angle, 0);
        angle *= rotationSpeed;

    }
}
