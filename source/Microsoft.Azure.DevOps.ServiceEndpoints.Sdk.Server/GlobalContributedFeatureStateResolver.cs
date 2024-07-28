// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.GlobalContributedFeatureStateResolver
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal static class GlobalContributedFeatureStateResolver
  {
    public static async Task<bool> IsFeatureEnabled(
      IVssRequestContext requestContext,
      string contributedFeatureName,
      Guid hostingService)
    {
      ContributedFeatureState featureState = await GlobalContributedFeatureStateResolver.GetFeatureState(requestContext, contributedFeatureName, hostingService);
      return featureState != null && featureState.State == ContributedFeatureEnabledValue.Enabled;
    }

    public static async Task<ContributedFeatureState> GetFeatureState(
      IVssRequestContext requestContext,
      string contributedFeatureName,
      Guid hostingService)
    {
      ContributedFeatureState featureState = requestContext.GetService<IContributedFeatureService>().GetFeatureState(requestContext, contributedFeatureName);
      if (featureState == null)
        featureState = (await GlobalContributedFeatureStateResolver.GetServiceHttpClient<FeatureManagementHttpClient>(requestContext, GlobalContributedFeatureStateResolver.GetCollectionUrl(requestContext, requestContext.ServiceHost.InstanceId, hostingService)).QueryFeatureStatesAsync(new ContributedFeatureStateQuery()
        {
          FeatureIds = (IList<string>) new string[1]
          {
            contributedFeatureName
          }
        })).FeatureStates.GetValueOrDefault<string, ContributedFeatureState>(contributedFeatureName);
      return featureState;
    }

    private static Uri GetCollectionUrl(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid serviceTypeIdentifier)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Uri hostUri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, collectionId, serviceTypeIdentifier);
      return !(hostUri == (Uri) null) ? hostUri : throw new ArgumentException(string.Format("Unable to resolve a uri for the service type '{0}' and collection id '{1}'", (object) serviceTypeIdentifier, (object) collectionId));
    }

    private static T GetServiceHttpClient<T>(IVssRequestContext requestContext, Uri endpoint) where T : VssHttpClientBase
    {
      ApiResourceLocationCollection resourceLocations = (ApiResourceLocationCollection) null;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Audit.ApiResourceLocationCollectionsFromLocationService"))
        resourceLocations = requestContext.GetService<ILocationService>().GetResourceLocations(requestContext);
      return (requestContext.ClientProvider as ICreateClient).CreateClient<T>(requestContext, endpoint, nameof (GlobalContributedFeatureStateResolver), resourceLocations);
    }
  }
}
