using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitAI : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
public class InputInfo
{
    public enum CommandType
    {
        Move, Attack, Patrol
    }
    private CommandType command;
    private Vector3 position;
    public CommandType Command => command;
    public Vector3 Position => position;
    
}