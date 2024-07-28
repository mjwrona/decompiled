// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.GraphCondition`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class GraphCondition<TInstance> where TInstance : IGraphNodeInstance
  {
    private readonly string m_condition;
    private readonly ExpressionParser m_parser;
    protected readonly IExpressionNode m_parsedCondition;
    private readonly Lazy<bool> m_requiresOutputs;
    private readonly Lazy<bool> m_requiresVariables;
    private static readonly INamedValueInfo[] s_namedValueInfo = new INamedValueInfo[3]
    {
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ExpressionConstants.PipelineNamedValue,
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ExpressionConstants.VariablesNamedValue,
      (INamedValueInfo) new NamedValueInfo<DependenciesContextNode<TInstance>>(Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ExpressionConstants.Dependencies)
    };
    private static readonly IFunctionInfo[] s_functionInfo = new IFunctionInfo[5]
    {
      (IFunctionInfo) new FunctionInfo<GraphCondition<TInstance>.AlwaysNode>("always", 0, 0),
      (IFunctionInfo) new FunctionInfo<GraphCondition<TInstance>.FailedNode>("failed", 0, int.MaxValue),
      (IFunctionInfo) new FunctionInfo<GraphCondition<TInstance>.CanceledNode>("canceled", 0, 0),
      (IFunctionInfo) new FunctionInfo<GraphCondition<TInstance>.SucceededNode>("succeeded", 0, int.MaxValue),
      (IFunctionInfo) new FunctionInfo<GraphCondition<TInstance>.SucceededOrFailedNode>("succeededOrFailed", 0, int.MaxValue)
    };

    private protected GraphCondition(string condition)
    {
      this.m_condition = !string.IsNullOrEmpty(condition) ? condition : GraphCondition<TInstance>.Default;
      this.m_parser = new ExpressionParser();
      this.m_parsedCondition = this.m_parser.CreateTree(this.m_condition, (ITraceWriter) new GraphCondition<TInstance>.ConditionTraceWriter(), this.GetSupportedNamedValues(), (IEnumerable<IFunctionInfo>) GraphCondition<TInstance>.s_functionInfo);
      this.m_requiresOutputs = new Lazy<bool>(new Func<bool>(this.HasOutputsReferences));
      this.m_requiresVariables = new Lazy<bool>(new Func<bool>(this.HasVariablesReference));
    }

    public static string Default => "succeeded()";

    public bool RequiresOutputs => this.m_requiresOutputs.Value;

    public bool RequiresVariables => this.m_requiresVariables.Value;

    private bool HasOutputsReferences()
    {
      if (this.m_parsedCondition.GetParameters<StageDependenciesContextNode>().Any<StageDependenciesContextNode>() || this.RequiresVariables)
        return true;
      List<DependenciesContextNode<TInstance>> list = this.m_parsedCondition.GetParameters<DependenciesContextNode<TInstance>>().ToList<DependenciesContextNode<TInstance>>();
      if (list.Count == 0)
        return false;
      foreach (ExpressionNode expressionNode in list)
      {
        Microsoft.TeamFoundation.DistributedTask.Expressions.ContainerNode container = expressionNode.Container?.Container;
        if (container != null)
        {
          LiteralValueNode literalValueNode = container.Parameters.OfType<LiteralValueNode>().FirstOrDefault<LiteralValueNode>();
          if (literalValueNode != null && literalValueNode.Kind == ValueKind.String && ((string) literalValueNode.Value).Equals("outputs", StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    private bool HasVariablesReference() => this.m_parsedCondition.GetParameters<VariablesContextNode>().Any<VariablesContextNode>();

    private static IEnumerable<IGraphNodeInstance> GetNodesForEvaluation(
      FunctionNode function,
      EvaluationContext context,
      GraphExecutionContext<TInstance> expressionContext)
    {
      if (function.Parameters.Count == 0)
      {
        foreach (KeyValuePair<string, TInstance> dependency in (IEnumerable<KeyValuePair<string, TInstance>>) expressionContext.Dependencies)
          yield return (IGraphNodeInstance) dependency.Value;
      }
      else
      {
        foreach (ExpressionNode parameter in (IEnumerable<ExpressionNode>) function.Parameters)
        {
          TInstance instance;
          if (!expressionContext.Dependencies.TryGetValue(parameter.EvaluateString(context), out instance))
            yield return (IGraphNodeInstance) null;
          else
            yield return (IGraphNodeInstance) instance;
        }
      }
    }

    protected virtual IEnumerable<INamedValueInfo> GetSupportedNamedValues()
    {
      INamedValueInfo[] namedValueInfoArray = GraphCondition<TInstance>.s_namedValueInfo;
      for (int index = 0; index < namedValueInfoArray.Length; ++index)
        yield return namedValueInfoArray[index];
      namedValueInfoArray = (INamedValueInfo[]) null;
    }

    protected sealed class ConditionTraceWriter : ITraceWriter
    {
      private StringBuilder m_info = new StringBuilder();

      public string Trace => this.m_info.ToString();

      public void Info(string message) => this.m_info.AppendLine(message);

      public void Verbose(string message)
      {
      }
    }

    private sealed class AlwaysNode : FunctionNode
    {
      protected override object EvaluateCore(EvaluationContext context) => (object) true;
    }

    private sealed class CanceledNode : FunctionNode
    {
      protected override object EvaluateCore(EvaluationContext context) => (object) ((context.State as GraphExecutionContext<TInstance>).State == PipelineState.Canceling);
    }

    private sealed class FailedNode : FunctionNode
    {
      protected override object EvaluateCore(EvaluationContext context)
      {
        GraphExecutionContext<TInstance> state = context.State as GraphExecutionContext<TInstance>;
        if (state.State != PipelineState.InProgress)
          return (object) false;
        bool core = false;
        foreach (IGraphNodeInstance graphNodeInstance in GraphCondition<TInstance>.GetNodesForEvaluation((FunctionNode) this, context, state))
        {
          if (graphNodeInstance == null)
            return (object) false;
          TaskResult? result = graphNodeInstance.Result;
          TaskResult taskResult = TaskResult.Failed;
          if (result.GetValueOrDefault() == taskResult & result.HasValue)
          {
            core = true;
            break;
          }
        }
        return (object) core;
      }
    }

    private sealed class SucceededNode : FunctionNode
    {
      protected override object EvaluateCore(EvaluationContext context)
      {
        GraphExecutionContext<TInstance> state = context.State as GraphExecutionContext<TInstance>;
        if (state.State != PipelineState.InProgress)
          return (object) false;
        bool core = true;
        foreach (IGraphNodeInstance graphNodeInstance in GraphCondition<TInstance>.GetNodesForEvaluation((FunctionNode) this, context, state))
        {
          if (!core || graphNodeInstance == null)
            return (object) false;
          int num1 = core ? 1 : 0;
          TaskResult? result = graphNodeInstance.Result;
          TaskResult taskResult1 = TaskResult.Succeeded;
          int num2;
          if (!(result.GetValueOrDefault() == taskResult1 & result.HasValue))
          {
            result = graphNodeInstance.Result;
            TaskResult taskResult2 = TaskResult.SucceededWithIssues;
            num2 = result.GetValueOrDefault() == taskResult2 & result.HasValue ? 1 : 0;
          }
          else
            num2 = 1;
          core = (num1 & num2) != 0;
        }
        return (object) core;
      }
    }

    private sealed class SucceededOrFailedNode : FunctionNode
    {
      protected override object EvaluateCore(EvaluationContext context)
      {
        GraphExecutionContext<TInstance> state = context.State as GraphExecutionContext<TInstance>;
        if (state.State != PipelineState.InProgress)
          return (object) false;
        bool flag1 = false;
        bool flag2 = true;
        foreach (IGraphNodeInstance graphNodeInstance in GraphCondition<TInstance>.GetNodesForEvaluation((FunctionNode) this, context, state))
        {
          if (graphNodeInstance == null)
            return (object) false;
          TaskResult? result1 = graphNodeInstance.Result;
          TaskResult taskResult1 = TaskResult.Failed;
          if (result1.GetValueOrDefault() == taskResult1 & result1.HasValue)
          {
            flag1 = true;
            break;
          }
          TaskResult? result2 = graphNodeInstance.Result;
          TaskResult taskResult2 = TaskResult.Succeeded;
          int num;
          if (!(result2.GetValueOrDefault() == taskResult2 & result2.HasValue))
          {
            TaskResult? result3 = graphNodeInstance.Result;
            TaskResult taskResult3 = TaskResult.SucceededWithIssues;
            num = result3.GetValueOrDefault() == taskResult3 & result3.HasValue ? 1 : 0;
          }
          else
            num = 1;
          flag2 = num != 0;
        }
        return (object) (flag1 | flag2);
      }
    }
  }
}
