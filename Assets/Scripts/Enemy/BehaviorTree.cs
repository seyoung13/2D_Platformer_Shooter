using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    FAILURE,
    SUCCESS,
    RUNNING
}

public abstract class BehaviorTree
{   
    public delegate NodeState ReturnNode();

    protected NodeState node_state;
    
    public NodeState NodeState
    { get { return node_state; } }

    public BehaviorTree() { }

    public abstract NodeState Run();

    public abstract string GetName();
}

public class SelectorNode : BehaviorTree
{
    protected List<BehaviorTree> nodes = new List<BehaviorTree>();
    private int prev_running_pos = 0;

    public SelectorNode(List<BehaviorTree> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Run()
    {
        for (int i = prev_running_pos; i < nodes.Count; i++)
        {
            switch (nodes[i].Run())
            {
                case NodeState.RUNNING:
                    prev_running_pos = i;
                    node_state = NodeState.RUNNING;
                    return node_state;
                case NodeState.SUCCESS:
                    prev_running_pos = 0;
                    node_state = NodeState.SUCCESS;
                    return node_state;
                default:
                    continue;
            }
        }

        prev_running_pos = 0;
        node_state = NodeState.FAILURE;
        return node_state;
    }

    public override string GetName()
    {
        string curr_state = "";
        foreach (BehaviorTree node in nodes)
        {
            curr_state += node.GetName();
        }

        return curr_state;
    }
}

public class SequenceNode :  BehaviorTree
{
    private List<BehaviorTree> nodes = new List<BehaviorTree>();
    private int prev_running_pos = 0;

    public SequenceNode(List<BehaviorTree> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Run()
    {
        for (int i = prev_running_pos; i<nodes.Count; i++)
        {
            switch (nodes[i].Run())
            {   
                case NodeState.RUNNING:
                    prev_running_pos = i;
                    node_state = NodeState.RUNNING;
                    return node_state;
                case NodeState.FAILURE:
                    prev_running_pos = 0;
                    node_state = NodeState.FAILURE;
                    return node_state;
                default:
                    continue;
            }
        }

        prev_running_pos = 0;
        node_state = NodeState.SUCCESS;
        return node_state;
    }

    public override string GetName()
    {
        string curr_state = "";
        foreach (BehaviorTree node in nodes)
        {
            curr_state += node.GetName();
        }

        return curr_state;
    }
}

public class LeafNode : BehaviorTree
{
    public delegate NodeState FunctionPointer();

    private FunctionPointer func;
    private string name;

    public LeafNode(FunctionPointer func, string name)
    {
        this.func = func;
        this.name = name;
    }

    public override string GetName()
    {
        return name + ": " + node_state + "| ";
    }

    public override NodeState Run()
    {
        switch (func())
        {
            case NodeState.FAILURE:
                node_state = NodeState.FAILURE;
                return node_state;
            case NodeState.RUNNING:
                node_state = NodeState.RUNNING;
                return node_state;
            case NodeState.SUCCESS:
                node_state = NodeState.SUCCESS;
                return node_state;
            default:
                node_state = NodeState.FAILURE;
                return node_state;
        }
    }

}
