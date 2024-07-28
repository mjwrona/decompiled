// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.ResourceLinks.ResourceLinkData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.ResourceLinks
{
  internal class ResourceLinkData
  {
    public int AreaId { get; set; }

    public DateTime RemovedDate { get; set; }

    public DateTime AddedDate { get; set; }

    public int FldId { get; set; }

    public int Id { get; set; }

    public string FilePath { get; set; }

    public int FilePathHash { get; set; }

    public string OriginalName { get; set; }

    public int ExtId { get; set; }

    public string Comment { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastWriteDate { get; set; }

    public int Length { get; set; }
  }
}
