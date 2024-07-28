// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class LinkInfo
  {
    public DateTime AddedDate;
    public DateTime RemovedDate;
    private DateTime? m_authorizedAddedDate;
    private DateTime? m_authorizedRemovedDate;

    public string Comment { get; set; }

    public int FieldId { get; set; }

    protected LinkInfo()
    {
      this.AddedDate = SharedVariables.FutureDateTimeValue;
      this.RemovedDate = SharedVariables.FutureDateTimeValue;
      this.Comment = string.Empty;
      this.FieldId = 0;
    }

    protected LinkInfo(LinkInfo link)
    {
      this.AddedDate = link.AddedDate;
      this.RemovedDate = link.RemovedDate;
      this.Comment = link.Comment;
      this.FieldId = link.FieldId;
    }

    public DateTime AuthorizedAddedDate
    {
      get => !this.m_authorizedAddedDate.HasValue ? this.AddedDate : this.m_authorizedAddedDate.Value;
      set => this.m_authorizedAddedDate = new DateTime?(value);
    }

    public DateTime AuthorizedRemovedDate
    {
      get => !this.m_authorizedRemovedDate.HasValue ? this.RemovedDate : this.m_authorizedRemovedDate.Value;
      set => this.m_authorizedRemovedDate = new DateTime?(value);
    }
  }
}
