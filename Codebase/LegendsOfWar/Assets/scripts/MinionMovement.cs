﻿using UnityEngine;
public enum Move_State
{ LANING_STATE, COMBAT_STATE, COMMAND_STATE, IDLE_STATE, ENGAGE_STATE, DISENGAGE_STATE }
public enum Path
{ CENTER_PATH = 0x121, NORTH_PATH = 0x241, SOUTH_PATH = 0x481, ANY_PATH = 0x7e7 }
public class MinionMovement : MovementScript
{
    [SerializeField]
    private Transform goal;
    [SerializeField]
    private Move_State m_state, m_prevState;
    private UnityEngine.AI.NavMeshAgent agent;
    private SkinnedMeshRenderer temp_smr;
    private LineRenderer line;
    private MinionInfo info;
    private Interactive interactive;
    private Path m_path;
    private bool followingNav = true;
    public void ChangeLane(Path _newPath = Path.ANY_PATH)
    {
        if (!followingNav)
            m_path = Path.ANY_PATH;
        else
            m_path = _newPath;
        if (!agent)
            Start2();
        agent.areaMask = (int)m_path;
        if (agent.isPathStale)
            agent.ResetPath();
    }
    protected override void Start()
    {
        base.Start();
        Start2();
        agent.speed = info.MovementSpeed;
        if (info.IsBasicMinionType)
        {
            line = gameObject.AddComponent<LineRenderer>();
            temp_smr = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (temp_smr)
                line.material = temp_smr.material;
        }
    }
    private void Update()
    {
        if (GameManager.GameRunning)
            switch (m_state)
            {
                case Move_State.LANING_STATE:
                    agent.enabled = true;
                    if (agent.pathPending)
                        break;
                    if (info.IsBasicMinionType)
                        line.enabled = false;
                    if (inCombat)
                        SetState(Move_State.ENGAGE_STATE);
                    else if (!followingNav)
                    {
                        followingNav = true;
                        if (transform.position.z <= GameManager.botSplitZ)
                            ChangeLane(Path.SOUTH_PATH);
                        else if (transform.position.z >= GameManager.topSplitZ)
                            ChangeLane(Path.NORTH_PATH);
                        else
                            ChangeLane(Path.CENTER_PATH);
                        agent.destination = goal.position;
                    }
                    if (!interactive.Selected)
                        rayHit = false;
                    CheckForInput();
                    break;
                case Move_State.COMMAND_STATE:
                    if (agent.pathPending)
                        break;
                    if (info.IsBasicMinionType)
                    {
                        line.enabled = true;
                        Vector3[] temp = new Vector3[] { transform.localPosition, agent.
                            destination };
                        line.SetPositions(temp);
                    }
                    if (Path.ANY_PATH != m_path)
                        ChangeLane();
                    agent.SetDestination(hit.point);
                    CheckForInput();
                    if (!agent.pathPending)
                        if (agent.remainingDistance <= 3.0f)
                            SetState(Move_State.IDLE_STATE);
                    break;
                case Move_State.COMBAT_STATE:
                    if (info.IsBasicMinionType)
                        line.enabled = false;
                    if (inCombat && TargetPosition)
                    {
                        if (Vector3.Distance(transform.position, TargetPosition.position) >
                            combatRange)
                        {
                            agent.isStopped = false;
                            withinRange = false;
                        }
                        else
                        {
                            agent.isStopped = true;
                            withinRange = true;
                        }
                    }
                    else
                        SetState(Move_State.DISENGAGE_STATE);
                    break;
                case Move_State.IDLE_STATE:
                    if (info.IsBasicMinionType)
                        line.enabled = false;
                    if (CheckForInput())
                        SetState(Move_State.COMMAND_STATE);
                    else if (!interactive.Selected)
                        SetState(Move_State.LANING_STATE);
                    else if (InCombat)
                        SetState(Move_State.ENGAGE_STATE);
                    else
                        agent.destination = hit.point;
                    break;
                case Move_State.ENGAGE_STATE:
                    if (agent && TargetPosition)
                        agent.destination = TargetPosition.position;
                    followingNav = false;
                    withinRange = false;
                    SetState(Move_State.COMBAT_STATE);
                    break;
                case Move_State.DISENGAGE_STATE:
                    agent.isStopped = false;
                    inCombat = false;
                    withinRange = false;
                    SetState(m_prevState);
                    m_prevState = m_state;
                    break;
                default:
                    break;
            }
    }
    private void Start2()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_state = m_prevState = Move_State.LANING_STATE;
        info = GetComponent<MinionInfo>();
        interactive = GetComponent<Interactive>();
        if (Team.RED_TEAM == info.team)
        {
            goal = GameManager.BluePortalTransform;
            if (agent.enabled)
                agent.destination = goal.position;
        }
        else
            goal = GameManager.RedPortalTransform;
    }
    private void SetState(Move_State _state)
    {
        switch (_state)
        {
            case Move_State.COMBAT_STATE:
            case Move_State.ENGAGE_STATE:
            case Move_State.DISENGAGE_STATE:
                m_state = _state;
                break;
            default:
                m_prevState = m_state;
                m_state = _state;
                break;
        }
    }
    private bool CheckForInput()
    {
        if (!HeroCamScript.onHero && interactive.Selected)
        {
            if (rayHit)
            {
                rayHit = false;
                followingNav = false;
                agent.ResetPath();
                agent.SetDestination(hit.point);
                if (info.IsBasicMinionType)
                {
                    Vector3[] temp = new Vector3[] { transform.localPosition, agent.destination };
                    line.SetPositions(temp);
                }
                SetState(Move_State.COMMAND_STATE);
                return true;
            }
            else if (Input.GetMouseButton(1))
            {
                if (Physics.Raycast(CameraControl.Current.ScreenPointToRay(Input.mousePosition),
                    out hit, 1000.0f, 5943))
                {
                    followingNav = false;
                    agent.ResetPath();
                    agent.SetDestination(hit.point);
                    if (info.IsBasicMinionType)
                    {
                        Vector3[] temp = new Vector3[] { transform.localPosition, agent.
                            destination };
                        line.SetPositions(temp);
                    }
                    SetState(Move_State.COMMAND_STATE);
                    return true;
                }
            }
        }
        return false;
    }
}