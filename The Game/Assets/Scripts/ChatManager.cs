using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
public class ChatManager : MonoBehaviourPunCallbacks 
{
    bool isChatting = false;
    string chatInput = "";

    public static ChatManager Instance;

    [System.Serializable]
    public class ChatMessage
    {
        public string sender = "";
        public string message = "";
        public float timer = 0;
    }


    List<ChatMessage> chatMessages = new List<ChatMessage>();
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Initialize Photon View
        if (gameObject.GetComponent<PhotonView>() == null)
        {
            PhotonView photonView = gameObject.AddComponent<PhotonView>();
            photonView.ViewID = 1;
        }
        else
        {
            photonView.ViewID = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Slash) && !isChatting)
        {
            isChatting = true;
            chatInput = "";
        }

        //Hide messages after timer is expired
        for (int i = 0; i < chatMessages.Count; i++)
        {
            if (chatMessages[i].timer > 0)
            {
                chatMessages[i].timer -= Time.deltaTime;
            }
        }
    }


    public void ChatOn()
    {
        if (!isChatting)
        {
            isChatting = true;
            chatInput = "";
        }
    }
    void OnGUI()
    {
        if (!isChatting)
        {
            GUI.Label(new Rect(5, Screen.height - 25, 200, 25), "Press '/' to start chatting...");
        }
        else
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                isChatting = false;
                if (chatInput.Replace(" ", "") != "")
                {
                    //Send message
                    photonView.RPC("SendChat", RpcTarget.All, PhotonNetwork.LocalPlayer, chatInput);
                }
                chatInput = "";
            }

            GUI.SetNextControlName("ChatField");
            GUI.Label(new Rect(5, Screen.height - 25, 200, 25), "Say:");
            GUIStyle inputStyle = GUI.skin.GetStyle("box");
            inputStyle.alignment = TextAnchor.MiddleLeft;
            chatInput = GUI.TextField(new Rect(10 + 25, Screen.height - 27, 400, 22), chatInput, 60, inputStyle);

            GUI.FocusControl("ChatField");
        }

        //Show messages
        for (int i = 0; i < chatMessages.Count; i++)
        {
            if (chatMessages[i].timer > 0 || isChatting)
            {
                GUI.Label(new Rect(5, Screen.height - 50 - 25 * i, 500, 25), chatMessages[i].sender + ": " + chatMessages[i].message);
            }
        }
    }

    [PunRPC]
    void SendChat(Photon.Realtime.Player sender, string message)
    {
        ChatMessage m = new ChatMessage();
        m.sender = sender.NickName;
        m.message = message;
        m.timer = 15.0f;

        chatMessages.Insert(0, m);
        if (chatMessages.Count > 8)
        {
            chatMessages.RemoveAt(chatMessages.Count - 1);
        }
    }
    [PunRPC]
    public void SendSystemMessage(string message)
    {
        ChatMessage m = new ChatMessage();
        m.sender = "{SYSTEM}";
        m.message = message;
        m.timer = 15.0f;
        
        chatMessages.Insert(0, m);
        if (chatMessages.Count > 8)
        {
            chatMessages.RemoveAt(chatMessages.Count - 1);
        }
    }
    public void SendSystemMessageGlobal(string _message)
    {
        photonView.RPC("SendSystemMessage", RpcTarget.All, _message);
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        photonView.RPC("SendSystemMessage", RpcTarget.All, newPlayer.NickName + " has joined the room.");
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        photonView.RPC("SendSystemMessage", RpcTarget.All, otherPlayer.NickName + " has left the room.");
    }
}
