using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Netcode;

public class PlayerUIManager : MonoBehaviour
{
    // [Header("NETWORK JOIN")]
    // [SerializeField] bool startGameAsClient;
    // Start is called before the first frame update

    [HideInInspector] public PlayerUIHudManager playerUIHudManager;
    public static PlayerUIManager instance;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // if (startGameAsClient) {
        //     startGameAsClient = false;
        //     NetworkManager.Singleton.Shutdown();
        //     NetworkManager.Singleton.StartClient();
        // }
    }

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }

        playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
    }
}
