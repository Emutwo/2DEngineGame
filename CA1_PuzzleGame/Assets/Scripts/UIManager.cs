using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Player player;

    [SerializeField] private TMP_Text text;
    private float time;
    
    
    void Update()
    {
        player = gameObject.GetComponent<Player>();
        if (player != null)
        {
            text.text = player.ToString();
        }
    }
}
