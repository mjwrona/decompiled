// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyEvaluationsController
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Policy.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PolicyEvaluationsController : PolicyApiController
  {
    [HttpGet]
    [ClientLocationId("46AECB7A-5D2C-4647-897B-0209505A9FE4")]
    [ClientResponseType(typeof (PolicyEvaluationRecord), null, null)]
    public HttpResponseMessage GetPolicyEvaluation(Guid evaluationId)
    {
      ArgumentUtility.CheckForEmptyGuid(evaluationId, nameof (evaluationId));
      return this.Request.CreateResponse<PolicyEvaluationRecord>(HttpStatusCode.OK, this.PolicyService.GetPolicyEvaluationRecord(this.TfsRequestContext, this.ProjectId, evaluationId));
    }

    [HttpPatch]
    [ClientLocationId("46AECB7A-5D2C-4647-897B-0209505A9FE4")]
    [ValidateModel]
    public PolicyEvaluationRecord RequeuePolicyEvaluation(Guid evaluationId)
    {
      ArgumentUtility.CheckForEmptyGuid(evaluationId, nameof (evaluationId));
      this.PolicyService.RequeuePolicyEvaluationRecord(this.TfsRequestContext, this.ProjectId, evaluationId);
      return this.PolicyService.GetPolicyEvaluationRecord(this.TfsRequestContext, this.ProjectId, evaluationId);
    }

    [HttpGet]
    [ClientLocationId("C23DDFF5-229C-4D04-A80B-0FDCE9F360C8")]
    [ClientResponseType(typeof (List<PolicyEvaluationRecord>), null, null)]
    public HttpResponseMessage GetPolicyEvaluations(
      string artifactId,
      bool includeNotApplicable = false,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(artifactId, nameof (artifactId));
      return this.GenerateResponse<PolicyEvaluationRecord>(this.PolicyService.GetPolicyEvaluationRecords(this.TfsRequestContext, this.ProjectId, LinkingUtilities.DecodeUri(artifactId), includeNotApplicable, top, skip), PolicySecuredObjectFactory.CreateReadOnlyInstance(this.ProjectId));
    }
  }
}
