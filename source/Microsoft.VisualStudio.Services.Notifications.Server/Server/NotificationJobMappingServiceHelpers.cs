// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationJobMappingServiceHelpers
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class NotificationJobMappingServiceHelpers
  {
    public static Guid GetProcessingJobId(
      this Dictionary<Tuple<string, string>, Guid> mapping,
      string processQueue)
    {
      Guid empty = Guid.Empty;
      mapping.TryGetValue(new Tuple<string, string>(processQueue, string.Empty), out empty);
      return empty;
    }

    public static Guid GetDeliveryJobId(
      this Dictionary<Tuple<string, string>, Guid> mapping,
      string processQueue,
      string channel)
    {
      Guid empty = Guid.Empty;
      mapping.TryGetValue(new Tuple<string, string>(processQueue, channel), out empty);
      return empty;
    }

    public static HashSet<string> GetSupportedProcessingJobProcessQueues(
      this Dictionary<Tuple<string, string>, Guid> mapping,
      Guid jobId)
    {
      HashSet<string> jobProcessQueues = new HashSet<string>();
      foreach (KeyValuePair<Tuple<string, string>, Guid> keyValuePair in mapping)
      {
        if (keyValuePair.Value.Equals(jobId))
          jobProcessQueues.Add(keyValuePair.Key.Item1);
      }
      return jobProcessQueues;
    }

    public static HashSet<Tuple<string, string>> GetSupportedDeliveryJobProcessQueuesAndChannels(
      this Dictionary<Tuple<string, string>, Guid> mapping,
      Guid jobId)
    {
      HashSet<Tuple<string, string>> queuesAndChannels = new HashSet<Tuple<string, string>>();
      foreach (KeyValuePair<Tuple<string, string>, Guid> keyValuePair in mapping)
      {
        if (keyValuePair.Value.Equals(jobId))
          queuesAndChannels.Add(keyValuePair.Key);
      }
      return queuesAndChannels;
    }
  }
}
