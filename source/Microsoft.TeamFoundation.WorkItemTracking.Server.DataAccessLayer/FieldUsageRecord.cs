// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldUsageRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class FieldUsageRecord
  {
    internal FieldUsageRecord()
    {
    }

    public int FieldId { get; set; }

    public int ObjectId { get; set; }

    public int DirectObjectId { get; set; }

    public bool OftenQueried { get; set; }

    public bool SupportsTextQuery { get; set; }

    public bool Core { get; set; }
  }
}
