// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssMessageBusProvider
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SignalR.Messaging;

namespace Microsoft.VisualStudio.Services.SignalR
{
  internal class VssMessageBusProvider : IVssMessageBusProvider
  {
    public IVssMessageBus GetMessageBus(
      IVssRequestContext requestContext,
      bool supportsClientConnections)
    {
      VssMessageBusConfiguration busConfiguration = requestContext.GetService<IVssSignalRConfigurationService>().GetMessageBusConfiguration(requestContext);
      busConfiguration.QueueBehavior = QueuingBehavior.Disabled;
      busConfiguration.SupportsClientConnections = supportsClientConnections;
      return (IVssMessageBus) new VssMessageBus(requestContext.ServiceHost.DeploymentServiceHost, GlobalHost.DependencyResolver, busConfiguration);
    }
  }
}
