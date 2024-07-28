// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.OrchestrationUtilities
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal static class OrchestrationUtilities
  {
    public static bool TryParseSessionId(string sessionId, out Guid planId, out string instanceId)
    {
      planId = Guid.Empty;
      instanceId = (string) null;
      if (string.IsNullOrEmpty(sessionId))
        return false;
      int length = sessionId.IndexOf('.', 0, Math.Min(sessionId.Length, 37));
      if (length < 0)
      {
        length = sessionId.IndexOf('_', 0, Math.Min(sessionId.Length, 37));
        if (length < 0)
          length = sessionId.Length;
      }
      if (!Guid.TryParse(sessionId.Substring(0, length), out planId) || planId.Equals(Guid.Empty))
        return false;
      if (length < sessionId.Length)
        instanceId = sessionId.Substring(length + 1);
      return true;
    }
  }
}
