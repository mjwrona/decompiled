// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeAction
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTypeAction
  {
    public WorkItemTypeAction(string workItemType, string name, string fromState, string toState)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(workItemType, nameof (workItemType));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(fromState, nameof (fromState));
      ArgumentUtility.CheckStringForNullOrEmpty(toState, nameof (toState));
      this.Name = name;
      this.FromState = fromState;
      this.ToState = toState;
      this.WorkItemType = workItemType;
    }

    internal WorkItemTypeAction()
    {
    }

    public string FromState { get; internal set; }

    public string ToState { get; internal set; }

    public string Name { get; internal set; }

    public string WorkItemType { get; internal set; }
  }
}
