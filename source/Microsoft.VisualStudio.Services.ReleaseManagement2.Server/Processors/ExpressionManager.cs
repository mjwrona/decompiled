// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.ExpressionManager
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "By design")]
  public sealed class ExpressionManager
  {
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "By design")]
    public sealed class FailedNode : FunctionNode
    {
      protected override sealed object EvaluateCore(EvaluationContext evaluationContext)
      {
        DeployPhaseExecutionState state = evaluationContext != null ? evaluationContext.State as DeployPhaseExecutionState : throw new ArgumentNullException(nameof (evaluationContext));
        int currentRank = state.PhaseSnapshot.Rank;
        int num = state.Environment.ReleaseDeployPhases.Where<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (x => x.Attempt == state.EngineInput.AttemptNumber && x.Rank < currentRank)).Any<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (z => z.Status == DeployPhaseStatus.Failed || z.Status == DeployPhaseStatus.Canceled)) ? 1 : 0;
        bool flag = state.Environment.Status == ReleaseEnvironmentStatus.Canceled;
        return (object) (bool) (num == 0 ? 0 : (!flag ? 1 : 0));
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "By design")]
    public sealed class SucceededNode : FunctionNode
    {
      protected override sealed object EvaluateCore(EvaluationContext evaluationContext)
      {
        DeployPhaseExecutionState state = evaluationContext != null ? evaluationContext.State as DeployPhaseExecutionState : throw new ArgumentNullException(nameof (evaluationContext));
        int currentRank = state.PhaseSnapshot.Rank;
        int num = state.Environment.ReleaseDeployPhases.Where<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (x => x.Attempt == state.EngineInput.AttemptNumber && x.Rank < currentRank)).Any<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (z => z.Status == DeployPhaseStatus.Failed || z.Status == DeployPhaseStatus.Canceled)) ? 1 : 0;
        bool flag = state.Environment.Status == ReleaseEnvironmentStatus.Canceled;
        return (object) (bool) (num != 0 ? 0 : (!flag ? 1 : 0));
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "By design")]
    public sealed class SucceededOrFailedNode : FunctionNode
    {
      protected override sealed object EvaluateCore(EvaluationContext evaluationContext)
      {
        if (evaluationContext == null)
          throw new ArgumentNullException(nameof (evaluationContext));
        return (object) ((evaluationContext.State as DeployPhaseExecutionState).Environment.Status != ReleaseEnvironmentStatus.Canceled);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "By design")]
    public sealed class VariablesNode : NamedValueNode
    {
      protected override sealed object EvaluateCore(EvaluationContext evaluationContext)
      {
        DeployPhaseExecutionState phaseExecutionState = evaluationContext != null ? evaluationContext.State as DeployPhaseExecutionState : throw new ArgumentNullException(nameof (evaluationContext));
        return (object) new ExpressionManager.VariablesDictionary(phaseExecutionState.EngineInput.Variables, phaseExecutionState.SystemVariables);
      }
    }

    private sealed class VariablesDictionary : 
      IReadOnlyDictionary<string, object>,
      IReadOnlyCollection<KeyValuePair<string, object>>,
      IEnumerable<KeyValuePair<string, object>>,
      IEnumerable
    {
      private readonly Dictionary<string, ConfigurationVariableValue> variables;

      public VariablesDictionary(
        Dictionary<string, ConfigurationVariableValue> variables,
        IDictionary<string, string> systemVariables)
      {
        this.variables = variables;
        systemVariables.ForEach<KeyValuePair<string, string>>((Action<KeyValuePair<string, string>>) (systemVariable => this.variables[systemVariable.Key] = new ConfigurationVariableValue()
        {
          Value = systemVariable.Value
        }));
      }

      public object this[string key] => (object) this.variables.GetValueOrDefault<string, ConfigurationVariableValue>(key, (ConfigurationVariableValue) null);

      public IEnumerable<string> Keys => throw new NotSupportedException();

      public IEnumerable<object> Values => throw new NotSupportedException();

      public bool ContainsKey(string key) => this.variables.TryGetValue(key, out ConfigurationVariableValue _);

      public bool TryGetValue(string key, out object value)
      {
        if (this.variables.ContainsKey(key) && this.variables[key].IsSecret)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.PhaseConditionSecretVariableError, (object) key));
        ConfigurationVariableValue configurationVariableValue;
        bool flag = this.variables.TryGetValue(key, out configurationVariableValue);
        value = flag ? (object) configurationVariableValue.Value : (object) string.Empty;
        return flag;
      }

      public int Count => throw new NotSupportedException();

      IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => throw new NotSupportedException();

      IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
    }
  }
}
