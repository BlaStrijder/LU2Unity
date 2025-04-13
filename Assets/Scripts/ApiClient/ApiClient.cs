
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance { get; private set; }
    public string accessToken;
    public string userName;
    public string currentEnvironmentId;
    public string currentEnvironmentName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public async Task Register(string email, string password)
    {
        var request = new PostRegisterRequestDto()
        {
            email = email,
            password = password
        };
        var jsondata = JsonUtility.ToJson(request);
        //Debug.Log(jsondata);
        var response = await PerformApiCall("https://avansict2233364.azurewebsites.net/account/register", "Post", jsondata);
        if (response != null)
        {
            await Login(email, password);
         
            var requestUser = new PostUserRequestDto()
            {
                userName = userName,
                email = email,
                password = "Secret"
            };
            var jsondataUser = JsonUtility.ToJson(requestUser);
            //Debug.Log(jsondataUser);
            var responseUser = await PerformApiCall("https://avansict2233364.azurewebsites.net/user", "Post", jsondataUser, accessToken);
        }
    }

    public async Task Login(string email, string password)
    {
        var request = new PostRegisterRequestDto()
        {
            email = email,
            password = password
        };

        string[] emailSplit = email.Split('@')[0].Split('.');
        userName = emailSplit[0];
        //Debug.Log($"username {userName}");

        var jsondata = JsonUtility.ToJson(request);
        //Debug.Log(jsondata);
        Debug.Log("Trying to login");
        var response = await PerformApiCall("https://avansict2233364.azurewebsites.net/account/login", "Post", jsondata);
        //Debug.Log(response);
        if(!string.IsNullOrEmpty(response))
        {
            Debug.Log("Login successful");
            var responseDto = JsonUtility.FromJson<PostLoginResponseDto>(response);
            //Debug.Log(responseDto.accessToken);

            if (responseDto.accessToken != null)
            {
                accessToken = responseDto.accessToken;
                SceneManager.LoadScene(3);
                await GetEnvironments();
            }
        }
        else
        {
            Debug.Log("Apicall failed");
        }
    }

    private async Task<string> PerformApiCall(string url, string method, string jsonData = null, string token = null)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            await request.SendWebRequest();
            //Debug.Log("HTTP Status Code: " + request.responseCode);
            //Debug.Log("Error Body: " + request.downloadHandler.text);
            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log("API-aanroep is successvol: " + request.downloadHandler.text);

                return request.downloadHandler.text;
            }
            else
            {
                //Debug.Log("Fout bij API-aanroep: " + request.error);
                return null;
            }
        }
    }

    public async Task<List<GetEnvironmentsResponseDto>> GetEnvironments()
    {
        var response = await PerformApiCall($"https://avansict2233364.azurewebsites.net/environment/username/{Instance.userName}", "GET", null, accessToken);
        //Debug.Log("Raw API Response: " + response);
        //Debug.Log("Access Token: " + accessToken);
        // Wrap the response into an object
        string wrappedResponse = "{\"environments\":" + response + "}";

        // Deserialize the wrapped response
        EnvironmentListWrapper wrapper = JsonUtility.FromJson<EnvironmentListWrapper>(wrappedResponse);

        if (wrapper == null || wrapper.environments == null || wrapper.environments.Count == 0)
        {
            Debug.Log("No environments found");
            return null;
        }

        foreach (var environment in wrapper.environments)
        {
            //Debug.Log($"Environment ID: {environment.id}, Name: {environment.environmentName}");
        }
        Debug.Log("Environments found");

        return wrapper.environments;
    }


    public async Task<GetCurrentEnvironmentResponseDto> GetCurrentEnvironment()
    {
        var response = await PerformApiCall($"https://avansict2233364.azurewebsites.net/environment/id/{Instance.currentEnvironmentId}", "GET", null, accessToken);
        var responseDto = JsonUtility.FromJson<GetCurrentEnvironmentResponseDto>(response);

        if (responseDto == null)
        {
            Debug.Log("No environment found");
        }

        currentEnvironmentName = responseDto.environmentName;
        //Debug.Log(currentEnvironmentName);
        //Debug.Log(responseDto.environmentName);

        Debug.Log("Environment found");

        return responseDto;
    }

    public async Task DeleteEnvironment(string chosenEnvironment)
    {
        List<GetEnvironmentsResponseDto> environments = await Instance.GetEnvironments();
        var matchedEnvironment = environments.FirstOrDefault(environment => environment.environmentName == chosenEnvironment);

        if (matchedEnvironment != null)
        {
            string foundId = matchedEnvironment.id;
            //Debug.Log("Found environment to delete: " + foundId);
        }
        else
        {
            Debug.Log("Environment not found!");
            return;
        }
        var response = await PerformApiCall($"https://avansict2233364.azurewebsites.net/environment/{matchedEnvironment.id}", "DELETE", null, accessToken);
        if (response != null)
        {
            Debug.Log("Found environment to delete");
        }
    }

    public async Task CreateEnvironment(string chosenEnvironment, int maxLength, int maxHeight)
    {
        List<GetEnvironmentsResponseDto> environments = await Instance.GetEnvironments();

        // creates empty list
        if (environments == null)
        {
            environments = new List<GetEnvironmentsResponseDto>();
        }

        var matchedEnvironment = environments.FirstOrDefault(environment => environment.environmentName == chosenEnvironment);
        //Debug.Log(matchedEnvironment);
        if (matchedEnvironment != null)
        {
            Debug.Log("Environmentname already exists");
            return;
        }
        else if (environments.Count >= 5)
        {
            Debug.Log("Max of 5 environments reached ");
            return;
        }
        else
        {
            Debug.Log("Environmentname doesnt match");
        }

        var request = new PostEnvironmentRequestDto()
        {
            environmentName = chosenEnvironment,
            ownerUserName = userName,
            maxLength = maxLength,
            maxHeight = maxHeight
        };
        var jsondata = JsonUtility.ToJson(request);
        //Debug.Log(jsondata);

        var response = await PerformApiCall($"https://avansict2233364.azurewebsites.net/environment", "Post", jsondata, accessToken);
    }

    public async Task<string> CreateObject(PostObjectRequestDto data)
    {
        var request = new PostObjectRequestDto()
        {
            environmentId = data.environmentId,
            prefabId = data.prefabId,
            positionX = data.positionX,
            positionY = data.positionY,
            scaleX = data.scaleX,
            scaleY = data.scaleY,
            rotationZ = data.rotationZ,
            sortingLayer = data.sortingLayer
        };

        var jsondata = JsonUtility.ToJson(request);
        //Debug.Log(jsondata);

        var response = await PerformApiCall($"https://avansict2233364.azurewebsites.net/gameobject", "Post", jsondata, accessToken);
        //Debug.Log($"response object made {response}");
        return response;
    }

    public async Task<string> UpdateObject(string id, PutObjectRequestDto data)
    {
        var jsondata = JsonUtility.ToJson(data);
        //Debug.Log(jsondata);
        //Debug.Log(id);

        var response = await PerformApiCall($"https://avansict2233364.azurewebsites.net/gameobject/{id}", "PUT", jsondata, accessToken);
        if (response != null)
        {
            Debug.Log("Object updated");
        }
        return response;
    }

    public async Task<List<GetEnvironmentObjectsResponseDto>> GetEnvironmentObjects()
    {
        var response = await PerformApiCall($"https://avansict2233364.azurewebsites.net/gameobject/environmentid/{Instance.currentEnvironmentId}", "GET", null, accessToken);

        string wrappedResponse = "{\"objects\":" + response + "}";

        ObjectsListWrapper wrapper = JsonUtility.FromJson<ObjectsListWrapper>(wrappedResponse);

        return wrapper.objects;
    }
}
