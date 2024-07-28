// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemRelationTypesController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemRelationTypes", ResourceVersion = 2)]
  public class WorkItemRelationTypesController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5906000;
    private static readonly IDictionary<string, string> s_resourceLinkMap = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemLinkTypeReferenceName)
    {
      {
        "AttachedFile",
        "Attached File"
      },
      {
        "Hyperlink",
        "Hyperlink"
      },
      {
        "ArtifactLink",
        "Artifact Link"
      }
    };

    public override string TraceArea => "workItemRelationTypes";

    [TraceFilter(5906000, 5906010)]
    [HttpGet]
    [ClientExample("GET__relation_types.json", "Get the work item relation types", null, null)]
    public IEnumerable<WorkItemRelationType> GetRelationTypes()
    {
      WorkItemTrackingLinkService linkService = this.TfsRequestContext.WitContext().LinkService;
      IEnumerable<WorkItemRelationType> source = linkService.GetLinkTypeIds(this.TfsRequestContext, true).Select<int, WorkItemLinkTypeEnd>((Func<int, WorkItemLinkTypeEnd>) (linkId => linkService.GetLinkTypeEndById(this.TfsRequestContext, linkId))).Select<WorkItemLinkTypeEnd, WorkItemRelationType>((Func<WorkItemLinkTypeEnd, WorkItemRelationType>) (linkEnd => WorkItemRelationTypeFactory.Create(this.WitRequestContext, linkEnd))).Concat<WorkItemRelationType>(WorkItemRelationTypesController.s_resourceLinkMap.Select<KeyValuePair<string, string>, WorkItemRelationType>((Func<KeyValuePair<string, string>, WorkItemRelationType>) (kvp => WorkItemRelationTypeFactory.Create(this.WitRequestContext, kvp.Key, kvp.Value))));
      return source == null ? (IEnumerable<WorkItemRelationType>) null : (IEnumerable<WorkItemRelationType>) source.ToList<WorkItemRelationType>();
    }

    [TraceFilter(5906010, 5906020)]
    [HttpGet]
    [ClientExample("GET__relation_type.json", "Get the work item relation type", null, null)]
    public WorkItemRelationType GetRelationType(string relation)
    {
      WorkItemLinkTypeEnd linkType;
      if (this.WitRequestContext.LinkService.TryGetLinkTypeEndByReferenceName(this.TfsRequestContext, relation, out linkType))
        return WorkItemRelationTypeFactory.Create(this.WitRequestContext, linkType);
      string resourceLinkName;
      if (WorkItemRelationTypesController.s_resourceLinkMap.TryGetValue(relation, out resourceLinkName))
        return WorkItemRelationTypeFactory.Create(this.WitRequestContext, relation, resourceLinkName);
      throw new WitResourceNotFoundException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.InvalidRelationType((object) relation));
    }
  }
}
