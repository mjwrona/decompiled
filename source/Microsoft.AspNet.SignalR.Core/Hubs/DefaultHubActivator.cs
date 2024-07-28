// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.DefaultHubActivator
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class DefaultHubActivator : IHubActivator
  {
    private readonly IDependencyResolver _resolver;

    public DefaultHubActivator(IDependencyResolver resolver) => this._resolver = resolver;

    public IHub Create(HubDescriptor descriptor)
    {
      if (descriptor == null)
        throw new ArgumentNullException(nameof (descriptor));
      return descriptor.HubType == (Type) null ? (IHub) null : (this._resolver.Resolve(descriptor.HubType) ?? Activator.CreateInstance(descriptor.HubType)) as IHub;
    }
  }
}
