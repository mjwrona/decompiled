// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticPoolDeleteListener
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  internal class ElasticPoolDeleteListener : ISubscriber
  {
    public string Name => nameof (ElasticPoolDeleteListener);

    public SubscriberPriority Priority => SubscriberPriority.Normal;

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEvent,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      statusCode = 0;
      properties = (ExceptionPropertyCollection) null;
      statusMessage = (string) null;
      if (notificationType == NotificationType.Notification)
      {
        DeleteAgentPoolEvent deleteEvent = notificationEvent as DeleteAgentPoolEvent;
        if (deleteEvent != null)
        {
          try
          {
            requestContext.RunSynchronously((Func<Task>) (async () => await this.DeleteElasticPoolAsync(requestContext, deleteEvent.PoolId)));
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "DistributedTask", this.Name, ex);
          }
        }
      }
      return EventNotificationStatus.ActionPermitted;
    }

    private async Task DeleteElasticPoolAsync(IVssRequestContext requestContext, int poolId)
    {
      requestContext = requestContext.Elevate();
      IElasticPoolService eps = requestContext.GetService<IElasticPoolService>();
      ElasticPool elasticPoolAsync = await eps.GetElasticPoolAsync(requestContext, poolId);
      if (elasticPoolAsync == null)
      {
        eps = (IElasticPoolService) null;
      }
      else
      {
        await eps.DeleteElasticPoolAsync(requestContext, elasticPoolAsync, true);
        eps = (IElasticPoolService) null;
      }
    }

    public Type[] SubscribedTypes() => new Type[1]
    {
      typeof (DeleteAgentPoolEvent)
    };
  }
}
