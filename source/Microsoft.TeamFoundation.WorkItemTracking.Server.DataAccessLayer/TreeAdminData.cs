// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TreeAdminData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class TreeAdminData
  {
    public int AreaId { get; set; }

    public bool FDeleted { get; set; }

    public int TypeId { get; set; }

    public string Name { get; set; }

    public int ParentId { get; set; }

    public bool FAdminOnly { get; set; }

    public int StructureType { get; set; }

    public string Guid { get; set; }

    public long? CacheStamp { get; set; }
  }
}
