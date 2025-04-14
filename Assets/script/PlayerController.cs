using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private Rigidbody playerRigidbody;
    public float speed;
    public float baseSpeed = 8f;
    public Vector3 baseScale;
    public Gear gear;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        baseScale = transform.localScale;
    }

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        gear = GetComponentInChildren<Gear>();
    }

    void Update()
    {
        if (!GameManager.instance.isLive) return;

        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");
        Vector3 newVelocity = new Vector3(xInput * speed, 0f, zInput * speed);
        playerRigidbody.velocity = newVelocity;
    }

    public void TakeDamage()
    {
        if (gear == null)
            gear = GetComponentInChildren<Gear>();

        if (gear != null && gear.hasShield)
        {
            gear.hasShield = false;
            Transform shieldTransform = transform.Find("ShieldEffect(Clone)");
            if (shieldTransform != null)
                Destroy(shieldTransform.gameObject);
            Debug.Log("[Player] 쉴드로 피해 무효화");
        }
        else
        {
            GameManager.instance.GameOver();
        }
    }
}
