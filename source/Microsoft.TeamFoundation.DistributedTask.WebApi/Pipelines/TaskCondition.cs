// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.TaskCondition
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
  public class TaskCondition
  {
    private readonly string m_condition;
    private readonly ExpressionParser m_parser;
    private readonly IExpressionNode m_parsedCondition;
    private readonly Lazy<bool> m_requiresVariables;
    private static readonly INamedValueInfo[] s_namedValueInfo = new INamedValueInfo[1]
    {
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ExpressionConstants.VariablesNamedValue
    };
    private static readonly IFunctionInfo[] s_functionInfo = new IFunctionInfo[5]
    {
      (IFunctionInfo) new FunctionInfo<TaskCondition.AlwaysNode>("always", 0, 0),
      (IFunctionInfo) new FunctionInfo<TaskCondition.FailedNode>("failed", 0, 0),
      (IFunctionInfo) new FunctionInfo<TaskCondition.CanceledNode>("canceled", 0, 0),
      (IFunctionInfo) new FunctionInfo<TaskCondition.SucceededNode>("succeeded", 0, 0),
      (IFunctionInfo) new FunctionInfo<TaskCondition.SucceededOrFailedNode>("succeededOrFailed", 0, 0)
    };

    public TaskCondition(string condition)
    {
      this.m_condition = condition ?? TaskCondition.Default;
      this.m_parser = new ExpressionParser();
      this.m_parsedCondition = this.m_parser.CreateTree(this.m_condition, (ITraceWriter) new TaskCondition.ConditionTraceWriter(), (IEnumerable<INamedValueInfo>) TaskCondition.s_namedValueInfo, (IEnumerable<IFunctionInfo>) TaskCondition.s_functionInfo);
      this.m_requiresVariables = new Lazy<bool>(new Func<bool>(this.HasVariablesReference));
    }

    public static string Default => "succeeded()";

    public bool RequiresVariables => this.m_requiresVariables.Value;

    public ConditionResult Evaluate(JobExecutionContext context)
    {
      TaskCondition.ConditionTraceWriter trace = new TaskCondition.ConditionTraceWriter();
      bool flag = this.m_parsedCondition.Evaluate<bool>((ITraceWriter) trace, context.SecretMasker, (object) context, context.ExpressionOptions);
      return new ConditionResult()
      {
        Value = flag,
        Trace = trace.Trace
      };
    }

    private bool HasVariablesReference() => this.m_parsedCondition.GetParameters<VariablesContextNode>().Any<VariablesContextNode>();

    private sealed class ConditionTraceWriter : ITraceWriter
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
      protected override object EvaluateCore(EvaluationContext context) => (object) ((context.State as JobExecutionContext).State == PipelineState.Canceling);
    }

    private sealed class FailedNode : FunctionNode
    {
      protected override object EvaluateCore(EvaluationContext context)
      {
        JobExecutionContext state = context.State as JobExecutionContext;
        if (state.State != PipelineState.InProgress)
          return (object) false;
        VariableValue variableValue;
        TaskResult result;
        return !state.Variables.TryGetValue(WellKnownDistributedTaskVariables.JobStatus, out variableValue) || !Enum.TryParse<TaskResult>(variableValue.Value, true, out result) ? (object) false : (object) (result == TaskResult.Failed);
      }
    }

    private sealed class SucceededNode : FunctionNode
    {
      protected override object EvaluateCore(EvaluationContext context)
      {
        JobExecutionContext state = context.State as JobExecutionContext;
        if (state.State != PipelineState.InProgress)
          return (object) false;
        VariableValue variableValue;
        TaskResult result;
        return !state.Variables.TryGetValue(WellKnownDistributedTaskVariables.JobStatus, out variableValue) || !Enum.TryParse<TaskResult>(variableValue.Value, true, out result) ? (object) false : (object) (bool) (result == TaskResult.Succeeded ? 1 : (result == TaskResult.SucceededWithIssues ? 1 : 0));
      }
    }

    private sealed class SucceededOrFailedNode : FunctionNode
    {
      protected override object EvaluateCore(EvaluationContext context) => (object) ((context.State as JobExecutionContext).State != PipelineState.Canceling);
    }
  }
}
