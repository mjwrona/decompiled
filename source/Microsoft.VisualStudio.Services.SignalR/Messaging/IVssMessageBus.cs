// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Messaging.IVssMessageBus
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.SignalR.Messaging
{
  public interface IVssMessageBus : IMessageBus
  {
    void Attach(IVssDeploymentServiceHost serviceHost);

    void Detach(IVssDeploymentServiceHost serviceHost);

    Task Start();
  }
}
