using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(Rigidbody))]
public class MagneticHoverboard : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float speed = 20f;
    public float steerSpeed = 100f; 
    
    [Header("Hover Ayarları")]
    public float hoverHeight = 2.0f;
    public float rotateSpeed = 10f; 
    public LayerMask groundLayer;

    private Rigidbody rb;
    private float currentInput;
    private float currentTurnInput;  

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; 
        rb.linearDamping = 1f; 
    }

    void Update()
    {
        float w = Keyboard.current.wKey.isPressed ? 1f : 0f;
        float s = Keyboard.current.sKey.isPressed ? 1f : 0f;
        currentInput = w - s;
        float d = Keyboard.current.dKey.isPressed ? 1f : 0f; 
        float a = Keyboard.current.aKey.isPressed ? 1f : 0f; 
        currentTurnInput = d - a; 
    }

    void FixedUpdate()
    {
        ApplyMagneticPhysics();
    }

    void ApplyMagneticPhysics()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, -transform.up, out hit, hoverHeight * 2, groundLayer))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);

            transform.Rotate(Vector3.up * currentTurnInput * steerSpeed * Time.fixedDeltaTime);

            float distanceError = hoverHeight - hit.distance;
            Vector3 hoverForce = transform.up * distanceError * 50f; 
            rb.AddForce(hoverForce);

            Vector3 surfaceForward = Vector3.Cross(transform.right, hit.normal);

            float slopeDot = Vector3.Dot(surfaceForward.normalized, Vector3.down);
            float slopeMultiplier = 1f + (slopeDot * 0.5f);

            rb.AddForce(surfaceForward * currentInput * speed * slopeMultiplier);
        }
        else
        {
            rb.AddForce(Vector3.down * 20f);
        }
    }
}