// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.EventExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public static class EventExtensions
  {
    private const string c_messageBusName = "Microsoft.VisualStudio.Services.ServiceHooks.Server";

    public static void CloneEventProperties(this Event destinationEvent, Event sourceEvent)
    {
      destinationEvent.CreatedDate = sourceEvent.CreatedDate;
      destinationEvent.DetailedMessage = sourceEvent.DetailedMessage;
      destinationEvent.EventType = sourceEvent.EventType;
      destinationEvent.Id = sourceEvent.Id;
      destinationEvent.Message = sourceEvent.Message;
      destinationEvent.Resource = sourceEvent.Resource;
      destinationEvent.ResourceContainers = sourceEvent.ResourceContainers;
      destinationEvent.ResourceVersion = sourceEvent.ResourceVersion;
      destinationEvent.PublisherId = sourceEvent.PublisherId;
      destinationEvent.SessionToken = sourceEvent.SessionToken;
    }

    public static void SimulateExternalEvent(
      this IVssRequestContext requestContext,
      object eventPayload)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        new MessageBusMessage(eventPayload).PublishAsNotification(requestContext);
      else
        requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext, "Microsoft.VisualStudio.Services.ServiceHooks.Server", new object[1]
        {
          eventPayload
        });
    }
  }
}
