// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemDataset
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  internal class WorkItemDataset : WorkItemRevisionDataset
  {
    public WorkItemDataset() => this.Revisions = new List<WorkItemRevisionDataset>();

    public List<WorkItemRevisionDataset> Revisions { get; private set; }

    internal static WorkItemRevisionDataset CreateFrom(IDictionary<int, object> latestData)
    {
      WorkItemRevisionDataset from = new WorkItemRevisionDataset();
      if (latestData != null)
      {
        foreach (KeyValuePair<int, object> keyValuePair in (IEnumerable<KeyValuePair<int, object>>) latestData)
          from.Fields[keyValuePair.Key] = keyValuePair.Value;
      }
      return from;
    }
  }
}
