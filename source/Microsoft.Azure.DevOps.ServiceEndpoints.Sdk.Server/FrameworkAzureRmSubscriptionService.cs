// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.FrameworkAzureRmSubscriptionService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class FrameworkAzureRmSubscriptionService : 
    IAzureRmSubscriptionService2,
    IVssFrameworkService
  {
    private const string c_layer = "FrameworkAzureRmSubscriptionService";

    public AzureSubscriptionQueryResult GetAzureSubscriptions(IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, nameof (FrameworkAzureRmSubscriptionService), nameof (GetAzureSubscriptions)))
        return requestContext.GetClient<ServiceEndpointHttpClient>().GetAzureSubscriptionsAsync((object) requestContext).SyncResult<AzureSubscriptionQueryResult>();
    }

    public AzureManagementGroupQueryResult GetAzureManagementGroups(
      IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, nameof (FrameworkAzureRmSubscriptionService), nameof (GetAzureManagementGroups)))
        return requestContext.GetClient<ServiceEndpointHttpClient>().GetAzureManagementGroupsAsync((object) requestContext).SyncResult<AzureManagementGroupQueryResult>();
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
