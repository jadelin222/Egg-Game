using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class EggBehavior : MonoBehaviour
{
    public Rigidbody rb;
    public AudioSource cookingSound;
    public Material eggMaterial;
    public Transform spawnPoint; 
    public float idleRotationSpeed = 20f;
    public float fadeSpeed = 2f; //egg transparency
    public float cookingThreshold = 0.2f; // velocity threshold to be considered sliding
    //public float fadeOutSpeed = .5f;
    public TextMeshProUGUI cookLevelTxt;
    public TextMeshProUGUI eggWastedTxt;
    public TextMeshProUGUI winScreenTxt;
    public GameObject WinUI;
    public GameObject gameUI;

    public bool isFloating = true;

    private bool hasLanded = false;
    private bool isCooking = false;
    private bool isOffPan = false;
    private bool hasSoundPlayed = false;
    private float cookLevel = 0f;
    private int eggWasted = 0;

    private void Start()
    {
        ResetEgg();
    }

    void Update()
    {
        if (isFloating)
        {
            //stay at spawn point position while floating
            transform.position = Vector3.Lerp(transform.position, spawnPoint.position, Time.deltaTime * 2f);

            //random smooth idle rotation
            transform.Rotate(Vector3.up, idleRotationSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, Mathf.Sin(Time.time) * idleRotationSpeed * Time.deltaTime);
        }

        if (hasLanded && Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) > cookingThreshold)
            isCooking = true;
        else
            isCooking = false;


        if (isCooking)
        {        
            Color color = eggMaterial.color;
            color.a = Mathf.MoveTowards(color.a, 1f, Time.deltaTime * fadeSpeed);
            eggMaterial.color = color;
            cookLevel = (color.a / 1f)*100;

            if (!hasSoundPlayed)
            {
                cookingSound.Play();
                hasSoundPlayed = true;
            }

        }
        else
        {
            if (hasSoundPlayed)
            {
                cookingSound.Stop();
                hasSoundPlayed = false;
            }
            
        }

        if (isOffPan) 
        {
            cookingSound.Stop();
            hasSoundPlayed = false;
            LoseScore();
            RespawnEgg();
        }
        cookLevelTxt.text = $"Cooked: {cookLevel:F0}%";
        if(cookLevel >= 100)
        {
            Win();
        }
        //mouse click: drop or respawn
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (isFloating)
        //    {
        //        DropEgg();
        //    }
        //    else
        //    {
        //        RespawnEgg();
        //    }
        //}
    }

    public void DropEgg()
    {
        isFloating = false;
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    private void RespawnEgg()
    {
        // reset physics and position to spawn point
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        hasLanded = false;
        isFloating = true;
        isCooking = false;
        isOffPan = false;
        hasSoundPlayed = false;
        cookLevel = 0;


        // reset alpha
        Color color = eggMaterial.color;
        color.a = 0f;
        eggMaterial.color = color;

        // set position to spawn point
        transform.position = spawnPoint.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (!hasLanded && collision.gameObject.CompareTag("Pan"))
        {
            //hitSound.Play();
            hasLanded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (hasLanded && collision.gameObject.CompareTag("Pan"))
        {
            isOffPan = true;
        }
    }
    private void LoseScore()
    {
        eggWasted += 1;
        eggWastedTxt.text = $"Eggs Wasted: {eggWasted}";
        //Debug.Log("Egg fell off the pan! -1 Score");
        
    }
    public bool Win()
    {
        WinUI.SetActive(true);
        gameUI.SetActive(false);
        winScreenTxt.text = $"After {eggWasted} tries...";
        return true;    
    }
    //call this from spawner or start
    public void ResetEgg()
    {
        RespawnEgg();
    }
}
