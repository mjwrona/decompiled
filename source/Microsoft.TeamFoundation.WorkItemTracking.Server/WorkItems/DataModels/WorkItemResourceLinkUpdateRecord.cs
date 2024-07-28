// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemResourceLinkUpdateRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  internal class WorkItemResourceLinkUpdateRecord
  {
    public int Order { get; set; }

    public int SourceId { get; set; }

    public LinkUpdateType UpdateType { get; set; }

    public int? ResourceId { get; set; }

    public ResourceLinkType? Type { get; set; }

    public string Location { get; set; }

    public string Name { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public int? Length { get; set; }

    public string Comment { get; set; }

    public Guid ProjectId { get; set; }
  }
}
