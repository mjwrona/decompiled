// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.ServerTaskExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public static class ServerTaskExtensions
  {
    public static string GetServerTaskOrchestrationId(Guid planId, Guid jobId, Guid taskId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:N}_{1:N}_{2:N}", (object) planId, (object) jobId, (object) taskId);
  }
}
