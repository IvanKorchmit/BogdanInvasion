using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
public class UnitAI : MonoBehaviour, ISelectable
{
    private Path path;
    private CharacterController controller;
    [SerializeField] Transform visuals;
    [SerializeField] float speed;
    [SerializeField] float wayPointDistance;
    [SerializeField] Seeker seeker;
    [SerializeField] Vector3 targetPosition;
    private bool hasTarget;
    private void Move()
    {
        if ((path == null || path.path.Count == 0))
        {
            seeker.StartPath(transform.position, targetPosition, OnPathFound);
            hasTarget = false;
            return;
        }
        Vector3 move = (Vector3)path.path[0].position - transform.position;
        Vector3 dir = (Vector3)path.path[0].position;
        dir.y = transform.position.y;
        visuals.LookAt(dir);
        controller.Move(move.normalized * speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, (Vector3)path.path[0].position) <= wayPointDistance)
        {
            path.path.RemoveAt(0);
        }
    }
    private void OnPathFound(Path p)
    {
        if (!p.error)
        {
            path = p;
            hasTarget = true;
        }
        
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    public void OnSelect()
    {
        PlayerInput.OnPlayerInput += PlayerInput_OnPlayerInput;

    }
    public void OnDeselect()
    {
        PlayerInput.OnPlayerInput -= PlayerInput_OnPlayerInput;

    }
    void Update()
    {
        if (hasTarget)
        {
            Move();
        }
    }

    private void PlayerInput_OnPlayerInput(InputInfo info)
    {
        if (info.Command == InputInfo.CommandType.Move)
        {
            Debug.Log($"{name} is moving");
            targetPosition = info.Position;
            seeker.StartPath(transform.position, targetPosition, OnPathFound);

            hasTarget = true;
        }
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
    public InputInfo(Vector3 position, CommandType command)
    {
        this.position = position;
        this.command = command;
    }
}


public interface ISelectable
{
    void OnSelect();
    void OnDeselect();
}