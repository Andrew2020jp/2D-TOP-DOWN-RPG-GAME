using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterTp : MonoBehaviour
{
    [SerializeField] private Transform[] teleportDestinations;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(teleportDestinations.Length > 0)
            {
                int randomIndex = Random.Range(0, teleportDestinations.Length);
                Transform chosen = teleportDestinations[randomIndex];
                other.transform.position = chosen.position;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
