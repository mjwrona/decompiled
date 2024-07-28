// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.PhaseConditionEvaluator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Logging;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  internal class PhaseConditionEvaluator
  {
    public PhaseConditionResult ShouldExecutePhase(
      IVssRequestContext requestContext,
      ReleaseEnvironment releaseEnvironment,
      DeployPhaseSnapshot deployPhaseSnapshot,
      AutomationEngineInput automationEngineInput,
      IDictionary<string, string> systemVariables)
    {
      INamedValueInfo[] namedValues = new INamedValueInfo[1]
      {
        (INamedValueInfo) new NamedValueInfo<ExpressionManager.VariablesNode>(PhaseConditionConstants.Expressions.Variables)
      };
      IFunctionInfo[] functions = new IFunctionInfo[3]
      {
        (IFunctionInfo) new FunctionInfo<ExpressionManager.SucceededNode>(PhaseConditionConstants.Expressions.Succeeded, 0, 0),
        (IFunctionInfo) new FunctionInfo<ExpressionManager.SucceededOrFailedNode>(PhaseConditionConstants.Expressions.SucceededOrFailed, 0, 0),
        (IFunctionInfo) new FunctionInfo<ExpressionManager.FailedNode>(PhaseConditionConstants.Expressions.Failed, 0, 0)
      };
      IExpressionNode tree = new ExpressionParser().CreateTree(deployPhaseSnapshot.GetDeploymentInput() == null ? "succeeded()" : (string.IsNullOrWhiteSpace(deployPhaseSnapshot.GetDeploymentInput().Condition) ? "succeeded()" : deployPhaseSnapshot.GetDeploymentInput().Condition), (ITraceWriter) new PhaseConditionTracer(requestContext), (IEnumerable<INamedValueInfo>) namedValues, (IEnumerable<IFunctionInfo>) functions);
      DeployPhaseExecutionState phaseExecutionState = new DeployPhaseExecutionState()
      {
        Environment = releaseEnvironment,
        PhaseSnapshot = deployPhaseSnapshot,
        EngineInput = automationEngineInput,
        SystemVariables = systemVariables
      };
      PhaseConditionTracer phaseConditionTracer = new PhaseConditionTracer(requestContext);
      PhaseConditionTracer trace = phaseConditionTracer;
      DeployPhaseExecutionState state = phaseExecutionState;
      bool boolean = tree.EvaluateBoolean((ITraceWriter) trace, (ISecretMasker) null, (object) state);
      return new PhaseConditionResult()
      {
        Value = boolean,
        Message = phaseConditionTracer.Message
      };
    }
  }
}
