// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ReportingWorkItemLink
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  [DataContract]
  public class ReportingWorkItemLink
  {
    [DataMember]
    public string Rel { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string LinkType { get; set; }

    [DataMember]
    public int SourceId { get; set; }

    [DataMember]
    public int TargetId { get; set; }

    [DataMember]
    public DateTime ChangedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ChangedBy { get; set; }

    [DataMember]
    public bool IsActive { get; set; }

    [DataMember]
    public LinkChangeType ChangedOperation { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid SourceProjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid TargetProjectId { get; set; }

    public static ReportingWorkItemLink Create(
      IVssRequestContext requestContext,
      WorkItemLinkChange linkChange,
      IDictionary<Guid, IdentityReference> identityMap = null,
      bool includeRemoteUrl = false)
    {
      return new ReportingWorkItemLink()
      {
        Rel = linkChange.LinkType,
        LinkType = linkChange.LinkTypeString,
        SourceId = linkChange.SourceID,
        TargetId = linkChange.TargetID,
        IsActive = linkChange.IsActive,
        ChangedDate = linkChange.ChangedDate,
        ChangedBy = linkChange.ChangedBy_Name != null ? ReportingWorkItemLink.GetIdentityRef(requestContext, linkChange.ChangedBy_TfId, linkChange.ChangedBy_Name, identityMap) : (IdentityRef) null,
        Comment = string.IsNullOrEmpty(linkChange.Comment) ? (string) null : linkChange.Comment,
        ChangedOperation = linkChange.IsActive ? LinkChangeType.Create : LinkChangeType.Remove,
        SourceProjectId = linkChange.SourceProjectId,
        TargetProjectId = linkChange.TargetProjectId
      };
    }

    private static IdentityRef GetIdentityRef(
      IVssRequestContext requestContext,
      Guid vsid,
      string witName,
      IDictionary<Guid, IdentityReference> identityMap)
    {
      IdentityReference identityRef = (IdentityReference) null;
      if (IdentityReferenceBuilder.ShouldUseProperIdentityRef(requestContext) && identityMap != null && identityMap.TryGetValue(vsid, out identityRef))
        return (IdentityRef) identityRef;
      return new IdentityRef()
      {
        Id = vsid.ToString(),
        UniqueName = witName
      };
    }
  }
}
