// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildDefinitionRevisions2Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "revisions", ResourceVersion = 2)]
  [ClientGroupByResource("definitions")]
  public class BuildDefinitionRevisions2Controller : BuildApiController
  {
    [HttpGet]
    public List<BuildDefinitionRevision> GetDefinitionRevisions(int definitionId)
    {
      int index = 0;
      IBuildRouteService routeService = this.TfsRequestContext.GetService<IBuildRouteService>();
      return this.DefinitionService.GetDefinitionHistory(this.TfsRequestContext, this.ProjectId, definitionId).OrderBy<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, DateTime>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, DateTime>) (historyEntry => historyEntry.CreatedDate)).Select<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, BuildDefinitionRevision>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, BuildDefinitionRevision>) (historyEntry =>
      {
        Microsoft.TeamFoundation.Build.WebApi.BuildDefinition apiBuildDefinition = historyEntry.ToWebApiBuildDefinition(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
        return new BuildDefinitionRevision()
        {
          Revision = apiBuildDefinition.Revision.Value,
          Name = apiBuildDefinition.Name,
          ChangedBy = apiBuildDefinition.AuthoredBy,
          ChangedDate = apiBuildDefinition.CreatedDate,
          ChangeType = index++ == 0 ? Microsoft.TeamFoundation.Build.WebApi.AuditAction.Add : Microsoft.TeamFoundation.Build.WebApi.AuditAction.Update,
          Comment = apiBuildDefinition.Comment,
          DefinitionUrl = routeService.GetDefinitionRestUrl(this.TfsRequestContext, this.ProjectId, definitionId, new int?(apiBuildDefinition.Revision.Value))
        };
      })).ToList<BuildDefinitionRevision>();
    }
  }
}
