// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.IHubManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public interface IHubManager
  {
    HubDescriptor GetHub(string hubName);

    IEnumerable<HubDescriptor> GetHubs(Func<HubDescriptor, bool> predicate);

    IHub ResolveHub(string hubName);

    IEnumerable<IHub> ResolveHubs();

    MethodDescriptor GetHubMethod(string hubName, string method, IList<IJsonValue> parameters);

    IEnumerable<MethodDescriptor> GetHubMethods(
      string hubName,
      Func<MethodDescriptor, bool> predicate);
  }
}
