using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuControl : MonoBehaviour
{
    public Button chatroom;
    // Start is called before the first frame update
    void Start()
    {
        chatroom.onClick.AddListener(ChatRoom);
    }

    private void Update()
    {
    }

    public void ChatRoom()
    {
        SceneManager.LoadScene("ChatRoom");
    }
}
