// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.NoneExecutionHandler
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public class NoneExecutionHandler : ExecutionHandler
  {
    public NoneExecutionHandler(NoneExecutionInput input, Action<string> traceMethod)
      : base((ExecutionInput) input, traceMethod)
    {
    }

    public override IDictionary<string, string> GetInvalidInputs(
      IDictionary<string, ConfigurationVariableValue> variables,
      IVssRequestContext context)
    {
      return (IDictionary<string, string>) new Dictionary<string, string>();
    }

    protected override void Apply(PlanEnvironment environment, TaskOrchestrationContainer container)
    {
    }

    protected override void UpdateContainerProperties(TaskOrchestrationContainer container)
    {
    }
  }
}
