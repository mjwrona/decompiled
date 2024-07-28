// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServicingOrchestrationService`2
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ServicingOrchestrationService<TRequest, TJobManager> : IVssFrameworkService
    where TRequest : FrameworkServicingOrchestrationRequest
    where TJobManager : ServicingOrchestrationJobManager<TRequest>, new()
  {
    private readonly ServicingOrchestrationSecurityManager m_securityManager = new ServicingOrchestrationSecurityManager();
    private readonly TJobManager m_jobManager = new TJobManager();

    public void ServiceStart(IVssRequestContext requestContext) => this.CheckRequestContext(requestContext);

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void ValidateRequest(IVssRequestContext requestContext, TRequest request)
    {
      this.CheckRequest(requestContext, request, ServicingOrchestrationPermission.Queue);
      try
      {
        requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) request);
      }
      catch (ActionDeniedBySubscriberException ex)
      {
        throw new ServicingOrchestrationInvalidRequestException(ex.Message, (Exception) ex);
      }
    }

    public void QueueRequest(IVssRequestContext requestContext, TRequest request)
    {
      this.CheckRequest(requestContext, request, ServicingOrchestrationPermission.Queue);
      this.m_jobManager.QueueJob(requestContext, request);
    }

    public void CancelRequest(IVssRequestContext requestContext, Guid requestId)
    {
      this.CheckRequest(requestContext, requestId, ServicingOrchestrationPermission.Cancel);
      this.m_jobManager.StopJob(requestContext, requestId);
    }

    public ServicingOrchestrationRequestStatus GetRequestStatus(
      IVssRequestContext requestContext,
      Guid requestId)
    {
      this.CheckRequest(requestContext, requestId, ServicingOrchestrationPermission.Read);
      return this.m_jobManager.GetJobStatus(requestContext, requestId);
    }

    private void CheckRequest(
      IVssRequestContext requestContext,
      TRequest request,
      ServicingOrchestrationPermission permission)
    {
      this.CheckRequestContext(requestContext);
      this.m_securityManager.CheckPermission(requestContext, permission);
      ArgumentUtility.CheckForNull<TRequest>(request, nameof (request));
      ArgumentUtility.CheckForEmptyGuid(request.HostId, "HostId");
      ArgumentUtility.CheckForEmptyGuid(request.RequestId, "RequestId");
      ArgumentUtility.CheckForEmptyGuid(request.ServicingJobId, "ServicingJobId");
      foreach (PropertyPair property in (List<PropertyPair>) request.Properties)
      {
        if (property == null || property.Name == null || property == null || property.Value == null)
          throw new ArgumentNullException("request property");
        this.m_jobManager.ValidateString(property.Name);
        this.m_jobManager.ValidateString(property.Value);
      }
    }

    private void CheckRequest(
      IVssRequestContext requestContext,
      Guid requestId,
      ServicingOrchestrationPermission permission)
    {
      this.CheckRequestContext(requestContext);
      this.m_securityManager.CheckPermission(requestContext, permission);
      ArgumentUtility.CheckForEmptyGuid(requestId, nameof (requestId));
    }

    private void CheckRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
    }
  }
}
