// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiV2Controller
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "wikis", ResourceVersion = 2)]
  public class WikiV2Controller : WikiApiController
  {
    [HttpGet]
    [ClientLocationId("288D122C-DBD4-451D-AA5F-7DBBBA070728")]
    [ClientResponseType(typeof (IEnumerable<WikiV2>), null, null)]
    [ClientResourceOperation(ClientResourceOperationName.List)]
    [TraceFilter(15250300, 15250399)]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET_wikis_queryInCollection.json", "Query in collection", null, null)]
    [ClientExample("GET_wikis_queryInProject.json", "Query in project", null, null)]
    public HttpResponseMessage GetAllWikis()
    {
      IEnumerable<WikiV2> wikis = WikiV2Helper.GetWikis(this.TfsRequestContext, this.ProjectId);
      foreach (WikiV2 wikiV2 in wikis)
        wikiV2.SetSecuredObject(GitSecuredObjectFactory.CreateRepositoryReadOnly(wikiV2.ProjectId, wikiV2.RepositoryId));
      ISecuredObject teamProjectReadOnly = this.ProjectInfo != null ? SharedSecuredObjectFactory.CreateTeamProjectReadOnly(this.ProjectId) : (ISecuredObject) null;
      return this.GenerateResponse<WikiV2>(wikis, teamProjectReadOnly);
    }

    [HttpGet]
    [ClientLocationId("288D122C-DBD4-451D-AA5F-7DBBBA070728")]
    [ClientResponseType(typeof (WikiV2), null, null)]
    [ClientResourceOperation(ClientResourceOperationName.Get)]
    [TraceFilter(15250300, 15250399)]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET_wiki_queryByName.json", "Query by name", null, null)]
    [ClientExample("GET_wiki_queryById.json", "Query by Id", null, null)]
    public HttpResponseMessage GetWiki([FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier)
    {
      if (string.IsNullOrEmpty(wikiIdentifier))
        throw new InvalidArgumentValueException(nameof (wikiIdentifier));
      WikiV2 wikiByIdentifier = WikiV2Helper.GetWikiByIdentifier(this.TfsRequestContext, this.ProjectId, wikiIdentifier);
      if (wikiByIdentifier == null)
        throw new WikiNotFoundException(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiNotFound);
      ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(wikiByIdentifier.ProjectId, wikiByIdentifier.RepositoryId);
      wikiByIdentifier.SetSecuredObject(repositoryReadOnly);
      return this.Request.CreateResponse<WikiV2>(HttpStatusCode.OK, wikiByIdentifier);
    }

    [HttpPost]
    [ClientResourceOperation(ClientResourceOperationName.Create)]
    [ClientResponseCode(HttpStatusCode.Created, "Wiki created", false)]
    [ClientLocationId("288D122C-DBD4-451D-AA5F-7DBBBA070728")]
    [ClientResponseType(typeof (WikiV2), null, null)]
    [TraceFilter(15250000, 15250099)]
    [ClientExample("POST_wiki_type_project.json", "Create project wiki", null, null)]
    [ClientExample("POST_wiki_type_code.json", "Create code wiki", null, null)]
    public HttpResponseMessage CreateWiki(WikiCreateParametersV2 wikiCreateParams)
    {
      Guid guid = wikiCreateParams != null ? wikiCreateParams.ProjectId : throw new InvalidArgumentValueException(nameof (wikiCreateParams), Microsoft.TeamFoundation.Wiki.Web.Resources.MalformedWikiData);
      if (!(wikiCreateParams.ProjectId != Guid.Empty))
        throw new InvalidArgumentValueException("ProjectId", Microsoft.TeamFoundation.Wiki.Web.Resources.TeamProjectRequired);
      if (this.ProjectId != Guid.Empty && wikiCreateParams.ProjectId != this.ProjectId)
        throw new InvalidArgumentValueException(Microsoft.TeamFoundation.Wiki.Web.Resources.MismatchedProjectId);
      Guid projectId = this.ProjectId != Guid.Empty ? this.ProjectId : wikiCreateParams.ProjectId;
      this.TfsRequestContext.GetService<IProjectService>().GetProject(this.TfsRequestContext, projectId);
      return this.Request.CreateResponse<WikiV2>(HttpStatusCode.Created, WikiV2Helper.CreateWiki(this.TfsRequestContext, wikiCreateParams, this.m_WikiJobHandler));
    }

    [HttpDelete]
    [ClientResourceOperation(ClientResourceOperationName.Delete)]
    [ClientResponseCode(HttpStatusCode.OK, "Wiki deleted", false)]
    [ClientLocationId("288D122C-DBD4-451D-AA5F-7DBBBA070728")]
    [ClientResponseType(typeof (WikiV2), null, null)]
    [TraceFilter(15250300, 15250399)]
    [ClientExample("DELETE_wiki_type_code.json", "Delete code wiki", null, null)]
    public HttpResponseMessage DeleteWiki([FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier)
    {
      if (string.IsNullOrEmpty(wikiIdentifier))
        throw new InvalidArgumentValueException(nameof (wikiIdentifier));
      return this.Request.CreateResponse<WikiV2>(HttpStatusCode.OK, WikiV2Helper.DeleteWiki(this.TfsRequestContext, this.ProjectId, wikiIdentifier, this.m_WikiJobHandler) ?? throw new WikiNotFoundException(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiNotFound));
    }

    [HttpPatch]
    [ClientLocationId("288D122C-DBD4-451D-AA5F-7DBBBA070728")]
    [ClientResponseType(typeof (WikiV2), null, null)]
    [ClientResourceOperation(ClientResourceOperationName.Update)]
    [TraceFilter(15250300, 15250399)]
    [ClientExample("PATCH_wiki_versions.json", "Update wiki versions", null, null)]
    public HttpResponseMessage UpdateWiki(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromBody] WikiUpdateParameters updateParameters)
    {
      if (string.IsNullOrEmpty(wikiIdentifier))
        throw new InvalidArgumentValueException(nameof (wikiIdentifier));
      if (updateParameters == null)
        throw new InvalidArgumentValueException(nameof (updateParameters));
      return this.Request.CreateResponse<WikiV2>(HttpStatusCode.OK, WikiV2Helper.UpdateWiki(this.TfsRequestContext, this.ProjectId, wikiIdentifier, updateParameters.Versions, this.m_WikiJobHandler, updateParameters.Name) ?? throw new WikiNotFoundException(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiNotFound));
    }
  }
}
