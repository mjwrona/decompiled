// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiController
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "wikis", ResourceVersion = 1)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WikiController : WikiApiController
  {
    [HttpGet]
    [ClientLocationId("288D122C-DBD4-451D-AA5F-7DBBBA070728")]
    [ClientResponseType(typeof (IEnumerable<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>), null, null)]
    [TraceFilter(15250300, 15250399)]
    public HttpResponseMessage GetWikis() => this.GenerateResponse<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>(WikiV1Helper.GetWikis(this.TfsRequestContext, this.ProjectId));

    [HttpPost]
    [ClientResourceOperation(ClientResourceOperationName.Create)]
    [ClientResponseCode(HttpStatusCode.Created, "Wiki created", false)]
    [ClientLocationId("288D122C-DBD4-451D-AA5F-7DBBBA070728")]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Wiki.WebApi.Wiki), null, null)]
    [TraceFilter(15250000, 15250099)]
    public HttpResponseMessage CreateWiki(WikiCreateParameters wikiCreateParams)
    {
      Guid guid = wikiCreateParams != null ? wikiCreateParams.ProjectId : throw new InvalidArgumentValueException(nameof (wikiCreateParams), Microsoft.TeamFoundation.Wiki.Web.Resources.MalformedWikiData);
      bool flag = wikiCreateParams.ProjectId != Guid.Empty;
      if (!flag && this.ProjectId == Guid.Empty)
        throw new InvalidArgumentValueException("ProjectId", Microsoft.TeamFoundation.Wiki.Web.Resources.TeamProjectRequired);
      if (this.ProjectId != Guid.Empty & flag && wikiCreateParams.ProjectId != this.ProjectId)
        throw new InvalidArgumentValueException(Microsoft.TeamFoundation.Wiki.Web.Resources.MismatchedProjectId);
      return this.Request.CreateResponse<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>(HttpStatusCode.Created, WikiV1Helper.CreateWiki(this.TfsRequestContext, this.ProjectId != Guid.Empty ? this.ProjectId : wikiCreateParams.ProjectId, wikiCreateParams.Name, this.m_WikiJobHandler));
    }
  }
}
