// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcChangesetChangesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("changesets")]
  public class TfvcChangesetChangesController : TfvcApiController
  {
    [ClientExample("GET__tfvc_changesetChanges.json", "GET the Tfvc changes for a given changeset", null, null)]
    [HttpGet]
    [ClientResponseType(typeof (IPagedList<TfvcChange>), null, null)]
    public HttpResponseMessage GetChangesetChanges(
      [ClientParameterType(typeof (int), false)] string id = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null,
      [ClientInclude(RestClientLanguages.Swagger2 | RestClientLanguages.Python)] string continuationToken = null)
    {
      int changesetNumber = VersionSpecCommon.ParseChangesetNumber((VersionSpecFactory) VersionSpec.ServerVersionSpecFactory, id);
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
      TfvcChangesContinuationToken token = (TfvcChangesContinuationToken) null;
      if (continuationToken != null && !TfvcChangesContinuationToken.TryParseContinuationToken(changesetNumber, continuationToken, out token))
        return this.Request.CreateErrorResponse((Exception) new InvalidArgumentValueException(Resources.Format("InvalidContinuationToken", (object) continuationToken)), (IHttpController) this);
      int top1 = Math.Max(0, top ?? 100);
      int skip1 = token == null ? Math.Max(0, skip.GetValueOrDefault()) : 0;
      TfvcChangesContinuationToken nextToken;
      HttpResponseMessage response = this.Request.CreateResponse<List<TfvcChange>>(HttpStatusCode.OK, TfvcChangesetUtility.RetrieveChangesetChangesPaged(this.TfsRequestContext, this.Url, changesetNumber, top1, skip1, token, out nextToken));
      if (nextToken != null)
      {
        string str = this.Url.RestLink(this.TfsRequestContext, TfvcConstants.TfvcChangesetChangesLocationId, (object) new Dictionary<string, object>()
        {
          {
            nameof (id),
            (object) id
          },
          {
            "$top",
            (object) top1
          },
          {
            nameof (continuationToken),
            (object) nextToken.ToString()
          }
        });
        response.Headers.Add("Link", string.Format(Resources.Get("LinkHeaderForNextPage"), (object) str));
        response.Headers.Add("x-ms-continuationtoken", nextToken.ToString());
      }
      return response;
    }
  }
}
