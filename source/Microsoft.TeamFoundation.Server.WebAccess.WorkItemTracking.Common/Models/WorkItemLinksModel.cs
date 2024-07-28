// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemLinksModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class WorkItemLinksModel : BaseSecuredObjectModel
  {
    public WorkItemLinksModel(
      IVssRequestContext requestContext,
      WorkItem workItem,
      ISecuredObject securedObject)
      : base(securedObject)
    {
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      this.WitLinkTypes = service.GetLinkTypes(requestContext).Select<WorkItemLinkType, WorkItemLinksTypeModel>((Func<WorkItemLinkType, WorkItemLinksTypeModel>) (link => new WorkItemLinksTypeModel(securedObject)
      {
        ReferenceName = link.ReferenceName,
        Topology = link.LinkTopology.ToString(),
        CanDelete = link.CanDelete,
        CanEdit = link.CanEdit,
        IsActive = link.IsActive,
        IsDirectional = link.IsDirectional,
        IsNonCircular = link.IsNonCircular,
        IsOneToMany = link.IsOneToMany,
        ForwardEnd = new WorkItemLinkTypeEndModel(link.ForwardEnd, securedObject),
        ReverseEnd = new WorkItemLinkTypeEndModel(link.ReverseEnd, securedObject),
        IsRemote = link.IsRemote
      })).ToArray<WorkItemLinksTypeModel>();
      this.RegisteredLinkTypes = service.GetRegisteredLinkTypes(requestContext).Select<RegisteredLinkType, RegisteredLinkTypeModel>((Func<RegisteredLinkType, RegisteredLinkTypeModel>) (link => new RegisteredLinkTypeModel(securedObject)
      {
        Name = link.Name,
        ToolId = link.ToolId
      })).ToArray<RegisteredLinkTypeModel>();
    }

    [DataMember]
    public WorkItemLinksTypeModel[] WitLinkTypes { get; private set; }

    [DataMember]
    public RegisteredLinkTypeModel[] RegisteredLinkTypes { get; private set; }
  }
}
