// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.ShardDetails
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class ShardDetails
  {
    public int Id { get; set; }

    public string EsClusterName { get; set; }

    public IEntityType EntityType { get; set; }

    public string IndexName { get; set; }

    public short ShardId { get; set; }

    public int ActualDocCount { get; set; }

    public int EstimatedDocCount { get; set; }

    public int EstimatedDocCountGrowth { get; set; }

    public int ReservedDocCount { get; set; }

    public int DeletedDocCount { get; set; }

    public long ActualSize { get; set; }

    public long EstimatedSize { get; set; }

    public long EstimatedSizeGrowth { get; set; }

    public long ReservedSpace { get; set; }

    public DateTime CreatedTimeStamp { get; set; }

    public DateTime LastModifiedTimeStamp { get; set; }
  }
}
