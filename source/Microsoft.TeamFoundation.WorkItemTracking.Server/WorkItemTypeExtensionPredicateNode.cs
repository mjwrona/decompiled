// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionPredicateNode
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTypeExtensionPredicateNode
  {
    private WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType m_nodeType;
    private List<WorkItemTypeExtensionPredicateNode> m_children;
    private PredicateFieldComparisonOperator m_operator;
    private bool m_inverted;
    private int m_fieldId;

    private WorkItemTypeExtensionPredicateNode(
      WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType nodeType,
      List<WorkItemTypeExtensionPredicateNode> children)
    {
      this.m_nodeType = nodeType;
      this.m_children = children;
    }

    private WorkItemTypeExtensionPredicateNode(
      WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType nodeType,
      PredicateFieldComparisonOperator predicateOp)
    {
      this.m_nodeType = nodeType;
      this.m_operator = predicateOp;
    }

    public WorkItemTypeExtensionPredicateNode(PredicateOperator predicateOp)
    {
      this.m_inverted = false;
      for (PredicateUnaryOperator predicateUnaryOperator = predicateOp as PredicateUnaryOperator; predicateUnaryOperator != null; predicateUnaryOperator = predicateOp as PredicateUnaryOperator)
      {
        if (predicateUnaryOperator is PredicateNotOperator)
          this.m_inverted = !this.m_inverted;
        predicateOp = predicateUnaryOperator.Operand;
      }
      if (predicateOp is PredicateBinaryOperator predicateBinaryOperator)
      {
        this.m_nodeType = predicateBinaryOperator is PredicateAndOperator ? WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And : WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Or;
        PredicateOperator[] operands = predicateBinaryOperator.Operands;
        if (operands == null)
          return;
        this.m_children = new List<WorkItemTypeExtensionPredicateNode>(operands.Length);
        foreach (PredicateOperator predicateOp1 in operands)
          this.m_children.Add(new WorkItemTypeExtensionPredicateNode(predicateOp1));
      }
      else
      {
        this.m_nodeType = WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Leaf;
        this.m_operator = predicateOp as PredicateFieldComparisonOperator;
        if (this.m_operator == null)
          return;
        this.m_fieldId = this.m_operator.GetReferencedFields().FirstOrDefault<int>();
        if (this.m_operator is PredicateNotUnderOperator notUnderOperator)
        {
          this.m_inverted = !this.m_inverted;
          PredicateUnderOperator predicateUnderOperator = new PredicateUnderOperator();
          predicateUnderOperator.Field = notUnderOperator.Field;
          predicateUnderOperator.Value = notUnderOperator.Value;
          this.m_operator = (PredicateFieldComparisonOperator) predicateUnderOperator;
        }
        else if (this.m_operator is PredicateNotEqualsOperator notEqualsOperator)
        {
          this.m_inverted = !this.m_inverted;
          PredicateEqualsOperator predicateEqualsOperator = new PredicateEqualsOperator();
          predicateEqualsOperator.Field = notEqualsOperator.Field;
          predicateEqualsOperator.Value = notEqualsOperator.Value;
          this.m_operator = (PredicateFieldComparisonOperator) predicateEqualsOperator;
        }
        else
        {
          if (!(this.m_operator is PredicateNotContainsOperator containsOperator1))
            return;
          this.m_inverted = !this.m_inverted;
          PredicateContainsOperator containsOperator2 = new PredicateContainsOperator();
          containsOperator2.Field = containsOperator1.Field;
          containsOperator2.Value = containsOperator1.Value;
          this.m_operator = (PredicateFieldComparisonOperator) containsOperator2;
        }
      }
    }

    public WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType NodeType
    {
      get => this.m_nodeType;
      set => this.m_nodeType = value;
    }

    public bool Inverted
    {
      get => this.m_inverted;
      set => this.m_inverted = value;
    }

    public PredicateFieldComparisonOperator Operator => this.m_operator;

    public List<WorkItemTypeExtensionPredicateNode> Children => this.m_children;

    public int FieldId => this.m_fieldId;

    public PredicateOperator ToPredicate()
    {
      PredicateOperator predicate1;
      if (this.m_nodeType == WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Leaf)
        predicate1 = (PredicateOperator) this.m_operator;
      else if (this.m_children == null || this.m_children.Count == 0)
      {
        predicate1 = (PredicateOperator) null;
      }
      else
      {
        PredicateOperator[] array = this.m_children.Select<WorkItemTypeExtensionPredicateNode, PredicateOperator>((Func<WorkItemTypeExtensionPredicateNode, PredicateOperator>) (pnw => pnw.ToPredicate())).Where<PredicateOperator>((Func<PredicateOperator, bool>) (op => op != null)).ToArray<PredicateOperator>();
        if (this.m_nodeType == WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And)
        {
          PredicateAndOperator predicateAndOperator = new PredicateAndOperator();
          predicateAndOperator.Operands = array;
          predicate1 = (PredicateOperator) predicateAndOperator;
        }
        else
        {
          PredicateOrOperator predicateOrOperator = new PredicateOrOperator();
          predicateOrOperator.Operands = array;
          predicate1 = (PredicateOperator) predicateOrOperator;
        }
      }
      if (!this.m_inverted || predicate1 == null)
        return predicate1;
      PredicateNotOperator predicate2 = new PredicateNotOperator();
      predicate2.Operand = predicate1;
      return (PredicateOperator) predicate2;
    }

    internal WorkItemTypeExtensionPredicateNode PushInvertsToLeaves() => this.PushInvertsToLeaves(false);

    private WorkItemTypeExtensionPredicateNode PushInvertsToLeaves(bool incomingInvert)
    {
      bool incomingInvert1 = incomingInvert ? !this.m_inverted : this.m_inverted;
      WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType nodeType = this.m_nodeType;
      switch (nodeType)
      {
        case WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And:
        case WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Or:
          if (this.m_children == null || this.m_children.Count == 0)
            return (WorkItemTypeExtensionPredicateNode) null;
          if (incomingInvert1)
            nodeType = nodeType == WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And ? WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Or : WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And;
          List<WorkItemTypeExtensionPredicateNode> children = new List<WorkItemTypeExtensionPredicateNode>(this.m_children.Count);
          foreach (WorkItemTypeExtensionPredicateNode child in this.m_children)
          {
            WorkItemTypeExtensionPredicateNode leaves = child.PushInvertsToLeaves(incomingInvert1);
            if (leaves != null)
              children.Add(leaves);
          }
          return new WorkItemTypeExtensionPredicateNode(nodeType, children)
          {
            Inverted = false
          };
        default:
          if (this.m_inverted == incomingInvert1)
            return this;
          return new WorkItemTypeExtensionPredicateNode(WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Leaf, this.m_operator)
          {
            Inverted = incomingInvert1
          };
      }
    }

    public WorkItemTypeExtensionPredicateNode[][] OrsOfAnds()
    {
      WorkItemTypeExtensionPredicateNode leaves = this.PushInvertsToLeaves();
      if (leaves != null)
      {
        WorkItemTypeExtensionPredicateNode extensionPredicateNode = leaves.OrsOfAndsInternal();
        if (extensionPredicateNode != null)
        {
          switch (extensionPredicateNode.m_nodeType)
          {
            case WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And:
              return new WorkItemTypeExtensionPredicateNode[1][]
              {
                extensionPredicateNode.m_children.ToArray()
              };
            case WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Or:
              return extensionPredicateNode.m_children.Select<WorkItemTypeExtensionPredicateNode, WorkItemTypeExtensionPredicateNode[]>((Func<WorkItemTypeExtensionPredicateNode, WorkItemTypeExtensionPredicateNode[]>) (node =>
              {
                if (node.m_nodeType == WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And)
                  return node.m_children.ToArray();
                return new WorkItemTypeExtensionPredicateNode[1]
                {
                  node
                };
              })).ToArray<WorkItemTypeExtensionPredicateNode[]>();
            default:
              return new WorkItemTypeExtensionPredicateNode[1][]
              {
                new WorkItemTypeExtensionPredicateNode[1]
                {
                  extensionPredicateNode
                }
              };
          }
        }
      }
      return Array.Empty<WorkItemTypeExtensionPredicateNode[]>();
    }

    internal WorkItemTypeExtensionPredicateNode OrsOfAndsInternal()
    {
      switch (this.m_nodeType)
      {
        case WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And:
        case WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Or:
          List<WorkItemTypeExtensionPredicateNode> children1 = new List<WorkItemTypeExtensionPredicateNode>(this.m_children.Count << 1);
          foreach (WorkItemTypeExtensionPredicateNode child in this.m_children)
          {
            WorkItemTypeExtensionPredicateNode extensionPredicateNode = child.OrsOfAndsInternal();
            if (extensionPredicateNode.m_nodeType == this.m_nodeType)
              children1.AddRange((IEnumerable<WorkItemTypeExtensionPredicateNode>) extensionPredicateNode.m_children);
            else
              children1.Add(extensionPredicateNode);
          }
          if (this.m_nodeType == WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And)
          {
            List<WorkItemTypeExtensionPredicateNode> children2 = new List<WorkItemTypeExtensionPredicateNode>(children1.Count);
            List<WorkItemTypeExtensionPredicateNode> extensionPredicateNodeList = new List<WorkItemTypeExtensionPredicateNode>(children1.Count);
            foreach (WorkItemTypeExtensionPredicateNode extensionPredicateNode in children1)
            {
              if (extensionPredicateNode.m_nodeType == WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Or)
                extensionPredicateNodeList.Add(extensionPredicateNode);
              else
                children2.Add(extensionPredicateNode);
            }
            if (extensionPredicateNodeList.Count > 0)
            {
              WorkItemTypeExtensionPredicateNode extensionPredicateNode1 = new WorkItemTypeExtensionPredicateNode(WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And, children2);
              foreach (WorkItemTypeExtensionPredicateNode extensionPredicateNode2 in extensionPredicateNodeList)
              {
                List<WorkItemTypeExtensionPredicateNode> children3 = new List<WorkItemTypeExtensionPredicateNode>(extensionPredicateNode2.m_children.Count << 1);
                foreach (WorkItemTypeExtensionPredicateNode child in extensionPredicateNode2.m_children)
                {
                  WorkItemTypeExtensionPredicateNode extensionPredicateNode3 = extensionPredicateNode1.And(child);
                  if (extensionPredicateNode3 != null && extensionPredicateNode3.m_nodeType == WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Or)
                    children3.AddRange((IEnumerable<WorkItemTypeExtensionPredicateNode>) extensionPredicateNode3.m_children);
                  else
                    children3.Add(extensionPredicateNode3);
                }
                extensionPredicateNode1 = new WorkItemTypeExtensionPredicateNode(WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Or, children3);
              }
              return extensionPredicateNode1;
            }
          }
          return new WorkItemTypeExtensionPredicateNode(this.m_nodeType, children1);
        default:
          return this;
      }
    }

    private WorkItemTypeExtensionPredicateNode And(WorkItemTypeExtensionPredicateNode node)
    {
      switch (this.m_nodeType)
      {
        case WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And:
          List<WorkItemTypeExtensionPredicateNode> children1 = new List<WorkItemTypeExtensionPredicateNode>(this.m_children.Count << 1);
          children1.AddRange((IEnumerable<WorkItemTypeExtensionPredicateNode>) this.m_children);
          if (node.m_nodeType == WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And)
            children1.AddRange((IEnumerable<WorkItemTypeExtensionPredicateNode>) node.m_children);
          else
            children1.Add(node);
          return new WorkItemTypeExtensionPredicateNode(WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And, children1);
        case WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Or:
          List<WorkItemTypeExtensionPredicateNode> children2 = new List<WorkItemTypeExtensionPredicateNode>(this.m_children.Count);
          foreach (WorkItemTypeExtensionPredicateNode child in this.m_children)
            children2.Add(child.And(node));
          return new WorkItemTypeExtensionPredicateNode(WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.Or, children2);
        default:
          return new WorkItemTypeExtensionPredicateNode(WorkItemTypeExtensionPredicateNode.WorkItemTypeExtensionPredicateNodeType.And, new List<WorkItemTypeExtensionPredicateNode>(2)
          {
            this,
            node
          });
      }
    }

    internal enum WorkItemTypeExtensionPredicateNodeType
    {
      Leaf,
      And,
      Or,
    }
  }
}
