using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nick;
    
    [SerializeField] private Button confirmation;

    private LoginController login;
    
    private void Start()
    {
        login = ComponentRoot.Resolve<LoginController>();
        
        confirmation.onClick.AddListener(Login);
    }

    private void Login()
    {
        if(nick.text == "")
            return;
        
        StartCoroutine(login.Login(nick.text));

        SceneManager.LoadScene("Menu");
    }
}
