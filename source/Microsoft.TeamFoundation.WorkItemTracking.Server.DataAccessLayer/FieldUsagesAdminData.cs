// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldUsagesAdminData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class FieldUsagesAdminData
  {
    public int FldUsageId { get; set; }

    public bool FDeleted { get; set; }

    public int ObjectId { get; set; }

    public int FldId { get; set; }

    public int DirectObjectId { get; set; }

    public bool FOftenQueried { get; set; }

    public bool FCore { get; set; }

    public bool FOftenQueriedAsText { get; set; }

    public long? CacheStamp { get; set; }

    public bool FSupportsTextQuery { get; set; }
  }
}
