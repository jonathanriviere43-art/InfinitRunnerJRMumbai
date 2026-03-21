using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float worldSpeed = 10f;

    void Awake()
    {
        Instance = this;
    }
}
