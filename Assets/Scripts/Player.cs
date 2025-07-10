using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using TMPro;
public class Player : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();
    private NetworkVariable<FixedString32Bytes> playerScore = new NetworkVariable<FixedString32Bytes>();
    private NetworkVariable<FixedString32Bytes> playerColor = new NetworkVariable<FixedString32Bytes>();
    private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private float score;    
    [SerializeField] private TextMesh playerNameText;
    [SerializeField] private TextMesh scoreText;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material red, blue, yellow, green;
    [Header("Attack Variables")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bullet2;
    [SerializeField] private GameObject shootPoint;
    [SerializeField] private bool attacking;
    [SerializeField] private bool haveFaster;
    [SerializeField] private float delayAttack;
    [SerializeField] private float biggerDelayAttack;
    [SerializeField] private float fasterDelayAttack;
    private float lastAttackTime;
    [SerializeField] public GunType gunType;
    public enum GunType { biggerShoot, fasterShoot }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        playerScore.Value = $"Score: {score}";
    }
    public override void OnNetworkSpawn()
    {
        playerNameText.text = playerName.Value.ToString();
        playerName.OnValueChanged += (oldName, newName) =>
        {
            playerNameText.text = newName.Value.ToString();
        };
        scoreText.text = playerScore.Value.ToString();
        playerScore.OnValueChanged += (oldName, newName) =>
        {
            scoreText.text = newName.Value.ToString();
        };

    }

    public void SetName(string name)
    {
        if (IsOwner)
        {
            SendNameToServerRpc(name);
        }
    }
    public void SetScore(float score)
    {
        if (IsOwner)
        {
            SendScoreToServerRpc(score);
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
    private void SendScoreToServerRpc(float score)
    {
        playerScore.Value = $"Score: {this.score}";
        SendScoreToClientsRpc($"Score: {this.score}", score);
    }
    [Rpc(SendTo.Server)]
    private void SendScoreToClientsRpc(string scoreS, float score)
    {
        this.score += score;
        scoreText.text = scoreS;
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
            AttackType();
            PlayerShoot();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (IsOwner)
            {
                SetScore(-10);
            }
        }
        if (other.CompareTag("Weapon1"))
        {
            if (IsOwner)
            {
                SetColor("red");
                haveFaster = false;
            }
        }
        if (other.CompareTag("Weapon2"))
        {
            if (IsOwner)
            {
                SetColor("blue");
                haveFaster = true;
            }
        }
    }
    void PlayerMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(h, v);
        direction.Normalize();
        rb.linearVelocity = new Vector3(direction.x, 0, 0) * speed + Vector3.up * rb.linearVelocity.y;
    }

    void PlayerShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            attacking = true;
            switch (gunType)
            {
                case GunType.biggerShoot:
                    if (Time.time - lastAttackTime > delayAttack)
                    {
                        if (attacking)
                        {
                            StartCoroutine(ShootBullet());
                        }
                        lastAttackTime = Time.time;
                        //Invoke(nameof(SpawnBullet), 0.9f);
                    }
                    break;
                case GunType.fasterShoot:
                    if (Time.time - lastAttackTime > delayAttack)
                    {
                        if (attacking)
                        {
                            StartCoroutine(ShootBullet2());

                        }
                        lastAttackTime = Time.time;
                        //Invoke(nameof(SpawnBullet), 0.9f);
                    }
                    break;
            }
        }
        else
        {
            attacking = false;
        }
    }
    private void AttackType()
    {
        if (haveFaster)
        {
            gunType = GunType.fasterShoot;
            delayAttack = fasterDelayAttack;
        }
        else if (!haveFaster)
        {
            gunType = GunType.biggerShoot;
            delayAttack = biggerDelayAttack;
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
