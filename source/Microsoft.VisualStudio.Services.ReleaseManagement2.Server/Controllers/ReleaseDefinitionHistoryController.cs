// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ReleaseDefinitionHistoryController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "revisions")]
  [ClientGroupByResource("definitions")]
  public class ReleaseDefinitionHistoryController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [ReleaseManagementSecurityPermission("definitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.ViewReleaseDefinition, true)]
    [ClientExample("GET__GetReleaseDefinitionRevision.json", null, null, null)]
    public IEnumerable<ReleaseDefinitionRevision> GetReleaseDefinitionHistory(int definitionId) => (IEnumerable<ReleaseDefinitionRevision>) this.GetService<ReleaseDefinitionHistoryService>().GetHistory(this.TfsRequestContext, this.ProjectId, definitionId).Select<ReleaseDefinitionRevision, ReleaseDefinitionRevision>((Func<ReleaseDefinitionRevision, ReleaseDefinitionRevision>) (revision =>
    {
      revision.DefinitionUrl = WebAccessUrlBuilder.GetReleaseDefinitionRevisionRestUrl(this.TfsRequestContext, this.ProjectId, definitionId, revision.Revision);
      return revision;
    })).ToList<ReleaseDefinitionRevision>();

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    [ReleaseManagementSecurityPermission("definitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.ViewReleaseDefinition, true)]
    public virtual HttpResponseMessage GetDefinitionRevision(int definitionId, int revision) => RmDefinitionsController.GetReleaseDefinitionRevision(this.TfsRequestContext, this.ProjectId, this.Request, this.GetService<ReleaseDefinitionHistoryService>(), definitionId, revision);
  }
}
