using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using TMPro;
public class Enemies : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> enemyLife = new NetworkVariable<FixedString32Bytes>();
    [SerializeField] public float life;
    [SerializeField] public string nameS;
    [SerializeField] private float speedEnemy;
    private ulong ownerId;
    private Rigidbody rb;
    [SerializeField] private TextMesh lifeText;
    [SerializeField] private float timerLife;


    void Start()
    {
        if (IsServer)
        {
            rb = gameObject.GetComponent<Rigidbody>();
            enemyLife.Value = nameS + $": {this.life}";
        }
    }
    public override void OnNetworkSpawn()
    {
        lifeText.text = enemyLife.Value.ToString();
        enemyLife.OnValueChanged += (oldName, newName) =>
        {
            lifeText.text = newName.Value.ToString();
        };

    }
    public void Init(ulong ownerId)
    {
        this.ownerId = ownerId;
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            TimerLife();
            rb.AddForce(new Vector3(0, speedEnemy, 0), ForceMode.Impulse);
        }
    }
    public void SetLife(float life)
    {
        if (IsOwner)
        {
            SendLifeToServerRpc(life);
        }
    }
    [Rpc(SendTo.Server)]
    private void SendLifeToServerRpc(float life)
    {
        enemyLife.Value = nameS + $": {this.life}";
        SendLifeToClientsRpc(nameS + $": {this.life}", life);
    }
    [Rpc(SendTo.Server)]
    private void SendLifeToClientsRpc(string lifeS, float life)
    {
        this.life -= life;
        lifeText.text = lifeS;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && IsServer)
        {
            SetLife(3);
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject player in players)
            {
                if(other.GetComponent<BulletMovement>().ownerId == player.GetComponent<Player>().OwnerClientId)
                {
                    player.GetComponent<Player>().SetScore(30);
                }
            }
            
            if (life <= 1)
            {
                EnemyAutoDestroy();
            }
        }
        if (other.CompareTag("Bullet2") && IsServer)
        {
            SetLife(1);
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (other.GetComponent<BulletMovement>().ownerId == player.GetComponent<Player>().OwnerClientId)
                {
                    player.GetComponent<Player>().SetScore(10);
                }
            }
            if (life <= 1)
            {
                EnemyAutoDestroy();
            }
        }
        if (other.CompareTag("Player") && IsServer)
        {
            EnemyAutoDestroy();
        }
    }
    private void TimerLife()
    {
        timerLife += Time.deltaTime;
        if (timerLife >= 6f)
        {
            Invoke(nameof(EnemyAutoDestroy), 0.05f);
        }
    }
    private void EnemyAutoDestroy()
    {
        
        SpawnEnemies.enemiesKilled += 1;
        Destroy(gameObject);
        //NetworkObject.Despawn(gameObject);

    }
}
