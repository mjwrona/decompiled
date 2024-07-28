// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubInvokerContext
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  internal class HubInvokerContext : IHubIncomingInvokerContext
  {
    public HubInvokerContext(
      IHub hub,
      StateChangeTracker tracker,
      MethodDescriptor methodDescriptor,
      IList<object> args)
    {
      this.Hub = hub;
      this.MethodDescriptor = methodDescriptor;
      this.Args = args;
      this.StateTracker = tracker;
    }

    public IHub Hub { get; private set; }

    public MethodDescriptor MethodDescriptor { get; private set; }

    public IList<object> Args { get; private set; }

    public StateChangeTracker StateTracker { get; private set; }
  }
}
