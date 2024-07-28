// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.PlatformServiceEndpointTypesService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public class PlatformServiceEndpointTypesService : 
    IServiceEndpointTypesService2,
    IVssFrameworkService
  {
    public const string EndpointAuthSchemes = "ms.vss-endpoint.endpoint-auth-schemes";
    public const string EndpointTypes = "ms.vss-endpoint.endpoint-types";
    private const string c_layer = "PlatformServiceEndpointTypesService";

    public static ServiceEndpointType GetServiceEndpointType(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      bool throwIfTypeNotFound = true)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointTypesService), nameof (GetServiceEndpointType)))
      {
        if (endpoint == null)
          throw new ArgumentNullException(nameof (endpoint));
        if (endpoint.Authorization == null)
          throw new ServiceEndpointException(ServiceEndpointResources.NullAuthorizationNotAllowed());
        if (!endpoint.IsCustomEndpointType())
          return (ServiceEndpointType) null;
        ServiceEndpointType serviceEndpointType = requestContext.GetService<IServiceEndpointTypesService2>().GetServiceEndpointTypes(requestContext, endpoint.Type, endpoint.Authorization.Scheme).FirstOrDefault<ServiceEndpointType>();
        return !(serviceEndpointType == null & throwIfTypeNotFound) ? serviceEndpointType : throw new ServiceEndpointException(ServiceEndpointResources.ServiceEndpointTypeNotFound((object) endpoint.Type, (object) endpoint.Authorization.Scheme));
      }
    }

    public static IDictionary<string, ServiceEndpointType> GetServiceEndpointTypesMap(
      IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointTypesService), nameof (GetServiceEndpointTypesMap)))
      {
        IEnumerable<ServiceEndpointType> serviceEndpointTypes = requestContext.GetService<IServiceEndpointTypesService2>().GetServiceEndpointTypes(requestContext, (string) null, (string) null);
        return serviceEndpointTypes != null ? (IDictionary<string, ServiceEndpointType>) serviceEndpointTypes.GroupBy<ServiceEndpointType, string>((Func<ServiceEndpointType, string>) (p => p.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToDictionary<IGrouping<string, ServiceEndpointType>, string, ServiceEndpointType>((Func<IGrouping<string, ServiceEndpointType>, string>) (type => type.Key), (Func<IGrouping<string, ServiceEndpointType>, ServiceEndpointType>) (type => type.First<ServiceEndpointType>()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IDictionary<string, ServiceEndpointType>) null;
      }
    }

    public IEnumerable<ServiceEndpointType> GetServiceEndpointTypes(
      IVssRequestContext requestContext,
      string type,
      string scheme)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointTypesService), nameof (GetServiceEndpointTypes)))
      {
        IContributionService service = requestContext.GetService<IContributionService>();
        IEnumerable<Contribution> collection1;
        IEnumerable<Contribution> collection2;
        if (requestContext.IsUserContext)
        {
          collection1 = service.QueryContributions(requestContext, (IEnumerable<string>) new List<string>()
          {
            "ms.vss-endpoint.endpoint-types"
          }, queryOptions: ContributionQueryOptions.IncludeChildren);
          collection2 = service.QueryContributions(requestContext, (IEnumerable<string>) new List<string>()
          {
            "ms.vss-endpoint.endpoint-auth-schemes"
          }, queryOptions: ContributionQueryOptions.IncludeChildren);
        }
        else
        {
          collection1 = service.QueryContributionsForTarget(requestContext, "ms.vss-endpoint.endpoint-types");
          collection2 = service.QueryContributionsForTarget(requestContext, "ms.vss-endpoint.endpoint-auth-schemes");
        }
        List<Contribution> contributionList = new List<Contribution>();
        if (!string.IsNullOrWhiteSpace(type))
        {
          foreach (Contribution contribution in collection1)
          {
            try
            {
              string contributionName = this.GetContributionName(contribution);
              if (string.Equals(type, contributionName, StringComparison.OrdinalIgnoreCase))
                contributionList.Add(contribution);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(34000818, "ServiceEndpoints", ex);
            }
          }
        }
        else
          contributionList.AddRange(collection1);
        List<Contribution> authContributions = new List<Contribution>();
        if (!string.IsNullOrWhiteSpace(scheme))
        {
          foreach (Contribution contribution in collection2)
          {
            try
            {
              string contributionName = this.GetContributionName(contribution);
              if (string.Equals(scheme, contributionName, StringComparison.OrdinalIgnoreCase))
                authContributions.Add(contribution);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(34000818, "ServiceEndpoints", ex);
            }
          }
        }
        else
          authContributions.AddRange(collection2);
        HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        HashSet<string> stringSet2 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (Contribution contribution in contributionList)
        {
          if (string.Equals(contribution.Id.Split('.')[0], "ms", StringComparison.OrdinalIgnoreCase))
          {
            stringSet1.Add(contribution.Id);
            stringSet2.Add(this.GetContributionName(contribution));
          }
        }
        IList<ServiceEndpointType> serviceEndpointTypes = (IList<ServiceEndpointType>) new List<ServiceEndpointType>();
        foreach (Contribution contribution in contributionList)
        {
          try
          {
            if (!stringSet1.Contains(contribution.Id))
            {
              if (stringSet2.Contains(this.GetContributionName(contribution)))
                continue;
            }
            if (this.IsFeatureAvailable(requestContext, contribution))
            {
              ServiceEndpointType serviceEndpointType = contribution.ToServiceEndpointType(requestContext, (IEnumerable<Contribution>) authContributions);
              if (!PlatformServiceEndpointTypesService.IsEndpointTypeHidden(serviceEndpointType.Name, type))
              {
                if (serviceEndpointType.AuthenticationSchemes.Count > 0)
                  serviceEndpointTypes.Add(serviceEndpointType);
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceInfo(34000818, "ServiceEndpoints", ServiceEndpointResources.FailedToLoadServiceEndpointTypeFromContribution((object) contribution.Id), (object) ex);
          }
        }
        return (IEnumerable<ServiceEndpointType>) serviceEndpointTypes;
      }
    }

    private string GetContributionName(Contribution contribution) => ServiceEndpointContributionExtensions.GetRequiredValue<string>(contribution.Properties, "name");

    private bool IsFeatureAvailable(IVssRequestContext requestContext, Contribution contribution)
    {
      string optionalValue = ServiceEndpointContributionExtensions.GetOptionalValue<string>(contribution.Properties, "featureFlag");
      return optionalValue == null || requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, optionalValue);
    }

    private static bool IsEndpointTypeHidden(string endpointType, string requestedType) => VssStringComparer.ServiceEndpointTypeCompararer.Equals(endpointType, "GitHubEnterpriseBoards") && !VssStringComparer.ServiceEndpointTypeCompararer.Equals(endpointType, requestedType);

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      requestContext.CheckProjectCollectionRequestContext();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
