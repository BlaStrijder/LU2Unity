using NUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldsDisplay : MonoBehaviour
{
    public GameObject environmentItemPrefab;
    public Transform environmentListContainer;
    public TextMeshProUGUI userName;
    public TMP_InputField worldInputField;
    public TMP_InputField worldHeightInputField;
    public TMP_InputField worldLengthInputField;
    public GameObject CreateWorldBox;
    bool createWorldBoxActive = false;

    public void Start()
    {
        LoadEnvironments();
        userName.text =$"Welcome {ApiClient.Instance.userName}";
    }

    //Load environments which user can choose from
    public async void LoadEnvironments()
    {
        List<GetEnvironmentsResponseDto> environments = await ApiClient.Instance.GetEnvironments();

        foreach (Transform Worlds in environmentListContainer)
        {
            Destroy(Worlds.gameObject);
        }

        if (environments != null)
        {
            CreateEnvironmentItems(environments);
            Debug.Log("Environments loaded");
        }
    }

    public void CreateEnvironmentItems(List<GetEnvironmentsResponseDto> environments)
    {
        foreach (var environment in environments)
        {
            GameObject panel = Instantiate(environmentItemPrefab, environmentListContainer);
            TextMeshProUGUI nameText = panel.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = environment.environmentName;
            }

            Button panelButton = panel.GetComponentInChildren<Button>();
            if (panelButton != null)
            {
                panelButton.onClick.AddListener(() => OnWorldtClick(environment.id));
            }
        }
    }

    public void OnWorldtClick(string environmentId)
    {
        ApiClient.Instance.currentEnvironmentId = environmentId;
        //Debug.Log(environmentId);
        SceneManager.LoadScene("World");
    }
    public void ShowCreateEnvironment()
    {
        if (!createWorldBoxActive)
        {
            CreateWorldBox.SetActive(true);
            createWorldBoxActive = true;
        }
        else { 
            CreateWorldBox.SetActive(false);
            createWorldBoxActive = false;
        }
    }
    public async void CreateEnvironmentButton()
    {
        await CreateEnvironment();
        LoadEnvironments();
    }
    public async void DeleteEnvironmentButton()
    {
        await DeleteEnvironment();
        LoadEnvironments();
    }
    public async Task CreateEnvironment()
    {
        string chosenEnvironment = worldInputField.text;
        int maxHeight = Convert.ToInt16(worldHeightInputField.text);
        int maxLength = Convert.ToInt16(worldLengthInputField.text);
        if (!string.IsNullOrEmpty(chosenEnvironment) && chosenEnvironment.Length <= 25)
        {
            await ApiClient.Instance.CreateEnvironment(chosenEnvironment, maxLength, maxHeight);
            Debug.Log($"World name has between 1 and 25 characters");
        }
        else
        {
            Debug.Log($"World name requires between 1 and 25 characters");
            return;
        }
    }

    public async Task DeleteEnvironment()
    {
        string chosenEnvironment = worldInputField.text;
        if(!string.IsNullOrEmpty(chosenEnvironment))
        {
            await ApiClient.Instance.DeleteEnvironment(chosenEnvironment);
        }
        else
        {
            Debug.Log("No environment name selected");
        }
    }
}
