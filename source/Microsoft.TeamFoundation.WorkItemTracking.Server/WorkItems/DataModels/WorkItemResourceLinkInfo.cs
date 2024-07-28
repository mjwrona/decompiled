// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemResourceLinkInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  public class WorkItemResourceLinkInfo : 
    IRevisedWorkItemEntity,
    IEquatable<WorkItemResourceLinkInfo>,
    IComparable<WorkItemResourceLinkInfo>
  {
    public int SourceId { get; set; }

    public int ResourceId { get; set; }

    public ResourceLinkType ResourceType { get; set; }

    public DateTime AuthorizedDate { get; set; }

    public DateTime RevisedDate { get; set; }

    public string Location { get; set; }

    public string Name { get; set; }

    public DateTime ResourceCreatedDate { get; set; }

    public DateTime ResourceModifiedDate { get; set; }

    public int ResourceSize { get; set; }

    public string Comment { get; set; }

    int IRevisedWorkItemEntity.Id => this.SourceId;

    DateTime IRevisedWorkItemEntity.AuthorizedDate
    {
      get => this.AuthorizedDate;
      set => this.AuthorizedDate = value;
    }

    DateTime IRevisedWorkItemEntity.RevisedDate => this.RevisedDate;

    bool IRevisedWorkItemEntity.SpansMultipleRevisions => true;

    public override int GetHashCode() => CommonUtils.CombineHashCodes(CommonUtils.CombineHashCodes(CommonUtils.CombineHashCodes(CommonUtils.CombineHashCodes(CommonUtils.CombineHashCodes(CommonUtils.CombineHashCodes(this.SourceId, this.ResourceId), this.AuthorizedDate.GetHashCode()), this.RevisedDate.GetHashCode()), this.ResourceCreatedDate.GetHashCode()), this.ResourceModifiedDate.GetHashCode()), this.ResourceSize);

    public override bool Equals(object obj) => this.Equals(obj as WorkItemResourceLinkInfo);

    public bool Equals(WorkItemResourceLinkInfo other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.SourceId == other.SourceId && this.ResourceId == other.ResourceId && this.ResourceType == other.ResourceType && this.AuthorizedDate == other.AuthorizedDate && this.RevisedDate == other.RevisedDate && this.ResourceSize == other.ResourceSize && this.ResourceCreatedDate == other.ResourceCreatedDate && this.ResourceModifiedDate == other.ResourceModifiedDate && StringComparer.Ordinal.Equals(this.Location, other.Location) && StringComparer.Ordinal.Equals(this.Name, other.Name) && StringComparer.Ordinal.Equals(this.Comment, other.Comment);
    }

    public int CompareTo(WorkItemResourceLinkInfo other) => other == null ? 1 : this.GetHashCode().CompareTo(other.GetHashCode());
  }
}
