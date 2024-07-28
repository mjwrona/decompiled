// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceHostHistoryEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServiceHostHistoryEntry : IEquatable<ServiceHostHistoryEntry>
  {
    public int HistoryId { get; set; }

    public DateTime InsertedDate { get; set; }

    public Guid HostId { get; set; }

    public ServiceHostHistoryChangeType ChangeType { get; set; }

    public Guid ParentHostId { get; set; }

    public int DatabaseId { get; set; }

    public int StorageAccountId { get; set; }

    public TeamFoundationServiceHostStatus Status { get; set; }

    public ServiceHostSubStatus SubStatus { get; set; }

    public string StatusReason { get; set; }

    public TeamFoundationHostType HostType { get; set; }

    public DateTime LastUserAccess { get; set; }

    public string Name { get; set; }

    public override string ToString() => string.Format("\n                HistoryId: {0}\n                InsertedDate: {1}\n                HostId: {2}\n                ChangeType: {3}\n                ParentHostId: {4}\n                DatabaseId: {5}\n                StorageAccountId: {6}\n                Status: {7}\n                StatusReason: {8}\n                HostType: {9}\n                LastUserAccess: {10}\n                Name: {11}\n                SubStatus: {12}\n                ", (object) this.HistoryId, (object) this.InsertedDate, (object) this.HostId, (object) this.ChangeType, (object) this.ParentHostId, (object) this.DatabaseId, (object) this.StorageAccountId, (object) this.Status, (object) this.StatusReason, (object) this.HostType, (object) this.LastUserAccess, (object) this.Name, (object) this.SubStatus);

    public override bool Equals(object obj) => obj is ServiceHostHistoryEntry other && this.Equals(other);

    public bool Equals(ServiceHostHistoryEntry other) => this.HostId == other.HostId && this.ChangeType == other.ChangeType && this.DatabaseId == other.DatabaseId && this.StatusReason == other.StatusReason && (this.LastUserAccess == other.LastUserAccess || (this.LastUserAccess - other.LastUserAccess).TotalSeconds < 1.0) && this.ParentHostId == other.ParentHostId && this.Status == other.Status && this.Name == other.Name && this.SubStatus == other.SubStatus;

    public override int GetHashCode() => this.HostId.GetHashCode() ^ this.ChangeType.GetHashCode();
  }
}
