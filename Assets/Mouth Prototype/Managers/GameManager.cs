using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [SerializeField] private IGameState currentState;
    [SerializeField] private GameObject player;


    void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

public interface IGameStateMachine
{
    public void OnGameStateChanged(IGameState newState);
}

public interface IGameState
{
    public void OnEnter(GameManager gameManager);
    public void OnUpdate(GameManager gameManager);
    public void OnExit(GameManager gameManager);

}