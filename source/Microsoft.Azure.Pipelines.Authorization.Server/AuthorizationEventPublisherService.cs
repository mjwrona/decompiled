// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.AuthorizationEventPublisherService
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.Azure.Pipelines.Checks.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Pipelines.Authorization.Server
{
  public sealed class AuthorizationEventPublisherService : 
    IAuthorizationEventPublisherService,
    IVssFrameworkService
  {
    private const string c_layer = "AuthorizationEventPublisher";

    public void OnAuthorizationCompleted(
      IVssRequestContext requestContext,
      Guid projectId,
      ResourcePipelinePermissions pipelinePermissionsForResource)
    {
      using (new MethodScope(requestContext, "AuthorizationEventPublisher", nameof (OnAuthorizationCompleted)))
      {
        AuthorizationCompletedEvent eventData = new AuthorizationCompletedEvent("MS.Azure.Pipelines.AuthorizationCompleted", projectId, pipelinePermissionsForResource);
        this.PublishEvent(requestContext, "MS.Azure.Pipelines.AuthorizationCompleted", (object) eventData);
        requestContext.TraceVerbose(34002008, "PipelinePolicy.Authorization", "AuthorizationEventPublisher", (object) "Raised AuthorizationCompletedEvent with authorization data: {0}", (object) pipelinePermissionsForResource);
      }
    }

    internal void PublishEvent(
      IVssRequestContext requestContext,
      string eventType,
      object eventData)
    {
      try
      {
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, eventData);
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34002007, "PipelinePolicy.Authorization", "AuthorizationEventPublisher", (object) "Failed to fire AuthorizationCompletedEvent with data {0}. Exception received: {1}", eventData, (object) ex);
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
