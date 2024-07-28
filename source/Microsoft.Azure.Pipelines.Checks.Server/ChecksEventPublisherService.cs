// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.ChecksEventPublisherService
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.Common;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  public sealed class ChecksEventPublisherService : 
    IChecksEventPublisherService,
    IVssFrameworkService
  {
    private const string c_layer = "ChecksEventPublisher";

    public void NotifyCheckSuiteUpdatedEvent(
      IVssRequestContext requestContext,
      string eventType,
      Guid projectId,
      CheckSuite response)
    {
      using (new MethodScope(requestContext, "ChecksEventPublisher", nameof (NotifyCheckSuiteUpdatedEvent)))
      {
        requestContext.TraceVerbose(34001901, "ChecksEventPublisher", "Raised event with response data: {0}", (object) response);
        this.PublishEvent(requestContext, (object) new CheckSuiteUpdatedEvent(eventType, projectId, response));
      }
    }

    internal void PublishEvent(IVssRequestContext requestContext, object eventData)
    {
      try
      {
        requestContext.GetService<ITeamFoundationEventService>().SyncPublishNotification(requestContext, eventData);
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34001902, "ChecksEventPublisher", "Failed to fire event with data {0}. Exception received: {1}", eventData, (object) ex);
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
