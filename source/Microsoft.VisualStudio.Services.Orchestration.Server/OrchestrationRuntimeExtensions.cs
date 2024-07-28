// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationRuntimeExtensions
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class OrchestrationRuntimeExtensions
  {
    public static OrchestrationRuntime AddTaskActivitiesFromInterface<TActivity>(
      this OrchestrationRuntime runtime,
      IVssRequestContext requestContext,
      Func<IVssRequestContext, TActivity> activityFactory,
      bool useFullyQualifiedMethodNames)
    {
      return runtime.AddTaskActivitiesFromInterface<TActivity>((ITaskActivityInvoker) new VssTaskActivityInvoker<TActivity>(requestContext.ServiceHost.DeploymentServiceHost, requestContext.ServiceHost.InstanceId, activityFactory, requestContext.CancellationToken), useFullyQualifiedMethodNames);
    }
  }
}
