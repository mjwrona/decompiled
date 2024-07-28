// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.RemoteServices.FrameworkProjectServiceBusReceiver
// Assembly: Microsoft.TeamFoundation.Server.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 836F660A-A756-49A7-82F0-68378533B43C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.RemoteServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.RemoteServices
{
  public class FrameworkProjectServiceBusReceiver : IMessageBusSubscriberJobExtensionReceiver
  {
    private static readonly string s_area = "Project";
    private static readonly string s_layer = nameof (FrameworkProjectServiceBusReceiver);

    public void Receive(IVssRequestContext requestContext, IMessage m)
    {
      ServiceEvent body = m.GetBody<ServiceEvent>();
      this.Receive(requestContext, (ProjectMessage) body.Resource);
    }

    internal void Receive(IVssRequestContext requestContext, ProjectMessage projectMessage)
    {
      try
      {
        long knownRevision = FrameworkProjectUpdateJobWorker.GetKnownRevision(requestContext);
        if (projectMessage.Project.Revision > knownRevision)
          this.QueueProjectUpdateJob(requestContext, projectMessage);
        if (!projectMessage.ShouldInvalidateSystemStore)
          return;
        requestContext.GetService<LocalSecurityInvalidationService>().InvalidateSystemStore(requestContext, nameof (FrameworkProjectServiceBusReceiver));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5500260, FrameworkProjectServiceBusReceiver.s_area, FrameworkProjectServiceBusReceiver.s_layer, ex);
        throw;
      }
    }

    protected virtual void QueueProjectUpdateJob(
      IVssRequestContext requestContext,
      ProjectMessage projectMessage)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      try
      {
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          FrameworkProjectUpdateJobWorker.ProjectUpdateJob
        });
      }
      catch (JobDefinitionNotFoundException ex)
      {
        TeamFoundationJobDefinition jobDefinition = FrameworkProjectUpdateJobWorker.CreateJobDefinition(requestContext, projectMessage.Project.Revision);
        service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          jobDefinition
        });
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          FrameworkProjectUpdateJobWorker.ProjectUpdateJob
        });
      }
    }

    public TeamFoundationHostType AcceptedHostTypes => TeamFoundationHostType.ProjectCollection;
  }
}
