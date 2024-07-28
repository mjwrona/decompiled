// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.OrchestrationContextExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

using Microsoft.VisualStudio.Services.Orchestration;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  public static class OrchestrationContextExtensions
  {
    public static void TraceInfo(this OrchestrationContext context, string message) => context.Trace(0, TraceLevel.Info, message);

    public static void TraceWarning(this OrchestrationContext context, string message) => context.Trace(0, TraceLevel.Warning, message);

    public static void TraceError(this OrchestrationContext context, string message) => context.Trace(0, TraceLevel.Error, message);
  }
}
