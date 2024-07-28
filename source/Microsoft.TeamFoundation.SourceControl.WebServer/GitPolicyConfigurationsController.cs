// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPolicyConfigurationsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(5.0)]
  public class GitPolicyConfigurationsController : GitApiController
  {
    [HttpGet]
    [ValidateModel]
    [ClientLocationId("2C420070-A0A2-49CC-9639-C9F271C5FF07")]
    [ClientResponseType(typeof (GitPolicyConfigurationResponse), null, null)]
    public HttpResponseMessage GetPolicyConfigurations(
      [FromUri, ClientParameterType(typeof (Guid?), false)] string repositoryId = null,
      [FromUri] string refName = null,
      [FromUri, ClientParameterType(typeof (Guid?), false)] string policyType = null,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri] string continuationToken = null)
    {
      ITeamFoundationGitPolicyService service = this.TfsRequestContext.GetService<ITeamFoundationGitPolicyService>();
      Guid? repoId = new Guid?();
      PolicyConfigurationsContinuationToken token = (PolicyConfigurationsContinuationToken) null;
      PolicyConfigurationsContinuationToken continuationToken1 = (PolicyConfigurationsContinuationToken) null;
      if (repositoryId != null)
      {
        Guid result;
        if (Guid.TryParse(repositoryId, out result))
          repoId = new Guid?(result);
        else
          throw new GitArgumentException(Resources.Format("InvalidRepositoryId", (object) repositoryId));
      }
      Guid? policyType1 = new Guid?();
      if (policyType != null)
      {
        Guid result;
        if (Guid.TryParse(policyType, out result))
          policyType1 = new Guid?(result);
        else
          throw new GitArgumentException(Resources.Format("InvalidPolicyType", (object) policyType));
      }
      if (continuationToken != null && !PolicyConfigurationsContinuationToken.TryParseContinuationToken(continuationToken, out token))
        throw new GitArgumentException(Resources.Format("InvalidContinuationToken", (object) continuationToken));
      int top1 = Math.Min(50000, Math.Max(0, top ?? 100));
      int firstConfigurationId = token != null ? token.NextConfigurationId : 1;
      int? nextConfigurationId;
      IEnumerable<PolicyConfigurationRecord> policyConfigurations = service.GetLatestGitPolicyConfigurations(this.TfsRequestContext, this.ProjectId, repoId, refName, policyType1, top1, firstConfigurationId, out nextConfigurationId);
      if (nextConfigurationId.HasValue)
        continuationToken1 = new PolicyConfigurationsContinuationToken(nextConfigurationId.Value);
      HttpResponseMessage response = this.Request.CreateResponse<IEnumerable<PolicyConfiguration>>(HttpStatusCode.OK, policyConfigurations.Select<PolicyConfigurationRecord, PolicyConfiguration>((Func<PolicyConfigurationRecord, PolicyConfiguration>) (c => c.ToWebApi(this.TfsRequestContext))));
      if (continuationToken1 != null)
        response.Headers.Add("x-ms-continuationtoken", continuationToken1.ToString());
      return response;
    }
  }
}
