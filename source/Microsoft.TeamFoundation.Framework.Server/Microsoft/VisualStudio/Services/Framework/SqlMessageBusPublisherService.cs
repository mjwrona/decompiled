// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Framework.SqlMessageBusPublisherService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Framework
{
  internal sealed class SqlMessageBusPublisherService : 
    IMessageBusPublisherService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void Publish(
      IVssRequestContext requestContext,
      string messageBusName,
      object[] serializableObjects,
      bool throwOnMissingPublisher = true,
      bool allowLoopback = true,
      bool includeAssignedHosts = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(messageBusName, nameof (messageBusName));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) serializableObjects, nameof (serializableObjects));
      if (!allowLoopback)
        return;
      SqlMessageBusManagementService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<SqlMessageBusManagementService>();
      List<MessageBusMessage> messageBusMessageList = new List<MessageBusMessage>(serializableObjects.Length);
      foreach (object serializableObject in serializableObjects)
      {
        if (!(serializableObject is MessageBusMessage messageBusMessage))
          messageBusMessage = new MessageBusMessage(serializableObject);
        messageBusMessageList.Add(messageBusMessage);
      }
      service.Publish(requestContext, messageBusName, messageBusMessageList.ToArray(), throwOnMissingPublisher);
    }

    public void TryPublish(
      IVssRequestContext requestContext,
      string messageBusName,
      object[] serializableObjects,
      bool throwOnMissingPublisher = true,
      bool allowLoopback = true,
      bool includeAssignedHosts = false)
    {
      this.Publish(requestContext, messageBusName, serializableObjects, throwOnMissingPublisher, allowLoopback, false);
    }

    public Task PublishAsync(
      IVssRequestContext requestContext,
      string messageBusName,
      object[] serializableObjects,
      bool throwOnMissingPublisher = true,
      bool allowLoopback = true,
      bool includeAssignedHosts = false)
    {
      this.Publish(requestContext, messageBusName, serializableObjects, throwOnMissingPublisher, allowLoopback, false);
      return Task.CompletedTask;
    }
  }
}
