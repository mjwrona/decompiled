// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.ParallelExecutionHandlerBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public abstract class ParallelExecutionHandlerBase : ExecutionHandler
  {
    private ParallelExecutionInputBase input;

    protected ParallelExecutionHandlerBase(
      ParallelExecutionInputBase input,
      Action<string> traceMethod)
      : base((ExecutionInput) input, traceMethod)
    {
      this.input = input;
    }

    public override IDictionary<string, string> GetInvalidInputs(
      IDictionary<string, ConfigurationVariableValue> variables,
      IVssRequestContext context)
    {
      IDictionary<string, string> invalidInputs = (IDictionary<string, string>) new Dictionary<string, string>();
      if (this.input.MaxNumberOfAgents < 1 || this.input.MaxNumberOfAgents > ParallelExecutionHandlerBase.GetMaxNumberOfAgent(context))
        invalidInputs["MaxNumberOfAgents"] = this.input.MaxNumberOfAgents.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      return invalidInputs;
    }

    protected override void UpdateContainerProperties(TaskOrchestrationContainer container)
    {
      if (container == null)
        throw new ArgumentNullException(nameof (container));
      container.MaxConcurrency = this.input.MaxNumberOfAgents;
      container.Parallel = this.input.Parallel;
      container.ContinueOnError = this.input.ContinueOnError;
    }

    private static int GetMaxNumberOfAgent(IVssRequestContext context) => context.GetService<IVssRegistryService>().GetValue<int>(context, (RegistryQuery) "/Service/ReleaseManagement/ReleaseDefinition/PhaseSettings/MaxNumberOfAgents", 99);
  }
}
