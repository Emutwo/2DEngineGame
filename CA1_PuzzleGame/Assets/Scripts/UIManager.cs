using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Player player;

    [SerializeField] private TMP_Text text;
    private float time;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        player = gameObject.GetComponent<Player>();
        if (player != null)
        {
            text.text = player.ToString();
        }
        else
        {
            //Debug.Log("Player is null");
        }
        
    }
}
