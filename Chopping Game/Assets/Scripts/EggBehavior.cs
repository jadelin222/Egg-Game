using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBehavior : MonoBehaviour
{
    public Rigidbody rb;
    public AudioSource hitSound;
    public Material eggMaterial;
    public Transform spawnPoint; // 📌 Assign this in the inspector
    public float idleRotationSpeed = 20f;
    public float fadeSpeed = 2f;

    private bool isFloating = true;
    private bool hasLanded = false;

    private void Start()
    {
        ResetEgg();
    }

    void Update()
    {
        if (isFloating)
        {
            // Stay at spawn point position while floating
            transform.position = Vector3.Lerp(transform.position, spawnPoint.position, Time.deltaTime * 2f);

            // Random smooth idle rotation
            transform.Rotate(Vector3.up, idleRotationSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, Mathf.Sin(Time.time) * idleRotationSpeed * Time.deltaTime);
        }

        // Smooth fade-in when egg lands
        if (hasLanded)
        {
            Color color = eggMaterial.color;
            color.a = Mathf.MoveTowards(color.a, 1f, Time.deltaTime * fadeSpeed);
            eggMaterial.color = color;
        }

        // Mouse click: Drop or Respawn
        if (Input.GetMouseButtonDown(0))
        {
            if (isFloating)
            {
                DropEgg();
            }
            else
            {
                RespawnEgg();
            }
        }
    }

    private void DropEgg()
    {
        isFloating = false;
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    private void RespawnEgg()
    {
        // Reset physics and position to spawn point
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        hasLanded = false;
        isFloating = true;

        // Reset alpha
        Color color = eggMaterial.color;
        color.a = 0f;
        eggMaterial.color = color;

        // Set position to spawn point
        transform.position = spawnPoint.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded && collision.gameObject.CompareTag("Pan"))
        {
            hitSound.Play();
            hasLanded = true;
        }
    }

    // Call this from spawner or start
    public void ResetEgg()
    {
        RespawnEgg();
    }
}
