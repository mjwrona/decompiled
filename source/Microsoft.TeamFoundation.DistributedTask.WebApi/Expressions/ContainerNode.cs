// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.ContainerNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class ContainerNode : ExpressionNode
  {
    private readonly List<ExpressionNode> m_parameters = new List<ExpressionNode>();

    public IReadOnlyList<ExpressionNode> Parameters => (IReadOnlyList<ExpressionNode>) this.m_parameters.AsReadOnly();

    public void AddParameter(ExpressionNode node)
    {
      this.m_parameters.Add(node);
      node.Container = this;
    }

    public void ReplaceParameter(int index, ExpressionNode node)
    {
      this.m_parameters[index] = node;
      node.Container = this;
    }

    public override IEnumerable<T> GetParameters<T>()
    {
      List<T> parameters = new List<T>();
      Queue<IExpressionNode> expressionNodeQueue = new Queue<IExpressionNode>((IEnumerable<IExpressionNode>) this.Parameters);
      while (expressionNodeQueue.Count > 0)
      {
        IExpressionNode expressionNode = expressionNodeQueue.Dequeue();
        if (typeof (T).GetTypeInfo().IsAssignableFrom(expressionNode.GetType().GetTypeInfo()))
          parameters.Add((T) expressionNode);
        foreach (T parameter in expressionNode.GetParameters<T>())
          expressionNodeQueue.Enqueue((IExpressionNode) parameter);
      }
      return (IEnumerable<T>) parameters;
    }
  }
}
