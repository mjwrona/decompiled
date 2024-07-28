// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTypeRecord
  {
    internal WorkItemTypeRecord()
    {
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int ProjectId { get; set; }
  }
}
