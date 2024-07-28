// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IMessageBusPublisherService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation("Microsoft.VisualStudio.Services.Cloud.ServiceBusPublisherService, Microsoft.VisualStudio.Services.Cloud")]
  public interface IMessageBusPublisherService : IVssFrameworkService
  {
    void Publish(
      IVssRequestContext requestContext,
      string messageBusName,
      object[] serializableObjects,
      bool throwOnMissingPublisher = true,
      bool allowLoopback = true,
      bool includeAssignedHosts = false);

    Task PublishAsync(
      IVssRequestContext requestContext,
      string messageBusName,
      object[] serializableObjects,
      bool throwOnMissingPublisher = true,
      bool allowLoopback = true,
      bool includeAssignedHosts = false);

    void TryPublish(
      IVssRequestContext requestContext,
      string messageBusName,
      object[] serializableObjects,
      bool throwOnMissingPublisher = true,
      bool allowLoopback = true,
      bool includeAssignedHosts = false);
  }
}
