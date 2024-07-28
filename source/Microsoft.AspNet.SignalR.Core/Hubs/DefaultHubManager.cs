// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.DefaultHubManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class DefaultHubManager : IHubManager
  {
    private readonly IEnumerable<IMethodDescriptorProvider> _methodProviders;
    private readonly IHubActivator _activator;
    private readonly IEnumerable<IHubDescriptorProvider> _hubProviders;

    public DefaultHubManager(IDependencyResolver resolver)
    {
      this._hubProviders = resolver.ResolveAll<IHubDescriptorProvider>();
      this._methodProviders = resolver.ResolveAll<IMethodDescriptorProvider>();
      this._activator = resolver.Resolve<IHubActivator>();
    }

    public HubDescriptor GetHub(string hubName)
    {
      HubDescriptor descriptor = (HubDescriptor) null;
      if (this._hubProviders.FirstOrDefault<IHubDescriptorProvider>((Func<IHubDescriptorProvider, bool>) (p => p.TryGetHub(hubName, out descriptor))) == null)
        return (HubDescriptor) null;
      this.ValidateHubDescriptor(descriptor);
      return descriptor;
    }

    public IEnumerable<HubDescriptor> GetHubs(Func<HubDescriptor, bool> predicate)
    {
      IEnumerable<HubDescriptor> source = this._hubProviders.SelectMany<IHubDescriptorProvider, HubDescriptor>((Func<IHubDescriptorProvider, IEnumerable<HubDescriptor>>) (p => (IEnumerable<HubDescriptor>) p.GetHubs()));
      if (predicate != null)
        source = source.Where<HubDescriptor>(predicate);
      List<HubDescriptor> list = source.ToList<HubDescriptor>();
      foreach (HubDescriptor hub in list)
        this.ValidateHubDescriptor(hub);
      return (IEnumerable<HubDescriptor>) list;
    }

    public MethodDescriptor GetHubMethod(
      string hubName,
      string method,
      IList<IJsonValue> parameters)
    {
      HubDescriptor hub = this.GetHub(hubName);
      if (hub == null)
        return (MethodDescriptor) null;
      MethodDescriptor descriptor = (MethodDescriptor) null;
      return this._methodProviders.FirstOrDefault<IMethodDescriptorProvider>((Func<IMethodDescriptorProvider, bool>) (p => p.TryGetMethod(hub, method, out descriptor, parameters))) != null ? descriptor : (MethodDescriptor) null;
    }

    public IEnumerable<MethodDescriptor> GetHubMethods(
      string hubName,
      Func<MethodDescriptor, bool> predicate)
    {
      HubDescriptor hub = this.GetHub(hubName);
      if (hub == null)
        return (IEnumerable<MethodDescriptor>) null;
      IEnumerable<MethodDescriptor> source = this._methodProviders.SelectMany<IMethodDescriptorProvider, MethodDescriptor>((Func<IMethodDescriptorProvider, IEnumerable<MethodDescriptor>>) (p => p.GetMethods(hub)));
      return predicate != null ? source.Where<MethodDescriptor>(predicate) : source;
    }

    public IHub ResolveHub(string hubName)
    {
      HubDescriptor hub = this.GetHub(hubName);
      return hub != null ? this._activator.Create(hub) : (IHub) null;
    }

    public IEnumerable<IHub> ResolveHubs() => this.GetHubs((Func<HubDescriptor, bool>) null).Select<HubDescriptor, IHub>((Func<HubDescriptor, IHub>) (hub => this._activator.Create(hub)));

    private void ValidateHubDescriptor(HubDescriptor hub)
    {
      if (hub.Name.Contains("."))
        throw new InvalidOperationException(string.Format(Resources.Error_HubNameIsInvalid, (object) hub.Name));
    }
  }
}
