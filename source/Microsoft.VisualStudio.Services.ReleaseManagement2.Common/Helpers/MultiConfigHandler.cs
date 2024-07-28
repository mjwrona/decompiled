// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.MultiConfigHandler
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Well known word in VSTS")]
  public class MultiConfigHandler : ParallelExecutionHandlerBase
  {
    private readonly MultiConfigInput input;

    public MultiConfigHandler(MultiConfigInput input, Action<string> traceMethod)
      : base((ParallelExecutionInputBase) input, traceMethod)
    {
      this.input = input;
    }

    public override IDictionary<string, string> GetInvalidInputs(
      IDictionary<string, ConfigurationVariableValue> variables,
      IVssRequestContext context)
    {
      IDictionary<string, string> invalidInputs = (IDictionary<string, string>) new Dictionary<string, string>();
      if (!this.IsMultiplierValueValid(variables))
        invalidInputs["Multipliers"] = this.input.Multipliers;
      foreach (KeyValuePair<string, string> invalidInput in (IEnumerable<KeyValuePair<string, string>>) base.GetInvalidInputs(variables, context))
      {
        if (!invalidInputs.ContainsKey(invalidInput.Key))
          invalidInputs[invalidInput.Key] = invalidInput.Value;
      }
      return invalidInputs;
    }

    protected override void Apply(PlanEnvironment environment, TaskOrchestrationContainer container) => this.ApplyMultiplier(environment, container);

    private bool IsMultiplierValueValid(
      IDictionary<string, ConfigurationVariableValue> variables)
    {
      ICollection<string> multiplierVariables = this.input.GetMultiplierVariables();
      if (multiplierVariables.Count == 0 || variables == null || variables.Count == 0)
        return true;
      foreach (string key in (IEnumerable<string>) multiplierVariables)
      {
        ConfigurationVariableValue configurationVariableValue;
        if (variables.TryGetValue(key, out configurationVariableValue) && configurationVariableValue.IsSecret)
          return false;
      }
      return true;
    }

    private void ApplyMultiplier(PlanEnvironment environment, TaskOrchestrationContainer container)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (container == null)
        throw new ArgumentNullException(nameof (container));
      IDictionary<string, IList<string>> multiplierValues = MultiplierHelpers.GetMultiplierValues(this.input, environment.Variables, false);
      if (!multiplierValues.Any<KeyValuePair<string, IList<string>>>())
        return;
      List<TaskOrchestrationJob> list = container.Children.Select<TaskOrchestrationItem, TaskOrchestrationJob>((Func<TaskOrchestrationItem, TaskOrchestrationJob>) (x => x as TaskOrchestrationJob)).ToList<TaskOrchestrationJob>();
      container.Children.RemoveAll((Predicate<TaskOrchestrationItem>) (x => x is TaskOrchestrationJob));
      this.TraceInfo(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Multiplier : Number of Multipliers {0}", (object) multiplierValues.Count));
      foreach (List<KeyValuePair<string, string>> combination in (IEnumerable<IList<KeyValuePair<string, string>>>) MultiplierHelpers.ComputeCombinations(multiplierValues))
      {
        foreach (TaskOrchestrationJob orchestrationJob1 in list)
        {
          TaskOrchestrationJob orchestrationJob2 = orchestrationJob1.Clone();
          orchestrationJob2.InstanceId = Guid.NewGuid();
          orchestrationJob2.Name = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Multiplier, (object) string.Join(",", combination.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (pair => pair.Value))));
          foreach (TaskInstance task in orchestrationJob2.Tasks)
            task.InstanceId = Guid.NewGuid();
          foreach (KeyValuePair<string, string> keyValuePair in combination)
            orchestrationJob2.Variables[keyValuePair.Key] = keyValuePair.Value;
          this.TraceInfo(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Multiplier : New Job created {0}", (object) orchestrationJob2.InstanceId));
          container.Children.Add((TaskOrchestrationItem) orchestrationJob2);
        }
      }
      this.TraceInfo(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Multiplier : New Job created {0}", (object) container.Children.Count));
    }
  }
}
