using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using TMPro;
public class Player : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();
    private NetworkVariable<FixedString32Bytes> playerLife = new NetworkVariable<FixedString32Bytes>();
    private NetworkVariable<FixedString32Bytes> playerColor = new NetworkVariable<FixedString32Bytes>();
    private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] public float life;
    [SerializeField] private TextMesh playerNameText;
    [SerializeField] private TextMesh lifeText;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material red, blue, yellow, green;
    [Header("Attack Variables")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bullet2;
    [SerializeField] private GameObject shootPoint;
    [SerializeField] private bool attacking;
    [SerializeField] private float delayAttack;
    [SerializeField] private float delayAttack2;
    private float lastAttackTime;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        playerLife.Value = $"Vida: {life}";
    }
    public override void OnNetworkSpawn()
    {
        playerNameText.text = playerName.Value.ToString();
        playerName.OnValueChanged += (oldName, newName) =>
        {
            playerNameText.text = newName.Value.ToString();
        };
        lifeText.text = playerLife.Value.ToString();
        playerLife.OnValueChanged += (oldName, newName) =>
        {
            lifeText.text = newName.Value.ToString();
        };

    }

    public void SetName(string name)
    {
        if (IsOwner)
        {
            SendNameToServerRpc(name);
        }
    }
    public void SetLife(float life)
    {
        if (IsOwner)
        {
            SendLifeToServerRpc(life);
        }
    }
    public void SetColor(string color)
    {
        if (IsOwner)
        {
            SendColorToServerRpc(color);
        }
    }

    [Rpc(SendTo.Server)]
    private void SendNameToServerRpc(string name)
    {
        playerName.Value = name;
        SendNameToClientsRpc(name);
    }
    [Rpc(SendTo.Server)]
    private void SendNameToClientsRpc(string name)
    {
        playerNameText.text = name;
    }
    [Rpc(SendTo.Server)]
    private void SendLifeToServerRpc(float life)
    {
        playerLife.Value = $"Vida: {this.life}";
        SendLifeToClientsRpc($"Vida: {this.life - 1}", life);
    }
    [Rpc(SendTo.Server)]
    private void SendLifeToClientsRpc(string lifeS, float life)
    {
        this.life -= life;
        lifeText.text = lifeS;
    }
    [Rpc(SendTo.Server)]
    private void SendColorToServerRpc(string color)
    {
        playerColor.Value = color;
        SendColorToClientsRpc(color);
    }
    [Rpc(SendTo.Server)]
    private void SendColorToClientsRpc(string color)
    {
        switch (color)
        {
            case "red":
                meshRenderer.sharedMaterial = red;
                break;
            case "blue":
                meshRenderer.sharedMaterial = blue;
                break;
            case "yellow":
                meshRenderer.sharedMaterial = yellow;
                break;
            case "green":
                meshRenderer.sharedMaterial = green;
                break;
        }
    }
    void Update()
    {
        if (IsOwner)
        {
            PlayerMove();
            PlayerShoot();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (IsOwner)
            {
                SetLife(1);
            }
        }
    }
    void PlayerMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(h, v);
        direction.Normalize();
        rb.linearVelocity = new Vector3(direction.x, 0, direction.y) * speed + Vector3.up * rb.linearVelocity.y;
    }

    void PlayerShoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            attacking = true;
            if (Time.time - lastAttackTime > delayAttack)
            {
                if (attacking)
                {
                    StartCoroutine(ShootBullet());
                }
                lastAttackTime = Time.time;
            }
        }
        else
        {
            attacking = false;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            attacking = true;
            if (Time.time - lastAttackTime > delayAttack2)
            {
                if (attacking)
                {
                    StartCoroutine(ShootBullet2());
                }
                lastAttackTime = Time.time;
            }
        }
        else
        {
            attacking = false;
        }
    }
    [Rpc(SendTo.Server)]
    private void SpawnBulletRpc()
    {
        GameObject obj = Instantiate(bullet, shootPoint.transform.position, shootPoint.transform.rotation);
        obj.GetComponent<BulletMovement>().Init(OwnerClientId);
        obj.GetComponent<NetworkObject>().Spawn();
        obj.GetComponent<MeshRenderer>().sharedMaterial = meshRenderer.sharedMaterial;
    }
    [Rpc(SendTo.Server)]
    private void SpawnBullet2Rpc()
    {
        GameObject obj = Instantiate(bullet2, shootPoint.transform.position, shootPoint.transform.rotation);
        obj.GetComponent<BulletMovement>().Init(OwnerClientId);
        obj.GetComponent<NetworkObject>().Spawn();
        obj.GetComponent<MeshRenderer>().sharedMaterial = meshRenderer.sharedMaterial;

    }
    private IEnumerator ShootBullet()
    {
        for (int i = 0; i < 1; i++)
        {
            SpawnBulletRpc();
            yield return new WaitForSeconds(1f);
        }
    }
    private IEnumerator ShootBullet2()
    {
        for (int i = 0; i < 1; i++)
        {
            SpawnBullet2Rpc();
            yield return new WaitForSeconds(1f);
        }
    }
}
