// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  public class WorkItemLinkInfo : 
    IRevisedWorkItemEntity,
    IEquatable<WorkItemLinkInfo>,
    IComparable<WorkItemLinkInfo>
  {
    public int SourceId { get; set; }

    public int TargetId { get; set; }

    public int LinkType { get; set; }

    public DateTime AuthorizedDate { get; set; }

    public DateTime RevisedDate { get; set; }

    public int AuthorizedById { get; set; }

    public Guid AuthorizedByTfid { get; set; }

    public string AuthorizedBy { get; set; }

    public int RevisedById { get; set; }

    public Guid RevisedByTfid { get; set; }

    public string RevisedBy { get; set; }

    public string Comment { get; set; }

    public bool IsLocked { get; set; }

    public Guid TargetProjectId { get; set; }

    public Guid? RemoteHostId { get; set; }

    public Guid? RemoteProjectId { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus? RemoteStatus { get; set; }

    public string RemoteStatusMessage { get; set; }

    public long? RemoteWatermark { get; set; }

    public string RemoteHostUrl { get; set; }

    public string RemoteHostName { get; set; }

    public long TimeStamp { get; set; }

    int IRevisedWorkItemEntity.Id => this.SourceId;

    DateTime IRevisedWorkItemEntity.AuthorizedDate
    {
      get => this.AuthorizedDate;
      set => this.AuthorizedDate = value;
    }

    DateTime IRevisedWorkItemEntity.RevisedDate => this.RevisedDate;

    bool IRevisedWorkItemEntity.SpansMultipleRevisions => true;

    public override int GetHashCode()
    {
      int hashA = CommonUtils.CombineHashCodes(CommonUtils.CombineHashCodes(CommonUtils.CombineHashCodes(CommonUtils.CombineHashCodes(CommonUtils.CombineHashCodes(CommonUtils.CombineHashCodes(this.SourceId, this.TargetId), this.LinkType), this.AuthorizedDate.GetHashCode()), this.RevisedDate.GetHashCode()), this.AuthorizedById), this.RevisedById);
      if (this.RemoteHostId.HasValue)
        hashA = CommonUtils.CombineHashCodes(hashA, this.RemoteHostId.GetHashCode());
      return hashA;
    }

    public override bool Equals(object obj) => this.Equals(obj as WorkItemLinkInfo);

    public bool Equals(WorkItemLinkInfo other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      if (this.SourceId == other.SourceId && this.TargetId == other.TargetId && this.LinkType == other.LinkType && this.AuthorizedDate == other.AuthorizedDate && this.RevisedDate == other.RevisedDate && this.AuthorizedById == other.AuthorizedById && this.RevisedById == other.RevisedById && this.IsLocked == other.IsLocked && this.TargetProjectId == other.TargetProjectId && StringComparer.Ordinal.Equals(this.Comment, other.Comment))
      {
        Guid? nullable1 = this.RemoteHostId;
        Guid? nullable2 = other.RemoteHostId;
        if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
        {
          nullable2 = this.RemoteProjectId;
          nullable1 = other.RemoteProjectId;
          if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
          {
            Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus? remoteStatus1 = this.RemoteStatus;
            Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus? remoteStatus2 = other.RemoteStatus;
            if (remoteStatus1.GetValueOrDefault() == remoteStatus2.GetValueOrDefault() & remoteStatus1.HasValue == remoteStatus2.HasValue && StringComparer.Ordinal.Equals(this.RemoteStatusMessage, other.RemoteStatusMessage))
            {
              long? remoteWatermark1 = this.RemoteWatermark;
              long? remoteWatermark2 = other.RemoteWatermark;
              return remoteWatermark1.GetValueOrDefault() == remoteWatermark2.GetValueOrDefault() & remoteWatermark1.HasValue == remoteWatermark2.HasValue;
            }
          }
        }
      }
      return false;
    }

    public int CompareTo(WorkItemLinkInfo other) => other == null ? 1 : this.GetHashCode().CompareTo(other.GetHashCode());
  }
}
