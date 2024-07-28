// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyModelExtensions
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public static class PolicyModelExtensions
  {
    private static void PopulatePolicyConfiguration(
      ref PolicyConfiguration partialConfiguration,
      Guid creatorId,
      Guid projectId,
      IVssRequestContext requestContext,
      ISecuredObject securedObject = null)
    {
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      IdentityRef identityRef = (IdentityRef) null;
      if (creatorId != Guid.Empty)
      {
        TeamFoundationIdentity identity = ((IEnumerable<TeamFoundationIdentity>) service.ReadIdentities(requestContext, new Guid[1]
        {
          creatorId
        })).SingleOrDefault<TeamFoundationIdentity>();
        if (identity != null)
          identityRef = identity.ToIdentityRef(requestContext);
      }
      ReferenceLinks referenceLinks = new ReferenceLinks();
      string configurationUrl = PolicyModelExtensions.GetPolicyConfigurationUrl(requestContext, projectId, partialConfiguration.Id);
      string policyTypeUrl = PolicyModelExtensions.GetPolicyTypeUrl(requestContext, projectId, partialConfiguration.Type.Id);
      referenceLinks.AddLink("self", configurationUrl, securedObject);
      referenceLinks.AddLink("policyType", policyTypeUrl, securedObject);
      partialConfiguration.CreatedBy = identityRef;
      partialConfiguration.Type.DisplayName = requestContext.GetService<ITeamFoundationPolicyService>().GetPolicyType(requestContext, partialConfiguration.Type.Id).DisplayName;
      partialConfiguration.Type.Url = policyTypeUrl;
      partialConfiguration.Url = configurationUrl;
      partialConfiguration.Links = referenceLinks;
    }

    public static PolicyConfiguration ToWebApi(
      this PolicyConfigurationRecord configuration,
      IVssRequestContext requestContext,
      bool minimalData = false,
      ISecuredObject securedObject = null)
    {
      PolicyConfiguration policyConfiguration = new PolicyConfiguration(securedObject);
      policyConfiguration.CreatedDate = configuration.CreationDate;
      policyConfiguration.Id = configuration.ConfigurationId;
      policyConfiguration.Revision = configuration.ConfigurationRevisionId;
      policyConfiguration.IsEnabled = configuration.IsEnabled;
      policyConfiguration.IsBlocking = configuration.IsBlocking;
      policyConfiguration.IsEnterpriseManaged = configuration.IsEnterpriseManaged;
      policyConfiguration.IsDeleted = configuration.IsDeleted;
      policyConfiguration.Settings = JObject.Parse(configuration.Settings);
      policyConfiguration.Type = new PolicyTypeRef(securedObject)
      {
        Id = configuration.TypeId
      };
      PolicyConfiguration partialConfiguration = policyConfiguration;
      if (!minimalData)
        PolicyModelExtensions.PopulatePolicyConfiguration(ref partialConfiguration, configuration.CreatorId, configuration.ProjectId, requestContext, securedObject);
      return partialConfiguration;
    }

    public static PolicyConfiguration PopulatePartialWebApi(
      this PolicyConfiguration policyConfiguration,
      Guid projectId,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      PolicyModelExtensions.PopulatePolicyConfiguration(ref policyConfiguration, policyConfiguration.CreatedBy != null ? Guid.Parse(policyConfiguration.CreatedBy.Id) : Guid.Empty, projectId, requestContext, securedObject);
      return policyConfiguration;
    }

    public static PolicyType ToWebApi(
      this ITeamFoundationPolicy type,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ReferenceLinks referenceLinks = new ReferenceLinks();
      string policyTypeUrl = PolicyModelExtensions.GetPolicyTypeUrl(requestContext, projectId, type.Id);
      referenceLinks.AddLink("self", policyTypeUrl);
      PolicyType webApi = new PolicyType();
      webApi.Description = type.Description;
      webApi.Id = type.Id;
      webApi.DisplayName = type.DisplayName;
      webApi.Url = policyTypeUrl;
      webApi.Links = referenceLinks;
      return webApi;
    }

    private static string GetPolicyTypeUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid typeId)
    {
      if (requestContext == null)
        return (string) null;
      ILocationService service = requestContext.GetService<ILocationService>();
      IVssRequestContext requestContext1 = requestContext;
      Guid guid = projectId;
      Guid policyTypesLocationId = PolicyWebApiConstants.PolicyTypesLocationId;
      Guid projectId1 = guid;
      var routeValues = new{ typeId = typeId };
      Guid serviceOwner = new Guid();
      return service.GetResourceUri(requestContext1, "policy", policyTypesLocationId, projectId1, (object) routeValues, serviceOwner).AbsoluteUri;
    }

    private static string GetPolicyConfigurationUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int configurationId,
      int? versionId = null)
    {
      if (requestContext == null)
        return (string) null;
      Guid empty = Guid.Empty;
      Guid identifier;
      object routeValues;
      if (!versionId.HasValue)
      {
        identifier = PolicyWebApiConstants.PolicyConfigurationsLocationId;
        routeValues = (object) new
        {
          configurationId = configurationId
        };
      }
      else
      {
        identifier = PolicyWebApiConstants.PolicyConfigurationRevisionsLocationId;
        routeValues = (object) new
        {
          configurationId = configurationId,
          versionId = versionId.Value
        };
      }
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "policy", identifier, projectId, routeValues).AbsoluteUri;
    }
  }
}
