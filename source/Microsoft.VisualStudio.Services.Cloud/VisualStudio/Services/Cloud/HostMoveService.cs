// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMoveService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public sealed class HostMoveService : IVssFrameworkService
  {
    private IVssServiceHost m_serviceHost;
    private static readonly string s_area = "HostMove";
    private static readonly string s_layer = "BusinessLogic";

    private HostMoveService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.m_serviceHost = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? systemRequestContext.ServiceHost : throw new InvalidRequestContextHostException(FrameworkResources.DeploymentHostRequired());

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddHostMoveRequest(
      IVssRequestContext requestContext,
      HostMoveRequest hostMoveRequest)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckForNull<HostMoveRequest>(hostMoveRequest, nameof (hostMoveRequest));
      try
      {
        requestContext.TraceEnter(105000, HostMoveService.s_area, HostMoveService.s_layer, nameof (AddHostMoveRequest));
        requestContext.Trace(105001, TraceLevel.Info, HostMoveService.s_area, HostMoveService.s_layer, "Adding host move request. HostId: {0}, TargetDatabaseId: {1}, Options: {2}", (object) hostMoveRequest.HostId, (object) hostMoveRequest.TargetDatabaseId, (object) hostMoveRequest.Options);
        using (HostMoveComponent component = requestContext.CreateComponent<HostMoveComponent>())
          component.AddHostMoveRequest(hostMoveRequest);
        requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, string.Format(FrameworkServerConstants.DisableDatabaseDownsizeDuringMigrationsUntil, (object) hostMoveRequest.TargetDatabaseId), DateTime.UtcNow.AddHours(24.0).ToString("yyyy-MM-ddTHH:mm:ss"));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(105001, HostMoveService.s_area, HostMoveService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(105000, HostMoveService.s_area, HostMoveService.s_layer, nameof (AddHostMoveRequest));
      }
    }

    public bool DeleteHostMoveRequest(IVssRequestContext requestContext, Guid hostId)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      requestContext.TraceEnter(105100, HostMoveService.s_area, HostMoveService.s_layer, nameof (DeleteHostMoveRequest));
      try
      {
        bool flag;
        using (HostMoveComponent component = requestContext.CreateComponent<HostMoveComponent>())
          flag = component.DeleteHostMoveRequest(hostId);
        requestContext.Trace(105102, TraceLevel.Info, HostMoveService.s_area, HostMoveService.s_layer, "DeleteHostMoveRequest. HostId: {0}, Deleted: {1}", (object) hostId, (object) flag);
        return flag;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(105103, HostMoveService.s_area, HostMoveService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(105100, HostMoveService.s_area, HostMoveService.s_layer, nameof (DeleteHostMoveRequest));
      }
    }

    public HostMoveRequest GetHostMoveRequest(IVssRequestContext requestContext, Guid hostId)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      requestContext.TraceEnter(105200, HostMoveService.s_area, HostMoveService.s_layer, nameof (GetHostMoveRequest));
      try
      {
        HostMoveRequest hostMoveRequest;
        using (HostMoveComponent component = requestContext.CreateComponent<HostMoveComponent>())
          hostMoveRequest = component.GetHostMoveRequest(hostId);
        requestContext.Trace(105201, TraceLevel.Verbose, HostMoveService.s_area, HostMoveService.s_layer, "GetHostMoveRequest executed. HostId: {0}, Request: {1}", (object) hostId, (object) hostMoveRequest);
        return hostMoveRequest;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(105202, HostMoveService.s_area, HostMoveService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(105200, HostMoveService.s_area, HostMoveService.s_layer, nameof (GetHostMoveRequest));
      }
    }

    public int GetMaxParallelism(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(105300, HostMoveService.s_area, HostMoveService.s_layer, nameof (GetMaxParallelism));
      try
      {
        int maxParallelism = requestContext.GetService<CachedRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.HostMoveMaxParallelism, 2);
        requestContext.Trace(105301, TraceLevel.Verbose, HostMoveService.s_area, HostMoveService.s_layer, "Host move max parallelism: {0}", (object) maxParallelism);
        return maxParallelism;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(105302, HostMoveService.s_area, HostMoveService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(105300, HostMoveService.s_area, HostMoveService.s_layer, nameof (GetMaxParallelism));
      }
    }

    public List<HostMoveRequest> QuerySchedulableHostMoveRequests(
      IVssRequestContext requestContext,
      int count,
      IEnumerable<int> excludeDatabaseIds)
    {
      this.ValidateRequestContext(requestContext);
      int num = requestContext.GetService<CachedRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.HostMoveMinDormancyInMinutes, 420);
      return this.QuerySchedulableHostMoveRequests(requestContext, count, excludeDatabaseIds, TimeSpan.FromMinutes((double) num));
    }

    public List<HostMoveRequest> QuerySchedulableHostMoveRequests(
      IVssRequestContext requestContext,
      int count,
      IEnumerable<int> excludeDatabaseIds,
      TimeSpan minDormancy)
    {
      this.ValidateRequestContext(requestContext);
      try
      {
        requestContext.TraceEnter(105400, HostMoveService.s_area, HostMoveService.s_layer, nameof (QuerySchedulableHostMoveRequests));
        ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0);
        requestContext.Trace(105401, TraceLevel.Info, HostMoveService.s_area, HostMoveService.s_layer, "Count: {0}, Min Dormancy: {1}", (object) count, (object) minDormancy);
        List<HostMoveRequest> hostMoveRequestList;
        using (HostMoveComponent component = requestContext.CreateComponent<HostMoveComponent>())
          hostMoveRequestList = component.QuerySchedulableHostMoveRequests(count, DateTime.UtcNow - minDormancy, excludeDatabaseIds);
        requestContext.Trace(105402, TraceLevel.Info, HostMoveService.s_area, HostMoveService.s_layer, "Found {0} move requests eligible for scheduling.", (object) hostMoveRequestList.Count);
        return hostMoveRequestList;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(105410, HostMoveService.s_area, HostMoveService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(105400, HostMoveService.s_area, HostMoveService.s_layer, nameof (QuerySchedulableHostMoveRequests));
      }
    }

    public bool SetHostMoveRequestJobId(IVssRequestContext requestContext, Guid hostId, Guid jobId)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(105400, HostMoveService.s_area, HostMoveService.s_layer, nameof (SetHostMoveRequestJobId));
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      ArgumentUtility.CheckForEmptyGuid(jobId, nameof (jobId));
      bool flag;
      using (HostMoveComponent component = requestContext.CreateComponent<HostMoveComponent>())
        flag = component.SetHostMoveRequestJobId(hostId, jobId);
      requestContext.Trace(105401, TraceLevel.Verbose, HostMoveService.s_area, HostMoveService.s_layer, "SetHostMoveRequestJobId. HostId: {0}, JobId: {1}, Updated: {2}", (object) hostId, (object) jobId, (object) flag);
      requestContext.TraceLeave(105400, HostMoveService.s_area, HostMoveService.s_layer, nameof (SetHostMoveRequestJobId));
      return flag;
    }

    public List<HostMoveRequest> QueryQueuedHostMoveRequests(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(105500, HostMoveService.s_area, HostMoveService.s_layer, nameof (QueryQueuedHostMoveRequests));
      using (HostMoveComponent component = requestContext.CreateComponent<HostMoveComponent>())
        return component.QueryQueuedHostMoveRequests();
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHost.InstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.ServicingServiceRequestContextHostMessage((object) this.m_serviceHost.InstanceId, (object) requestContext.ServiceHost.InstanceId));
    }
  }
}
