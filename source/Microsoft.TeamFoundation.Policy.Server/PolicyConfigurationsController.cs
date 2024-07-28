// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyConfigurationsController
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Policy.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PolicyConfigurationsController : PolicyApiController
  {
    private const string ContinuationTokenHeaderName = "x-ms-continuationtoken";

    [HttpGet]
    [ClientLocationId("DAD91CBE-D183-45F8-9C6E-9C1164472121")]
    [ClientExample("GET__policy_configurations.json", null, null, null)]
    [ClientResponseType(typeof (IPagedList<PolicyConfiguration>), null, null)]
    public HttpResponseMessage GetPolicyConfigurations(
      [FromUri(Name = "scope")] string scope = null,
      [ClientInclude(RestClientLanguages.Swagger2 | RestClientLanguages.Python), FromUri(Name = "$top")] int? top = null,
      [ClientInclude(RestClientLanguages.Swagger2 | RestClientLanguages.Python)] string continuationToken = null,
      Guid? policyType = null)
    {
      PolicyConfigurationsContinuationToken token = (PolicyConfigurationsContinuationToken) null;
      PolicyConfigurationsContinuationToken continuationToken1 = (PolicyConfigurationsContinuationToken) null;
      IEnumerable<PolicyConfigurationRecord> source;
      if (string.IsNullOrEmpty(scope))
      {
        if (continuationToken != null && !PolicyConfigurationsContinuationToken.TryParseContinuationToken(continuationToken, out token))
          throw new ArgumentException(PolicyResources.Format("InvalidContinuationToken", (object) continuationToken));
        int top1;
        int firstConfigurationId;
        if ((top.HasValue ? 1 : (token != null ? 1 : 0)) != 0)
        {
          top1 = Math.Min(50000, Math.Max(0, top ?? 100));
          firstConfigurationId = token != null ? token.NextConfigurationId : 1;
        }
        else
        {
          top1 = int.MaxValue;
          firstConfigurationId = 1;
        }
        int? nextConfigurationId;
        source = this.PolicyService.GetLatestPolicyConfigurationRecords(this.TfsRequestContext, this.ProjectId, top1, firstConfigurationId, out nextConfigurationId, policyType);
        if (nextConfigurationId.HasValue)
          continuationToken1 = new PolicyConfigurationsContinuationToken(nextConfigurationId.Value);
      }
      else
      {
        if (continuationToken != null || top.HasValue)
          throw new ArgumentException(PolicyResources.Format("PaginationIsNotSupported", (object) continuationToken));
        source = this.PolicyService.GetLatestPolicyConfigurationRecordsByScope(this.TfsRequestContext, this.ProjectId, (IEnumerable<string>) new string[1]
        {
          scope
        }, int.MaxValue, 1, out int? _, policyType);
      }
      ISecuredObject securedObject = PolicySecuredObjectFactory.CreateReadOnlyInstance(ProjectInfo.GetProjectUri(this.ProjectId));
      HttpResponseMessage response = this.Request.CreateResponse<IEnumerable<PolicyConfiguration>>(HttpStatusCode.OK, source.Select<PolicyConfigurationRecord, PolicyConfiguration>((Func<PolicyConfigurationRecord, PolicyConfiguration>) (c => c.ToWebApi(this.TfsRequestContext, securedObject: securedObject))));
      if (continuationToken1 != null)
        response.Headers.Add("x-ms-continuationtoken", continuationToken1.ToString());
      return response;
    }

    [HttpGet]
    [ClientLocationId("DAD91CBE-D183-45F8-9C6E-9C1164472121")]
    [ClientExample("GET__policy_configurations__configurationId_.json", null, null, null)]
    public PolicyConfiguration GetPolicyConfiguration(int configurationId) => this.PolicyService.GetPolicyConfigurationRecord(this.TfsRequestContext, this.ProjectId, configurationId).ToWebApi(this.TfsRequestContext);

    [HttpPost]
    [ClientLocationId("DAD91CBE-D183-45F8-9C6E-9C1164472121")]
    [ClientResponseType(typeof (PolicyConfiguration), null, null)]
    [ClientExample("POST__policy_configurations.json", "Example policy", null, null)]
    [ClientExample("POST__policy_configurations2.json", "Approval count policy", null, null)]
    [ClientExample("POST__policy_configurations3.json", "Build policy", null, null)]
    [ClientExample("POST__policy_configurations4.json", "Work item policy", null, null)]
    [ClientExample("POST__policy_configurations5.json", "Merge strategy policy", null, null)]
    [ClientExample("POST__policy_configurations6.json", "Git case enforcement policy", null, null)]
    [ClientExample("POST__policy_configurations7.json", "Git maximum blob size policy", null, null)]
    [ValidateModel]
    public HttpResponseMessage CreatePolicyConfiguration(PolicyConfiguration configuration)
    {
      this.ValidateNewConfiguration(configuration);
      string settings = configuration.Settings == null ? string.Empty : configuration.Settings.ToString();
      if (configuration.IsDeleted)
        throw new ArgumentException(PolicyResources.Get("PolicyIsDeleteCannotBeUsedForDeletion"));
      return this.Request.CreateResponse<PolicyConfiguration>(HttpStatusCode.OK, this.PolicyService.CreatePolicyConfiguration(this.TfsRequestContext, configuration.Type.Id, this.ProjectId, configuration.IsEnabled, configuration.IsBlocking, configuration.IsEnterpriseManaged, settings).ToWebApi(this.TfsRequestContext));
    }

    [HttpPut]
    [ClientLocationId("DAD91CBE-D183-45F8-9C6E-9C1164472121")]
    [ClientExample("PUT__policy_configurations__configurationId_.json", null, null, null)]
    [ValidateModel]
    public PolicyConfiguration UpdatePolicyConfiguration(
      PolicyConfiguration configuration,
      int configurationId)
    {
      this.ValidateNewConfiguration(configuration);
      if (configuration.Id != 0 && configuration.Id != configurationId)
        throw new ArgumentException(PolicyResources.Get("ConfigurationIdMismatch"));
      string settings = configuration.Settings == null ? string.Empty : configuration.Settings.ToString();
      if (configuration.IsDeleted)
        throw new ArgumentException(PolicyResources.Get("PolicyIsDeleteCannotBeUsedForDeletion"));
      return this.PolicyService.UpdatePolicyConfiguration(this.TfsRequestContext, configurationId, configuration.Type.Id, this.ProjectId, configuration.IsEnabled, configuration.IsBlocking, configuration.IsEnterpriseManaged, settings).ToWebApi(this.TfsRequestContext);
    }

    [HttpDelete]
    [ClientLocationId("DAD91CBE-D183-45F8-9C6E-9C1164472121")]
    [ClientResponseCode(HttpStatusCode.NoContent, null, false)]
    [ClientExample("DELETE__policy_configurations__configurationId_.json", null, null, null)]
    public void DeletePolicyConfiguration(int configurationId) => this.PolicyService.DeletePolicyConfiguration(this.TfsRequestContext, this.ProjectId, configurationId);

    private void ValidateNewConfiguration(PolicyConfiguration configuration)
    {
      ArgumentUtility.CheckForNull<PolicyConfiguration>(configuration, nameof (configuration));
      ArgumentUtility.CheckForNull<PolicyTypeRef>(configuration.Type, "configuration.type");
    }
  }
}
