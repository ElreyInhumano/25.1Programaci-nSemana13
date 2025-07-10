using UnityEngine;
using Unity.Netcode;
public class BulletMovement : NetworkBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speedBullet;
    [SerializeField] private float timerBullet;
    public ulong ownerId;
    public void Init(ulong ownerId)
    {
        this.ownerId = ownerId;
    }
    void Start()
    {
        if (IsServer)
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }
        
    }

    void Update()
    {
        if (IsServer)
        {
            TimerBullet();
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            rb.AddForce(new Vector3(0, speedBullet, 0), ForceMode.VelocityChange);
        }
    }
    private void BulletMove()
    {
        rb.AddForce(new Vector3(0, speedBullet, 0), ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.CompareTag("Enemy"))
        {
                //other.GetComponent<Player>().SetLife(1);
                Invoke(nameof(BulletAutoDestroy), 0.05f);
            
        }
    }

    private void TimerBullet()
    {
        timerBullet += Time.deltaTime;
        if (timerBullet >= 2f)
        {
            Invoke(nameof(BulletAutoDestroy), 0.05f);
        }
    }
    private void BulletAutoDestroy()
    {
        NetworkObject.Despawn(gameObject);

    }
}
