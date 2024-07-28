// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.PropertiesController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "properties")]
  public class PropertiesController : CodeReviewApiControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (PropertiesCollection), null, null)]
    [ClientLocationId("7CF0E9A4-CCD5-4D63-9C52-5241A213C3FE")]
    public HttpResponseMessage GetReviewProperties(int reviewId) => this.Request.CreateResponse<PropertiesCollection>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ICodeReviewService>().GetProperties(this.TfsRequestContext, this.GetProjectId(), reviewId));

    [HttpPatch]
    [ClientResponseType(typeof (PropertiesCollection), null, null)]
    [ClientLocationId("7CF0E9A4-CCD5-4D63-9C52-5241A213C3FE")]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    public HttpResponseMessage CreateOrUpdateReviewProperties(
      int reviewId,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IDictionary<string, object>> document)
    {
      ICodeReviewService service = this.TfsRequestContext.GetService<ICodeReviewService>();
      PropertiesCollection properties = PropertiesCollectionPatchHelper.ReadPatchDocument((IPatchDocument<IDictionary<string, object>>) document);
      return this.Request.CreateResponse<PropertiesCollection>(HttpStatusCode.OK, service.PatchProperties(this.TfsRequestContext, this.GetProjectId(), reviewId, properties));
    }

    [HttpGet]
    [ClientResponseType(typeof (PropertiesCollection), null, null)]
    [ClientLocationId("1031EA92-06F3-4550-A310-8BB3059B92FF")]
    public HttpResponseMessage GetIterationProperties(int reviewId, int iterationId) => this.Request.CreateResponse<PropertiesCollection>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ICodeReviewIterationService>().GetProperties(this.TfsRequestContext, this.GetProjectId(), reviewId, iterationId));

    [HttpPatch]
    [ClientResponseType(typeof (PropertiesCollection), null, null)]
    [ClientLocationId("1031EA92-06F3-4550-A310-8BB3059B92FF")]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    public HttpResponseMessage CreateOrUpdateIterationProperties(
      int reviewId,
      int iterationId,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IDictionary<string, object>> document)
    {
      ICodeReviewIterationService service = this.TfsRequestContext.GetService<ICodeReviewIterationService>();
      PropertiesCollection properties = PropertiesCollectionPatchHelper.ReadPatchDocument((IPatchDocument<IDictionary<string, object>>) document);
      return this.Request.CreateResponse<PropertiesCollection>(HttpStatusCode.OK, service.PatchProperties(this.TfsRequestContext, this.GetProjectId(), reviewId, iterationId, properties));
    }
  }
}
