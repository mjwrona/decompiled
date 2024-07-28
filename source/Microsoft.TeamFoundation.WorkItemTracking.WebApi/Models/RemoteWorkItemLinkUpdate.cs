// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.RemoteWorkItemLinkUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class RemoteWorkItemLinkUpdate
  {
    [DataMember]
    public int LocalWorkItemId { get; set; }

    [DataMember]
    public string LocalWorkItemTitle { get; set; }

    [DataMember]
    public string LocalWorkItemType { get; set; }

    [DataMember]
    public Guid LocalHostId { get; set; }

    [DataMember]
    public Guid LocalProjectId { get; set; }

    [DataMember]
    public long LocalWatermark { get; set; }

    [DataMember]
    public int RemoteWorkItemId { get; set; }

    [DataMember]
    public string LinkTypeEnd { get; set; }

    [DataMember]
    public string Comment { get; set; }

    [DataMember]
    public RemoteLinkChangeType ChangeType { get; set; }

    [DataMember]
    public IdentityDescriptor AuthorizedByDescriptor { get; set; }
  }
}
