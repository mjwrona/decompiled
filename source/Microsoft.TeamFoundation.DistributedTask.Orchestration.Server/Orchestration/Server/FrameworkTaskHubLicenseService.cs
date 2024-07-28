// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.FrameworkTaskHubLicenseService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class FrameworkTaskHubLicenseService : ITaskHubLicenseService, IVssFrameworkService
  {
    private const string c_layer = "FrameworkTaskHubLicenseService";

    public TaskHubLicenseDetails GetTaskHubLicenseDetails(
      IVssRequestContext requestContext,
      string hubName,
      bool includeEnterpriseUsersCount = false,
      bool includeHostedAgentMinutesCount = false)
    {
      using (new MethodScope(requestContext, nameof (FrameworkTaskHubLicenseService), nameof (GetTaskHubLicenseDetails)))
        return requestContext.GetClient<TaskAgentHttpClient>().GetTaskHubLicenseDetailsAsync(hubName, new bool?(includeEnterpriseUsersCount), new bool?(includeHostedAgentMinutesCount)).SyncResult<TaskHubLicenseDetails>();
    }

    public TaskHubLicenseDetails UpdateTaskHubLicenseDetails(
      IVssRequestContext requestContext,
      string hubName,
      TaskHubLicenseDetails taskHubLicenseDetails)
    {
      using (new MethodScope(requestContext, nameof (FrameworkTaskHubLicenseService), nameof (UpdateTaskHubLicenseDetails)))
        return requestContext.GetClient<TaskAgentHttpClient>().UpdateTaskHubLicenseDetailsAsync(hubName, taskHubLicenseDetails).SyncResult<TaskHubLicenseDetails>();
    }

    public IList<ResourceLimit> GetResourceLimits(
      IVssRequestContext requestContext,
      bool allowStaleValues = true)
    {
      using (new MethodScope(requestContext, nameof (FrameworkTaskHubLicenseService), nameof (GetResourceLimits)))
        return (IList<ResourceLimit>) requestContext.GetClient<TaskAgentHttpClient>().GetResourceLimitsAsync().SyncResult<List<ResourceLimit>>();
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void NotifyDataChanged(
      IVssRequestContext requestContext,
      OfferSubscriptionQuantityChangeMessage message)
    {
    }
  }
}
