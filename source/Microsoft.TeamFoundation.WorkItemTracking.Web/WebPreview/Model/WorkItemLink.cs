// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLink
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model
{
  [DataContract]
  public class WorkItemLink : Link
  {
    [DataMember]
    public WorkItemReference Target { get; set; }

    [DataMember]
    public string LinkType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? Locked { get; set; }

    public static int GetWorkItemLinkTypeId(
      string referenceName,
      IVssRequestContext tfsRequestContext)
    {
      WorkItemTrackingLinkService service = tfsRequestContext.GetService<WorkItemTrackingLinkService>();
      int workItemLinkTypeId = 0;
      IVssRequestContext requestContext = tfsRequestContext;
      string referenceName1 = referenceName.Split('-')[0];
      MDWorkItemLinkType workItemLinkType;
      ref MDWorkItemLinkType local = ref workItemLinkType;
      if (service.TryGetLinkTypeByReferenceName(requestContext, referenceName1, out local))
        workItemLinkTypeId = !workItemLinkType.IsDirectional ? workItemLinkType.ForwardId : (referenceName.EndsWith("-Forward") ? workItemLinkType.ForwardId : workItemLinkType.ReverseId);
      return workItemLinkTypeId;
    }

    public static string GetWorkItemLinkType(int linkId, IVssRequestContext tfsRequestContext)
    {
      MDWorkItemLinkType linkType;
      return tfsRequestContext.GetService<WorkItemTrackingLinkService>().TryGetLinkTypeById(tfsRequestContext, linkId, out linkType) ? WorkItemLink.GetLinkTypeRefName(linkId, linkType) : (string) null;
    }

    private static string GetLinkTypeRefName(int linkId, MDWorkItemLinkType linkType)
    {
      string linkTypeRefName = linkType.ReferenceName;
      if (linkType.IsDirectional)
        linkTypeRefName = string.Format("{0}-{1}", (object) linkTypeRefName, linkType.ForwardId == linkId ? (object) "Forward" : (object) "Reverse");
      return linkTypeRefName;
    }
  }
}
