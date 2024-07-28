// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.HostMigrationManagementService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class HostMigrationManagementService : IHostMigrationManagementService, IVssFrameworkService
  {
    private IVssServiceHost m_serviceHost;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_serviceHost = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? systemRequestContext.ServiceHost : throw new InvalidRequestContextHostException(FrameworkResources.DeploymentHostRequired());

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool IsParallelMigrationEnabled(IVssRequestContext requestContext)
    {
      using (HostMigrationQueueComponent component = requestContext.CreateComponent<HostMigrationQueueComponent>())
        return component is HostMigrationQueueComponent3;
    }

    public bool IsBackgroundMigrationEnabled(IVssRequestContext requestContext)
    {
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        return component is HostMigrationComponent7;
    }

    public List<HostMigrationRequest> GetRunningRequests(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      using (HostMigrationQueueComponent component = requestContext.CreateComponent<HostMigrationQueueComponent>())
        return component.GetRunningRequests();
    }

    public List<HostMigrationRequest> GetNextSchedulableRequests(
      IVssRequestContext requestContext,
      DateTime maxLastUserAccess,
      int maxNumberOfRequests,
      string[] targetInstanceNamesToIgnore)
    {
      this.ValidateRequestContext(requestContext);
      using (HostMigrationQueueComponent component = requestContext.CreateComponent<HostMigrationQueueComponent>())
        return component.GetSchedulableQueueRequest(maxLastUserAccess, maxNumberOfRequests, targetInstanceNamesToIgnore);
    }

    public HostMigrationRequestResultSegment GetNextSchedulableRequestsSegmented(
      IVssRequestContext requestContext,
      DateTime maxLastUserAccess,
      int maxNumberOfRequests,
      HostMigrationRequestContinuationToken continuationToken,
      string[] targetInstanceNamesToIgnore)
    {
      this.ValidateRequestContext(requestContext);
      List<HostMigrationRequest> requestsSegmented;
      using (HostMigrationQueueComponent component = requestContext.CreateComponent<HostMigrationQueueComponent>())
        requestsSegmented = component.GetSchedulableQueueRequestsSegmented(maxLastUserAccess, maxNumberOfRequests, continuationToken == null ? int.MaxValue : continuationToken.LastPriority, continuationToken == null ? Guid.Empty : continuationToken.LastHostId, targetInstanceNamesToIgnore);
      HostMigrationRequestContinuationToken continuationToken1 = requestsSegmented.Count < maxNumberOfRequests ? (HostMigrationRequestContinuationToken) null : new HostMigrationRequestContinuationToken(requestsSegmented.Last<HostMigrationRequest>());
      return new HostMigrationRequestResultSegment(requestsSegmented, continuationToken1);
    }

    public void SetQueueRequestDriverJobId(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId)
    {
      this.ValidateRequestContext(requestContext);
      using (HostMigrationQueueComponent component = requestContext.CreateComponent<HostMigrationQueueComponent>())
      {
        if (!component.SetQueueRequestDriverJobId(hostId, new Guid?(jobId)))
          throw new InvalidOperationException(string.Format("Queue request for host {0} was not found", (object) hostId));
      }
    }

    public void ClearQueueRequestDriverJobId(IVssRequestContext requestContext, Guid hostId)
    {
      this.ValidateRequestContext(requestContext);
      using (HostMigrationQueueComponent component = requestContext.CreateComponent<HostMigrationQueueComponent>())
      {
        if (!component.SetQueueRequestDriverJobId(hostId, new Guid?()))
          throw new InvalidOperationException(string.Format("Queue request for host {0} was not found", (object) hostId));
      }
    }

    public void DeleteQueueRequest(IVssRequestContext requestContext, Guid hostId)
    {
      this.ValidateRequestContext(requestContext);
      using (HostMigrationQueueComponent component = requestContext.CreateComponent<HostMigrationQueueComponent>())
        component.DeleteQueueRequest(hostId);
    }

    public HostMigrationRequest GetMigrationRequestForHost(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      this.ValidateRequestContext(requestContext);
      using (HostMigrationQueueComponent component = requestContext.CreateComponent<HostMigrationQueueComponent>())
        return component.GetQueueRequest(hostId);
    }

    public void QueueMigrationRequest(
      IVssRequestContext requestContext,
      HostMigrationRequest request)
    {
      this.ValidateRequestContext(requestContext);
      using (HostMigrationQueueComponent component = requestContext.CreateComponent<HostMigrationQueueComponent>())
        component.AddQueueRequest(request);
    }

    public void QueueMigrationRequests(
      IVssRequestContext requestContext,
      IEnumerable<HostMigrationRequest> requests)
    {
      this.ValidateRequestContext(requestContext);
      using (HostMigrationQueueComponent component = requestContext.CreateComponent<HostMigrationQueueComponent>())
        component.AddQueueRequests(requests);
    }

    public bool IsMigrationEnabledOnInstance(IVssRequestContext requestContext, string instanceName)
    {
      this.ValidateRequestContext(requestContext);
      Guid instanceId;
      HostMigrationUtil.GetServiceInstance(requestContext, instanceName, out instanceId, out Guid _);
      using (MigrationHttpClient httpClient = HostMigrationUtil.CreateHttpClient<MigrationHttpClient>(requestContext, instanceId))
        return httpClient.GetDeploymentInformationAsync().SyncResult<DeploymentInformation>().MigrationEnabled;
    }

    public string GetMigrationCertificateThumbprintForInstance(
      IVssRequestContext requestContext,
      string instanceName)
    {
      this.ValidateRequestContext(requestContext);
      Guid instanceId;
      HostMigrationUtil.GetServiceInstance(requestContext, instanceName, out instanceId, out Guid _);
      using (MigrationHttpClient httpClient = HostMigrationUtil.CreateHttpClient<MigrationHttpClient>(requestContext, instanceId))
        return httpClient.GetDeploymentInformationAsync().SyncResult<DeploymentInformation>().MigrationCertificateThumbprint;
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHost.InstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.ServicingServiceRequestContextHostMessage((object) this.m_serviceHost.InstanceId, (object) requestContext.ServiceHost.InstanceId));
    }
  }
}
