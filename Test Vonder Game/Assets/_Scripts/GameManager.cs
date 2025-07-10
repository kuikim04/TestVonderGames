using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isInteraction = false;

    [SerializeField] private GameObject bagPanel;
    private void Awake()
    {
        if(Instance == null)  
            Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            OpenBag();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            isInteraction = false;
            bagPanel.SetActive(false);
        }
    }

    public void OpenBag()
    {
        isInteraction = !isInteraction;
        bagPanel.SetActive(isInteraction);

    }
}
