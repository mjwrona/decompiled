// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.TreeNodeRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  internal class TreeNodeRecord
  {
    public Guid ProjectId { get; set; }

    public int Id { get; set; }

    public int ParentId { get; set; }

    public TreeStructureType StructureType { get; set; }

    public int TreeTypeId { get; set; }

    public Guid CssNodeId { get; set; }

    public string Name { get; set; }

    public bool IsDeleted { get; set; }

    public int ReclassifyId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? FinishDate { get; set; }

    public DateTime ChangeDate { get; set; }

    public string ChangedBy { get; set; }
  }
}
