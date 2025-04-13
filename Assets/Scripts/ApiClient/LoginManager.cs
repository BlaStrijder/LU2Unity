using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    public GameObject errorMessage;
    public async void Login()
    {
        await ApiClient.Instance.Login(inputEmail.text, inputPassword.text);
        if (errorMessage == null)
        {
            return;
        }
        errorMessage.SetActive(true);
    }
    public async void Register()
    {
        await ApiClient.Instance.Register(inputEmail.text, inputPassword.text);
        if (errorMessage == null)
        {
            return;
        }
        errorMessage.SetActive(true);
    }
}
