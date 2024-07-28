// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.StatusesController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "statuses")]
  public class StatusesController : CodeReviewApiControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (List<Status>), null, null)]
    [ClientLocationId("502D7933-25DE-42E3-BC82-8478B3796655")]
    public HttpResponseMessage GetReviewStatuses(int reviewId) => this.GenerateResponse<Status>(this.TfsRequestContext.GetService<ICodeReviewStatusService>().GetStatuses(this.TfsRequestContext, this.GetProjectId(), reviewId));

    [HttpGet]
    [ClientResponseType(typeof (List<Status>), null, null)]
    [ClientLocationId("CB958C49-F702-483A-BB3B-3454570FB72A")]
    public HttpResponseMessage GetIterationStatuses(int reviewId, int iterationId) => this.GenerateResponse<Status>(this.TfsRequestContext.GetService<ICodeReviewStatusService>().GetStatuses(this.TfsRequestContext, this.GetProjectId(), reviewId, new int?(iterationId)));

    [HttpGet]
    [ClientResponseType(typeof (Status), null, null)]
    [ClientLocationId("502D7933-25DE-42E3-BC82-8478B3796655")]
    public HttpResponseMessage GetReviewStatus(int reviewId, int statusId) => this.Request.CreateResponse<Status>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ICodeReviewStatusService>().GetStatus(this.TfsRequestContext, this.GetProjectId(), reviewId, statusId));

    [HttpGet]
    [ClientResponseType(typeof (Status), null, null)]
    [ClientLocationId("CB958C49-F702-483A-BB3B-3454570FB72A")]
    public HttpResponseMessage GetIterationStatus(int reviewId, int iterationId, int statusId) => this.Request.CreateResponse<Status>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ICodeReviewStatusService>().GetStatus(this.TfsRequestContext, this.GetProjectId(), reviewId, statusId, new int?(iterationId)));

    [HttpPost]
    [ClientResponseType(typeof (Status), null, null)]
    [ClientLocationId("502D7933-25DE-42E3-BC82-8478B3796655")]
    public HttpResponseMessage CreateReviewStatus(int reviewId, Status status)
    {
      StatusesController.ValidateStatusPayload(status);
      return this.Request.CreateResponse<Status>(HttpStatusCode.Created, this.TfsRequestContext.GetService<ICodeReviewStatusService>().SaveStatus(this.TfsRequestContext, this.GetProjectId(), reviewId, status));
    }

    [HttpPost]
    [ClientResponseType(typeof (Status), null, null)]
    [ClientLocationId("CB958C49-F702-483A-BB3B-3454570FB72A")]
    public HttpResponseMessage CreateIterationStatus(int reviewId, int iterationId, Status status)
    {
      StatusesController.ValidateStatusPayload(status);
      return this.Request.CreateResponse<Status>(HttpStatusCode.Created, this.TfsRequestContext.GetService<ICodeReviewStatusService>().SaveStatus(this.TfsRequestContext, this.GetProjectId(), reviewId, status, new int?(iterationId)));
    }

    [HttpPatch]
    [ClientResponseType(typeof (Status), null, null)]
    [ClientLocationId("502D7933-25DE-42E3-BC82-8478B3796655")]
    public HttpResponseMessage UpdateReviewStatus(int reviewId, int statusId, Status status)
    {
      StatusesController.ValidateStatusPayload(status);
      ICodeReviewStatusService service = this.TfsRequestContext.GetService<ICodeReviewStatusService>();
      status.Id = statusId;
      return this.Request.CreateResponse<Status>(HttpStatusCode.OK, service.SaveStatus(this.TfsRequestContext, this.GetProjectId(), reviewId, status));
    }

    [HttpPatch]
    [ClientResponseType(typeof (Status), null, null)]
    [ClientLocationId("CB958C49-F702-483A-BB3B-3454570FB72A")]
    public HttpResponseMessage UpdateIterationStatus(
      int reviewId,
      int iterationId,
      int statusId,
      Status status)
    {
      StatusesController.ValidateStatusPayload(status);
      ICodeReviewStatusService service = this.TfsRequestContext.GetService<ICodeReviewStatusService>();
      status.Id = statusId;
      return this.Request.CreateResponse<Status>(HttpStatusCode.OK, service.SaveStatus(this.TfsRequestContext, this.GetProjectId(), reviewId, status, new int?(iterationId)));
    }

    private static void ValidateStatusPayload(Status status)
    {
      if (status == null)
        throw new ArgumentNullException(nameof (status), CodeReviewResources.StatusMalformed());
    }
  }
}
