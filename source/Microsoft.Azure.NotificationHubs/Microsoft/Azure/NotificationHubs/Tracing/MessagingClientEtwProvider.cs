// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.MessagingClientEtwProvider
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class MessagingClientEtwProvider
  {
    private const int WindowsVistaMajorNumber = 6;
    private static readonly bool IsVistaOrGreater = Environment.OSVersion.Version.Major >= 6;
    private static readonly object thisLock = new object();
    private static volatile MessagingClientEventSource provider;
    private static volatile EventListener diagnostics;

    public static MessagingClientEventSource Provider
    {
      get
      {
        if (MessagingClientEtwProvider.provider == null)
        {
          lock (MessagingClientEtwProvider.thisLock)
          {
            if (MessagingClientEtwProvider.provider == null)
            {
              MessagingClientEtwProvider.diagnostics = (EventListener) new ServiceBusEventListener(new Guid("A307C7A2-A4CD-4D22-8093-94DB72934152"));
              MessagingClientEtwProvider.provider = new MessagingClientEventSource(!MessagingClientEtwProvider.IsVistaOrGreater);
            }
          }
        }
        return MessagingClientEtwProvider.provider;
      }
    }

    public static void Close()
    {
      lock (MessagingClientEtwProvider.thisLock)
      {
        if (MessagingClientEtwProvider.provider == null)
          return;
        MessagingClientEtwProvider.diagnostics.Dispose();
        MessagingClientEtwProvider.provider.Dispose();
        MessagingClientEtwProvider.provider = (MessagingClientEventSource) null;
      }
    }

    public Guid Id => MessagingClientEtwProvider.provider.Guid;

    public static bool IsEtwEnabled() => MessagingClientEtwProvider.IsVistaOrGreater;

    public static void TraceClient(Action action)
    {
      if (!MessagingClientEtwProvider.IsVistaOrGreater)
        return;
      action();
    }

    public static void TraceClient<T1>(Action<T1> action, T1 state1)
    {
      if (!MessagingClientEtwProvider.IsVistaOrGreater)
        return;
      action(state1);
    }

    public static void TraceClient<T1, T2>(Action<T1, T2> action, T1 state1, T2 state2)
    {
      if (!MessagingClientEtwProvider.IsVistaOrGreater)
        return;
      action(state1, state2);
    }

    public static void TraceClient<T1, T2, T3>(
      Action<T1, T2, T3> action,
      T1 state1,
      T2 state2,
      T3 state3)
    {
      if (!MessagingClientEtwProvider.IsVistaOrGreater)
        return;
      action(state1, state2, state3);
    }

    public static void TraceClient<T1, T2, T3, T4>(
      Action<T1, T2, T3, T4> action,
      T1 state1,
      T2 state2,
      T3 state3,
      T4 state4)
    {
      if (!MessagingClientEtwProvider.IsVistaOrGreater)
        return;
      action(state1, state2, state3, state4);
    }
  }
}
