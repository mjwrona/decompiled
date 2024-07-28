// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubContext`1
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

namespace Microsoft.AspNet.SignalR.Hubs
{
  internal class HubContext<T> : IHubContext<T>
  {
    public HubContext(IHubContext dynamicContext)
    {
      TypedClientBuilder<T>.Validate();
      this.Clients = (IHubConnectionContext<T>) new TypedHubConnectionContext<T>(dynamicContext.Clients);
      this.Groups = dynamicContext.Groups;
    }

    public IHubConnectionContext<T> Clients { get; private set; }

    public IGroupManager Groups { get; private set; }
  }
}
