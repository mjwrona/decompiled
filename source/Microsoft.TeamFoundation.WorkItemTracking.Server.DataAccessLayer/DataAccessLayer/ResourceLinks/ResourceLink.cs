// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.ResourceLinks.ResourceLink
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.ResourceLinks
{
  public class ResourceLink
  {
    public DateTime RemovedDate { get; private set; }

    public DateTime AddedDate { get; private set; }

    public int FldId { get; private set; }

    public int WorkItemId { get; private set; }

    public string FilePath { get; private set; }

    public int FilePathHash { get; private set; }

    public string OriginalName { get; private set; }

    public int ExtId { get; private set; }

    public string Comment { get; private set; }

    public DateTime CreationDate { get; private set; }

    public DateTime LastWriteDate { get; private set; }

    public int Length { get; private set; }

    public int AreaId { get; private set; }

    internal static ResourceLink Create(ResourceLinkData data) => new ResourceLink()
    {
      RemovedDate = data.RemovedDate,
      AddedDate = data.AddedDate,
      FldId = data.FldId,
      WorkItemId = data.Id,
      FilePath = data.FilePath,
      FilePathHash = data.FilePathHash,
      OriginalName = data.OriginalName,
      ExtId = data.ExtId,
      Comment = data.Comment,
      CreationDate = data.CreationDate,
      LastWriteDate = data.LastWriteDate,
      Length = data.Length,
      AreaId = data.AreaId
    };
  }
}
