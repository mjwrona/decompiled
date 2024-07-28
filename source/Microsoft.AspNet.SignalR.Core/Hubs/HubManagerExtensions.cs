// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubManagerExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public static class HubManagerExtensions
  {
    public static HubDescriptor EnsureHub(
      this IHubManager hubManager,
      string hubName,
      params IPerformanceCounter[] counters)
    {
      if (hubManager == null)
        throw new ArgumentNullException(nameof (hubManager));
      if (string.IsNullOrEmpty(hubName))
        throw new ArgumentNullException(nameof (hubName));
      if (counters == null)
        throw new ArgumentNullException(nameof (counters));
      HubDescriptor hub = hubManager.GetHub(hubName);
      if (hub == null)
      {
        for (int index = 0; index < counters.Length; ++index)
          counters[index].Increment();
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_HubCouldNotBeResolved, new object[1]
        {
          (object) hubName
        }));
      }
      return hub;
    }

    public static IEnumerable<HubDescriptor> GetHubs(this IHubManager hubManager) => hubManager != null ? hubManager.GetHubs((Func<HubDescriptor, bool>) (d => true)) : throw new ArgumentNullException(nameof (hubManager));

    public static IEnumerable<MethodDescriptor> GetHubMethods(
      this IHubManager hubManager,
      string hubName)
    {
      return hubManager != null ? hubManager.GetHubMethods(hubName, (Func<MethodDescriptor, bool>) (m => true)) : throw new ArgumentNullException(nameof (hubManager));
    }
  }
}
