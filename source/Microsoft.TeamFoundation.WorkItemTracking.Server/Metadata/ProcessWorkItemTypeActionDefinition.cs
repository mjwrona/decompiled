// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessWorkItemTypeActionDefinition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ProcessWorkItemTypeActionDefinition
  {
    public ProcessWorkItemTypeActionDefinition(
      string name,
      string workItemType,
      string fromState,
      string toState)
    {
      this.Name = name;
      this.WorkItemType = workItemType;
      this.FromState = fromState;
      this.ToState = toState;
    }

    public string Name { get; set; }

    public string WorkItemType { get; set; }

    public string FromState { get; set; }

    public string ToState { get; set; }
  }
}
