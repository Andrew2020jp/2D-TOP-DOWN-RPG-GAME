using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    //[SerializeField] public string newTown;
    public string newTown;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && newTown != null)
        {
            //PlayerHealth.Instance.TOWN_TEXT = newTown;
            newTown = SceneManager.GetActiveScene().name;
            PlayerHealth.Instance.TOWN_TEXT = newTown;
            Debug.Log("Town_text is updated to: " + newTown);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
