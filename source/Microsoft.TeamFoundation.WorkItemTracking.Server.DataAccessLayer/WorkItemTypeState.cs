// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeState
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTypeState
  {
    public WorkItemTypeState(string workItemType, string state)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(workItemType, nameof (workItemType));
      ArgumentUtility.CheckStringForNullOrEmpty(state, nameof (state));
      this.State = state;
      this.WorkItemType = workItemType;
    }

    internal WorkItemTypeState()
    {
    }

    public string State { get; internal set; }

    public string WorkItemType { get; internal set; }
  }
}
