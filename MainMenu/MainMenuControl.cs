using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuControl : MonoBehaviour
{
    public Button chatroom;
    public Button gobang;
    // Start is called before the first frame update
    void Start()
    {
        chatroom.onClick.AddListener(ChatRoom);
        gobang.onClick.AddListener(GoBang);
    }

    private void Update()
    {
    }

    public void ChatRoom()
    {
        SceneManager.LoadScene("ChatRoom");
    }

    public void GoBang()
    {
        SceneManager.LoadScene("GoBang");
    }
}
